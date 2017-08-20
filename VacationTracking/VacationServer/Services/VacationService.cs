using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Common.Core;
using App.Common.Core.Exceptions;
using App.Common.Core.Models;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using VacationServer.Models;
using VacationServer.Resources;
using VacationServer.ServiceInterfaces;

namespace VacationServer.Services
{
    public class VacationService : IVacationService
    {
        private VacationDbContext _context;
        private IConfiguration _vacationConfig;
        private List<ConfigReceiveEmail> _configReceiveEmail;
        public VacationService(VacationDbContext context, IConfiguration vacationConfig, IOptions<List<ConfigReceiveEmail>> configReceiveEmail)
        {
            _context = context;
            _vacationConfig = vacationConfig;
            _configReceiveEmail = configReceiveEmail.Value;
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

        public async Task<List<Booking>> GetAllByUserIdAsync(int userId)
        {
            return await _context.Bookings.Where(b => b.UserId == userId).OrderBy(o => o.StartDate).ToListAsync();
        }
        
        // Check confict booking
        public async Task<bool> CheckBookingAsync(int userId, DateTime startDate, DateTime endDate, bool allDay, int? excludeBookingId = null)
        {
            // Un-use
            if ((startDate >= endDate && !allDay) || startDate.Year != endDate.Year || (startDate > endDate))
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

            if ((booking.StartDate >= booking.EndDate && !booking.AllDay) || booking.StartDate > booking.EndDate)
            {
                throw new CustomException(Error.INVALID_BOOKING, Error.INVALID_BOOKING_MSG);
            }

            var _workingTime = _vacationConfig.GetSection("WorkingTime");
            var _lunchBreak = _vacationConfig.GetSection("LunchBreak");

            var startTimeOfWorkingTime = DateTime.Parse(_workingTime["StartTime"]).TimeOfDay;
            var endTimeOfWorkingTime = DateTime.Parse(_workingTime["EndTime"]).TimeOfDay;

            var startTimeOfLunchBreak = DateTime.Parse(_lunchBreak["StartTime"]).TimeOfDay;
            var endTimeOfLunchBreak = DateTime.Parse(_lunchBreak["EndTime"]).TimeOfDay;

            var lunchBreak = (endTimeOfLunchBreak - startTimeOfLunchBreak).TotalHours;
            var workingTime = (endTimeOfWorkingTime - startTimeOfWorkingTime).TotalHours - lunchBreak;

            var startTimeOfBooking = booking.StartDate.TimeOfDay;
            var endTimeOfBooking = booking.EndDate.TimeOfDay;

            if (booking.AllDay)
            {
                booking.StartDate = booking.StartDate.Date + startTimeOfWorkingTime;
                booking.EndDate = booking.EndDate.Date + endTimeOfWorkingTime;
            }

            // Check conflict
            var checkBooking = await this.CheckBookingAsync(booking.UserId, booking.StartDate, booking.EndDate, booking.AllDay);
            if (!checkBooking)
            {
                throw new CustomException(Error.CONFLICT_BOOKING, Error.CONFLICT_BOOKING_MSG);
            }

            double totalHours = 0;
            var countWeekEnd = 0;
            var totalDays = (booking.EndDate.Date - booking.StartDate.Date).Days + 1;
            var dateNext = new DateTime();
            var check = totalDays - 2;
            bool checkInDay = booking.StartDate.Date == booking.EndDate.Date;

            // variable - vacation duration rule
            VacationConfig weekVacationDuration = await this.GetVacationConfigAsync("VacationDurationWeek");
            VacationConfig monthVacationDuration = await this.GetVacationConfigAsync("VacationDurationMonth");

            // variable end - vacation duration rule

            // Calculate by days
            if (booking.AllDay)
            {
                // One day
                if (checkInDay)
                {
                    totalHours = workingTime;
                }
                // Many days
                else
                {
                    // Count except week end days
                    dateNext = booking.StartDate.AddDays(1);
                    while (check > 0)
                    {
                        if (dateNext.DayOfWeek == DayOfWeek.Saturday || dateNext.DayOfWeek == DayOfWeek.Sunday)
                        {
                            countWeekEnd++;
                        }
                        dateNext = dateNext.AddDays(1);
                        check--;
                    }
                    totalHours = (totalDays - countWeekEnd) * workingTime;
                }
            }
            // Calculate by hours
            else
            {
                // Invalid when booking is outside of working time
                if (startTimeOfBooking < startTimeOfWorkingTime || endTimeOfBooking > endTimeOfWorkingTime)
                {
                    throw new CustomException(Error.INVALID_BOOKING, Error.INVALID_BOOKING_MSG
                        + ". <br/>Working Time: " + _workingTime["StartTime"] + " -> " + _workingTime["EndTime"]);
                }

                // Invalid when booking: starttime or endtime is inside Lunch break
                if ((startTimeOfBooking >= startTimeOfLunchBreak && endTimeOfBooking <= endTimeOfLunchBreak)
                    || (startTimeOfBooking >= startTimeOfLunchBreak && startTimeOfBooking < endTimeOfLunchBreak)
                    || (endTimeOfBooking > startTimeOfLunchBreak && endTimeOfBooking <= endTimeOfLunchBreak)
                    )
                {
                    throw new CustomException(Error.INVALID_BOOKING, Error.INVALID_BOOKING_MSG
                        + ". <br/>Lunch Break: " + _lunchBreak["StartTime"] + " -> " + _lunchBreak["EndTime"]);
                }

                // Hours in one day
                if (checkInDay)
                {
                    // In 1 side
                    if((startTimeOfBooking < startTimeOfLunchBreak && endTimeOfBooking <= startTimeOfLunchBreak)
                        || startTimeOfBooking >= endTimeOfLunchBreak && endTimeOfBooking > endTimeOfLunchBreak)
                    {
                        totalHours = endTimeOfBooking.TotalHours - startTimeOfBooking.TotalHours;
                    }
                    // In 2 sides
                    else
                    {
                        totalHours = endTimeOfBooking.TotalHours - startTimeOfBooking.TotalHours - lunchBreak;
                    }
                }
                // Hours in many days
                else
                {
                    double firstDayHours = 0;
                    double lastDayHours = 0;
                    double hours = 0;
                    // The first day hours
                    if (startTimeOfBooking >= startTimeOfWorkingTime && startTimeOfBooking <= startTimeOfLunchBreak)
                    {
                        firstDayHours = endTimeOfWorkingTime.TotalHours - startTimeOfBooking.TotalHours - lunchBreak;
                    }
                    else if (startTimeOfBooking >= endTimeOfLunchBreak && startTimeOfBooking <= endTimeOfWorkingTime)
                    {
                        firstDayHours = endTimeOfWorkingTime.TotalHours - startTimeOfBooking.TotalHours;
                    }
                    // The last day hours
                    if (endTimeOfBooking > startTimeOfWorkingTime && endTimeOfBooking <= startTimeOfLunchBreak)
                    {
                        lastDayHours = endTimeOfBooking.TotalHours - startTimeOfWorkingTime.TotalHours;
                    }
                    else if (endTimeOfBooking > endTimeOfLunchBreak && endTimeOfBooking <= endTimeOfWorkingTime)
                    {
                        lastDayHours = endTimeOfBooking.TotalHours - startTimeOfWorkingTime.TotalHours - lunchBreak;
                    }

                    // allday from 2nd to before last day
                    dateNext = booking.StartDate.AddDays(1);
                    while (check > 0)
                    {
                        if (dateNext.DayOfWeek == DayOfWeek.Saturday || dateNext.DayOfWeek == DayOfWeek.Sunday)
                        {
                            countWeekEnd++;
                        }
                        dateNext = dateNext.AddDays(1);
                        check--;
                    }
                    hours = (totalDays - countWeekEnd - 2) * workingTime;

                    totalHours = firstDayHours + hours + lastDayHours;
                }
            }
            

            // check user is still remaining vacation day
            var userId = booking.UserId;
            var year = booking.EndDate.Year;
            var bookingVacationDay = await this.GetBookingVacationDay(userId, year);
            var vacationDay = (double)await this.GetVacationDay(userId, year) * 8;
            if ((totalHours + bookingVacationDay) > vacationDay)
            {
                throw new CustomException(Error.BOOKING_NOT_ENOUGH, Error.BOOKING_NOT_ENOUGH_MSG
                    + ", it's over <b>" + (totalHours + bookingVacationDay - vacationDay) + "</b> hours.");
            }

            // Check vacation duration > 1 and <= 5, not allow to book with in a week
            // >5, not allow to book with in a month
            // can be config
            var vacationDuration = totalHours / 8;
            if(monthVacationDuration.Status && vacationDuration > monthVacationDuration.Value.ConvertToInt())
            {
                if(booking.StartDate < (DateTime.Now).Date.AddDays(30))
                {
                    throw new CustomException(Error.BOOKING_RULE_MONTH, String.Format(Error.BOOKING_RULE_MONTH_MSG, monthVacationDuration.Value.ConvertToInt()));
                }
            }
            else if(weekVacationDuration.Status && vacationDuration > weekVacationDuration.Value.ConvertToInt())
            {
                if (booking.StartDate < (DateTime.Now).Date.AddDays(7))
                {
                    throw new CustomException(Error.BOOKING_RULE_WEEK, String.Format(Error.BOOKING_RULE_WEEK_MSG, weekVacationDuration.Value.ConvertToInt()));
                }
            }
            // end - check vacation duration

            booking.TotalHours = totalHours;

            var now = DateTime.Now;
            booking.CreatedAt = now;
            booking.ModifiedAt = now;

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<Booking> UpdateAsync(Booking booking)
        {
            if (booking == null)
            {
                throw new CustomException(Error.BOOKING_IS_NULL, Error.BOOKING_IS_NULL_MSG);
            }

            var existingBooking = await GetByIdAsync(booking.Id);
            if (existingBooking == null)
            {
                throw new CustomException(Error.BOOKING_NOT_FOUND, Error.BOOKING_NOT_FOUND_MSG);
            }

            if ((booking.StartDate >= booking.EndDate && !booking.AllDay) || booking.StartDate > booking.EndDate)
            {
                throw new CustomException(Error.INVALID_BOOKING, Error.INVALID_BOOKING_MSG);
            }

            

            var _workingTime = _vacationConfig.GetSection("WorkingTime");
            var _lunchBreak = _vacationConfig.GetSection("LunchBreak");

            var startTimeOfWorkingTime = DateTime.Parse(_workingTime["StartTime"]).TimeOfDay;
            var endTimeOfWorkingTime = DateTime.Parse(_workingTime["EndTime"]).TimeOfDay;

            var startTimeOfLunchBreak = DateTime.Parse(_lunchBreak["StartTime"]).TimeOfDay;
            var endTimeOfLunchBreak = DateTime.Parse(_lunchBreak["EndTime"]).TimeOfDay;

            var lunchBreak = (endTimeOfLunchBreak - startTimeOfLunchBreak).TotalHours;
            var workingTime = (endTimeOfWorkingTime - startTimeOfWorkingTime).TotalHours - lunchBreak;

            var startTimeOfBooking = booking.StartDate.TimeOfDay;
            var endTimeOfBooking = booking.EndDate.TimeOfDay;

            if (booking.AllDay)
            {
                booking.StartDate = booking.StartDate.Date + startTimeOfWorkingTime;
                booking.EndDate = booking.EndDate.Date + endTimeOfWorkingTime;
            }

            // Check conflict
            var checkBooking = await this.CheckBookingAsync(existingBooking.UserId, booking.StartDate, booking.EndDate, booking.AllDay, existingBooking.Id);
            if (!checkBooking)
            {
                throw new CustomException(Error.CONFLICT_BOOKING, Error.CONFLICT_BOOKING_MSG);
            }

            double totalHours = 0;
            var countWeekEnd = 0;
            var totalDays = (booking.EndDate.Date - booking.StartDate.Date).Days + 1;
            var dateNext = new DateTime();
            var check = totalDays - 2;
            bool checkInDay = booking.StartDate.Date == booking.EndDate.Date;

            // variable - vacation duration rule
            VacationConfig weekVacationDuration = await this.GetVacationConfigAsync("VacationDurationWeek");
            VacationConfig monthVacationDuration = await this.GetVacationConfigAsync("VacationDurationMonth");

            // variable end - vacation duration rule

            // Calculate by days
            if (booking.AllDay)
            {
                // One day
                if (checkInDay)
                {
                    totalHours = workingTime;
                }
                // Many days
                else
                {
                    // Count except week end days
                    dateNext = booking.StartDate.AddDays(1);
                    while (check > 0)
                    {
                        if (dateNext.DayOfWeek == DayOfWeek.Saturday || dateNext.DayOfWeek == DayOfWeek.Sunday)
                        {
                            countWeekEnd++;
                        }
                        dateNext = dateNext.AddDays(1);
                        check--;
                    }
                    totalHours = (totalDays - countWeekEnd) * workingTime;
                }
            }
            // Calculate by hours
            else
            {
                // Invalid when booking is outside of working time
                if (startTimeOfBooking < startTimeOfWorkingTime || endTimeOfBooking > endTimeOfWorkingTime)
                {
                    throw new CustomException(Error.INVALID_BOOKING, Error.INVALID_BOOKING_MSG
                        + ". <br/>Working Time: " + _workingTime["StartTime"] + " -> " + _workingTime["EndTime"]);
                }

                // Invalid when booking: starttime or endtime is inside Lunch break
                if ((startTimeOfBooking >= startTimeOfLunchBreak && endTimeOfBooking <= endTimeOfLunchBreak)
                    || (startTimeOfBooking >= startTimeOfLunchBreak && startTimeOfBooking < endTimeOfLunchBreak)
                    || (endTimeOfBooking > startTimeOfLunchBreak && endTimeOfBooking <= endTimeOfLunchBreak)
                    )
                {
                    throw new CustomException(Error.INVALID_BOOKING, Error.INVALID_BOOKING_MSG
                        + ". <br/>Lunch Break: " + _lunchBreak["StartTime"] + " -> " + _lunchBreak["EndTime"]);
                }

                // Hours in one day
                if (checkInDay)
                {
                    // In 1 side
                    if ((startTimeOfBooking < startTimeOfLunchBreak && endTimeOfBooking <= startTimeOfLunchBreak)
                        || startTimeOfBooking >= endTimeOfLunchBreak && endTimeOfBooking > endTimeOfLunchBreak)
                    {
                        totalHours = endTimeOfBooking.TotalHours - startTimeOfBooking.TotalHours;
                    }
                    // In 2 sides
                    else
                    {
                        totalHours = endTimeOfBooking.TotalHours - startTimeOfBooking.TotalHours - lunchBreak;
                    }
                }
                // Hours in many days
                else
                {
                    double firstDayHours = 0;
                    double lastDayHours = 0;
                    double hours = 0;
                    // The first day hours
                    if (startTimeOfBooking >= startTimeOfWorkingTime && startTimeOfBooking <= startTimeOfLunchBreak)
                    {
                        firstDayHours = endTimeOfWorkingTime.TotalHours - startTimeOfBooking.TotalHours - lunchBreak;
                    }
                    else if (startTimeOfBooking >= endTimeOfLunchBreak && startTimeOfBooking <= endTimeOfWorkingTime)
                    {
                        firstDayHours = endTimeOfWorkingTime.TotalHours - startTimeOfBooking.TotalHours;
                    }
                    // The last day hours
                    if (endTimeOfBooking > startTimeOfWorkingTime && endTimeOfBooking <= startTimeOfLunchBreak)
                    {
                        lastDayHours = endTimeOfBooking.TotalHours - startTimeOfWorkingTime.TotalHours;
                    }
                    else if (endTimeOfBooking > endTimeOfLunchBreak && endTimeOfBooking <= endTimeOfWorkingTime)
                    {
                        lastDayHours = endTimeOfBooking.TotalHours - startTimeOfWorkingTime.TotalHours - lunchBreak;
                    }

                    // allday from 2nd to before last day
                    dateNext = booking.StartDate.AddDays(1);
                    while (check > 0)
                    {
                        if (dateNext.DayOfWeek == DayOfWeek.Saturday || dateNext.DayOfWeek == DayOfWeek.Sunday)
                        {
                            countWeekEnd++;
                        }
                        dateNext = dateNext.AddDays(1);
                        check--;
                    }
                    hours = (totalDays - countWeekEnd - 2) * workingTime;

                    totalHours = firstDayHours + hours + lastDayHours;
                }
            }

            // check user is still remaining vacation day
            var userId = booking.UserId;
            var year = booking.EndDate.Year;
            // because update, must except booking is updating
            var bookingVacationDay = await this.GetBookingVacationDay(userId, year) - existingBooking.TotalHours;
            var vacationDay = (double)await this.GetVacationDay(userId, year) * 8;
            if ((totalHours + bookingVacationDay ) > vacationDay)
            {
                throw new CustomException(Error.BOOKING_NOT_ENOUGH, Error.BOOKING_NOT_ENOUGH_MSG
                    + ", it's over <b>" + (totalHours + bookingVacationDay - vacationDay) + "</b> hours.");
            }

            // Check vacation duration > 1 and <= 5, not allow to book with in a week
            // >5, not allow to book with in a month
            // can be config
            var vacationDuration = totalHours / 8;
            if (monthVacationDuration.Status && vacationDuration > monthVacationDuration.Value.ConvertToInt())
            {
                if (booking.StartDate < (DateTime.Now).Date.AddDays(30))
                {
                    throw new CustomException(Error.BOOKING_RULE_MONTH, String.Format(Error.BOOKING_RULE_MONTH_MSG, monthVacationDuration.Value.ConvertToInt()));
                }
            }
            else if (weekVacationDuration.Status && vacationDuration > weekVacationDuration.Value.ConvertToInt())
            {
                if (booking.StartDate < (DateTime.Now).Date.AddDays(7))
                {
                    throw new CustomException(Error.BOOKING_RULE_WEEK, String.Format(Error.BOOKING_RULE_WEEK_MSG, weekVacationDuration.Value.ConvertToInt()));
                }
            }
            // end - check vacation duration

            existingBooking.TotalHours = totalHours;

            existingBooking.StartDate = booking.StartDate;
            existingBooking.EndDate = booking.EndDate;
            existingBooking.BookingType = booking.BookingType;
            existingBooking.Reason = booking.Reason;

            existingBooking.ModifiedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return existingBooking;
        }

        public async Task<Booking> DeleteAsync(int id)
        {
            var existingBooking = await this.GetByIdAsync(id);
            if (existingBooking == null)
            {
                throw new CustomException(Error.BOOKING_NOT_FOUND, Error.BOOKING_NOT_FOUND_MSG);
            }

            _context.Bookings.Remove(existingBooking);
            await _context.SaveChangesAsync();

            return existingBooking;
        }

        public async Task<int> GetVacationDay(int userId, int year)
        {
            var vacationDay = await _context.VacationDays.FirstOrDefaultAsync(v => v.UserId == userId && v.Year == year);

            return vacationDay.TotalMonth;
        }

        public async Task<double> GetBookingVacationDay(int userId, int year)
        {
            return await _context.Bookings
                .Where(b => b.UserId == userId && b.EndDate.Year == year)
                .SumAsync(b => b.TotalHours);
        }

        public async Task<bool> CheckNewUser(int userId)
        {
            var result = await _context.VacationDays.AnyAsync(vd => vd.UserId == userId);
            return !result;
        }

        public async Task<VacationDay> CreateAsync(VacationDay vacationDay)
        {
            if(vacationDay == null)
            {
                throw new CustomException(Error.VACATIONDAY_IS_NULL, Error.VACATIONDAY_IS_NULL_MSG);
            }

            var now = DateTime.Now;
            vacationDay.CreatedAt = now;
            vacationDay.ModifiedAt = now;

            _context.VacationDays.Add(vacationDay);
            await _context.SaveChangesAsync();

            return vacationDay;
        }

        public async Task<List<VacationConfig>> GetVacationConfigAsync()
        {
            return await _context.VacationConfigs.ToListAsync();
        }

        public async Task<VacationConfig> GetVacationConfigAsync(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new CustomException(Error.INVALID_REQUEST, Error.INVALID_REQUEST_MSG);
            }

            return await _context.VacationConfigs.FirstOrDefaultAsync(vc => vc.Name == name);
        }

        public async Task<VacationConfig> UpdateVacationConfigAsync(VacationConfig vacationConfig)
        {
            if(vacationConfig == null)
            {
                throw new CustomException(Error.VACATION_CONFIG_IS_NULL, Error.VACATION_CONFIG_IS_NULL_MSG);
            }

            var existingVacationConfig = await this.GetVacationConfigAsync(vacationConfig.Name);

            if(existingVacationConfig == null)
            {
                throw new CustomException(Error.VACATION_CONFIG_NOT_FOUND, Error.VACATION_CONFIG_NOT_FOUND_MSG);
            }

            existingVacationConfig.Value = vacationConfig.Value;
            existingVacationConfig.ModifiedAt = DateTime.Now;
            existingVacationConfig.Status = vacationConfig.Status;

            await _context.SaveChangesAsync();

            return existingVacationConfig;

        }

        public async Task<VacationConfig> SetStatusVacationConfigAsync(VacationConfig vacationConfig)
        {
            if (vacationConfig == null)
            {
                throw new CustomException(Error.VACATION_CONFIG_IS_NULL, Error.VACATION_CONFIG_IS_NULL_MSG);
            }

            var existing = await this.GetVacationConfigAsync(vacationConfig.Name);

            if (existing == null)
            {
                throw new CustomException(Error.VACATION_CONFIG_NOT_FOUND, Error.VACATION_CONFIG_NOT_FOUND_MSG);
            }

            existing.Status = vacationConfig.Status;

            await _context.SaveChangesAsync();

            return existing;
        }

        public async Task SendMailBooking(ConfigSendEmail configSendEmail, string email, Booking booking)
        {
            var sender = configSendEmail.Sender;
            var username = configSendEmail.Username;
            var password = configSendEmail.Password;
            var host = configSendEmail.Host;
            var port = configSendEmail.Port.ConvertToInt();

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Vacation Tracking", sender));
            foreach (var receive in _configReceiveEmail)
            {
                emailMessage.To.Add(new MailboxAddress(receive.Name, receive.Address));
            }

            emailMessage.Subject = email + " have been booked for the vacation day";

            var mailTemplate = "<p>Dear Admin!</p>";
            mailTemplate += "<br />";
            mailTemplate += "<p>User {0} have been booked for the vacation day.</p>";
            mailTemplate += "<br />";
            if (booking.AllDay)
            {
                mailTemplate += "<p>Start date: {1:dd/MM/yyyy}</p>";
                mailTemplate += "<p>End date: {2:dd/MM/yyyy}</p>";
            }
            else
            {
                mailTemplate += "<p>Start date: {1}</p>";
                mailTemplate += "<p>End date: {2}</p>";
            }
            mailTemplate += "<p>Reason: {3}</p>";
            mailTemplate += "<p>Vacation duration: {4} hours</p>";

            emailMessage.Body = new TextPart("html")
            {
                Text = string.Format(mailTemplate, email, booking.StartDate, booking.EndDate, booking.Reason, booking.TotalHours)
            };

            using (var client = new SmtpClient())
            {
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await client.ConnectAsync(host, port, false);
                client.AuthenticationMechanisms.Remove("XOAUTH2"); // Must be removed for Gmail SMTP
                await client.AuthenticateAsync(username, password);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }

        }

        public async Task SendMailCancelBooking(ConfigSendEmail configSendEmail, string email, Booking booking)
        {
            var sender = configSendEmail.Sender;
            var username = configSendEmail.Username;
            var password = configSendEmail.Password;
            var host = configSendEmail.Host;
            var port = configSendEmail.Port.ConvertToInt();

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Vacation Tracking", sender));
            foreach (var receive in _configReceiveEmail)
            {
                emailMessage.To.Add(new MailboxAddress(receive.Name, receive.Address));
            }

            emailMessage.Subject = email + " have been cancelled booking for the vacation day";

            var mailTemplate = "<p>Dear Admin!</p>";
            mailTemplate += "<br />";
            mailTemplate += "<p>User {0} have been cancelled booking for the vacation day.</p>";
            mailTemplate += "<br />";
            if (booking.AllDay)
            {
                mailTemplate += "<p>Start date: {1:dd/MM/yyyy}</p>";
                mailTemplate += "<p>End date: {2:dd/MM/yyyy}</p>";
            }
            else
            {
                mailTemplate += "<p>Start date: {1}</p>";
                mailTemplate += "<p>End date: {2}</p>";
            }
            mailTemplate += "<p>Reason: {3}</p>";
            mailTemplate += "<p>Vacation duration: {4} hours</p>";

            emailMessage.Body = new TextPart("html")
            {
                Text = string.Format(mailTemplate, email, booking.StartDate, booking.EndDate, booking.Reason, booking.TotalHours)
            };

            using (var client = new SmtpClient())
            {
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await client.ConnectAsync(host, port, false);
                client.AuthenticationMechanisms.Remove("XOAUTH2"); // Must be removed for Gmail SMTP
                await client.AuthenticateAsync(username, password);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }

        }

        public async Task SendMailEditBooking(ConfigSendEmail configSendEmail, string email, Booking booking)
        {
            var sender = configSendEmail.Sender;
            var username = configSendEmail.Username;
            var password = configSendEmail.Password;
            var host = configSendEmail.Host;
            var port = configSendEmail.Port.ConvertToInt();

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Vacation Tracking", sender));
            foreach (var receive in _configReceiveEmail)
            {
                emailMessage.To.Add(new MailboxAddress(receive.Name, receive.Address));
            }

            emailMessage.Subject = email + " have been edited booking for the vacation day";

            var mailTemplate = "<p>Dear Admin!</p>";
            mailTemplate += "<br />";
            mailTemplate += "<p>User {0} have been edited booking for the vacation day.</p>";
            mailTemplate += "<br />";
            if (booking.AllDay)
            {
                mailTemplate += "<p>Start date: {1:dd/MM/yyyy}</p>";
                mailTemplate += "<p>End date: {2:dd/MM/yyyy}</p>";
            }
            else
            {
                mailTemplate += "<p>Start date: {1}</p>";
                mailTemplate += "<p>End date: {2}</p>";
            }
            mailTemplate += "<p>Reason: {3}</p>";
            mailTemplate += "<p>Vacation duration: {4} hours</p>";

            emailMessage.Body = new TextPart("html")
            {
                Text = string.Format(mailTemplate, email, booking.StartDate, booking.EndDate, booking.Reason, booking.TotalHours)
            };

            using (var client = new SmtpClient())
            {
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await client.ConnectAsync(host, port, false);
                client.AuthenticationMechanisms.Remove("XOAUTH2"); // Must be removed for Gmail SMTP
                await client.AuthenticateAsync(username, password);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }

        }
    }
}
