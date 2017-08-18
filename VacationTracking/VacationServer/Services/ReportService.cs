using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VacationServer.Models;
using VacationServer.ServiceInterfaces;

namespace VacationServer.Services
{
    public class ReportService : IReportService
    {
        private VacationDbContext _context;

        public ReportService(VacationDbContext context)
        {
            _context = context;
        }

        public async Task<List<List<Booking>>> GetAllAsync()
        {
            var userIds = _context.Bookings.Select(b => b.UserId).Distinct();

            var bookings = new List<List<Booking>>();

            foreach(var userId in userIds)
            {
                var bookingsPerUserId = await this.GetAllAsync(userId);
                bookings.Add(bookingsPerUserId);
            }

            return bookings;
        }
        public async Task<List<Booking>> GetAllAsync(int userId)
        {
            return await _context.Bookings.Where(b => b.UserId == userId).OrderBy(b => b.StartDate).ToListAsync();
        }

        public async Task<List<List<Booking>>> GetAllAsync(int year, int month)
        {
            var userIds = _context.Bookings.Select(b => b.UserId).Distinct();

            var bookings = new List<List<Booking>>();

            foreach (var userId in userIds)
            {
                var bookingsPerUserId = await this.GetAllAsync(userId,year,month);
                bookings.Add(bookingsPerUserId);
            }

            return bookings;
        }
        public async Task<List<Booking>> GetAllAsync(int userId, int year, int month)
        {
            return await _context.Bookings.Where(b => b.UserId == userId && b.EndDate.Year == year && b.EndDate.Month == month).OrderBy(b => b.StartDate).ToListAsync();
        }
    }
}
