using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationServer.Models;

namespace VacationServer.ServiceInterfaces
{
    public interface IVacationService
    {
        Task<Booking> GetByIdAsync(int id);
        Task<List<Booking>> GetByUserIdAsync(int userId, int? excludeBookingId = null);
        Task<bool> CheckBookingAsync(int userId, DateTime startDate, DateTime endDate, int? excludeBookingId = null);
        Task<Booking> CreateAsync(Booking booking);
        Task<Booking> UpdateAsync(Booking booking);
        Task DeleteAsync(int id);
    }
}
