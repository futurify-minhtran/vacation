using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Common.Core.Exceptions;
using Microsoft.EntityFrameworkCore;
using VacationServer.Models;
using VacationServer.Resources;
using VacationServer.ServiceInterfaces;

namespace VacationServer.Services
{
    public class VacationService : IVacationService
    {
        private VacationDbContext _context;

        public VacationService(VacationDbContext context)
        {
            _context = context;
        }

        public async Task<Booking> GetByIdAsync(int id)
        {
            return await _context.Bookings.FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<List<Booking>> GetByUserIdAsync(int userId, int? excludeBookingId = null)
        {
            if(excludeBookingId == null)
            {
                return await _context.Bookings.Where(b => b.UserId == userId).ToListAsync();
            } else
            {
                return await _context.Bookings.Where(b => b.UserId == userId && b.Id != excludeBookingId).ToListAsync();

            }
        }

        public async Task<List<Booking>> GetAllAsync()
        {
            return await _context.Bookings.OrderBy(o => o.StartDate).ToListAsync();
        }

        // Check confict booking
        public async Task<bool> CheckBookingAsync(int userId, DateTime startDate, DateTime endDate, int? excludeBookingId = null)
        {
            // Un-use
            if (startDate >= endDate)
            {
                return false;
            }

            var listBooking = await this.GetByUserIdAsync(userId, excludeBookingId);

            foreach (var item in listBooking)
            {
                // startDate any, endDate inside or equal endDate of (existing booking time)
                if(endDate > item.StartDate && endDate <= item.EndDate)
                {
                    return false;
                }
                // endDate any, startDate inside or equal startDate of (existing booking time)
                if (startDate >= item.StartDate && startDate < item.EndDate)
                {
                    return false;
                }

                // startDate and endDate cover or equal of (existing booking time)
                if (startDate <= item.StartDate && endDate >= item.EndDate)
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<Booking> CreateAsync(Booking booking)
        {
            if (booking == null)
            {
                throw new CustomException(Error.BOOKING_IS_NULL, Error.BOOKING_IS_NULL_MSG);
            }
            if (booking.StartDate >= booking.EndDate)
            {
                throw new CustomException(Error.INVALID_BOOKING, Error.INVALID_BOOKING_MSG);
            }

            var checkBooking = await this.CheckBookingAsync(booking.UserId, booking.StartDate, booking.EndDate);
            if (!checkBooking)
            {
                throw new CustomException(Error.CONFLICT_BOOKING, Error.CONFLICT_BOOKING_MSG);
            }

            var now = DateTime.Now;
            booking.CreatedAt = now;
            booking.ModifiedAt = now;

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<Booking> UpdateAsync(Booking booking)
        {
            if(booking == null)
            {
                throw new CustomException(Error.BOOKING_IS_NULL, Error.BOOKING_IS_NULL_MSG);
            }

            var existingBooking = await GetByIdAsync(booking.Id);
            if(existingBooking == null)
            {
                throw new CustomException(Error.BOOKING_NOT_FOUND, Error.BOOKING_NOT_FOUND_MSG);
            }

            var checkBooking = await this.CheckBookingAsync(existingBooking.UserId, booking.StartDate, booking.EndDate, existingBooking.Id);

            if (!checkBooking)
            {
                throw new CustomException(Error.CONFLICT_BOOKING, Error.CONFLICT_BOOKING_MSG);
            }

            existingBooking.StartDate = booking.StartDate;
            existingBooking.EndDate = booking.EndDate;
            existingBooking.BookingType = booking.BookingType;
            existingBooking.Reason = booking.Reason;

            existingBooking.ModifiedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return existingBooking;
        }

        public async Task DeleteAsync(int id)
        {
            var existingBooking = await this.GetByIdAsync(id);
            if (existingBooking == null)
            {
                throw new CustomException(Error.BOOKING_NOT_FOUND, Error.BOOKING_NOT_FOUND_MSG);
            }

            _context.Bookings.Remove(existingBooking);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetVacationDay(int userId, int year)
        {
            var vacationDay = await _context.VacationDays.FirstOrDefaultAsync(v => v.UserId == userId && v.Year == year);

            return vacationDay.TotalMonth;
        }
    }
}
