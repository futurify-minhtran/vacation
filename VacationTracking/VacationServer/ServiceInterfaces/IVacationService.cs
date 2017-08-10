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
        Task<List<Booking>> GetAllAsync();
        Task<List<Booking>> GetAllByUserIdAsync(int userId);
        Task<bool> CheckBookingAsync(int userId, DateTime startDate, DateTime endDate, int? excludeBookingId = null);
        Task<Booking> CreateAsync(Booking booking);
        Task<Booking> UpdateAsync(Booking booking);
        Task DeleteAsync(int id);

        Task<int> GetVacationDay(int userId, int year);
        Task<double> GetBookingVacationDay(int userId, int year);
        Task<bool> CheckNewUser(int userId);
        Task<VacationDay> CreateAsync(VacationDay vacationDay);
    }
}
