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

        public async Task<List<Booking>> GetAllAsync()
        {
            return await _context.Bookings.ToListAsync();
        }
        public async Task<List<Booking>> GetAllAsync(int userId)
        {
            return await _context.Bookings.Where(b => b.UserId == userId).ToListAsync();
        }
        public async Task<List<Booking>> GetAllAsync(int userId, int year, int month)
        {
            return await _context.Bookings.Where(b => b.UserId == userId && b.EndDate.Year == year && b.EndDate.Month == month).ToListAsync();
        }
    }
}
