using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Common.Core.Models;
using VacationServer.Models;

namespace VacationServer.ServiceInterfaces
{
    public interface IVacationService
    {
        Task<Booking> GetByIdAsync(int id);
        Task<List<Booking>> GetByUserIdAsync(int userId, int? excludeBookingId = null);
        Task<List<Booking>> GetAllAsync();
        Task<List<Booking>> GetAllByUserIdAsync(int userId);
        Task<bool> CheckBookingAsync(int userId, DateTime startDate, DateTime endDate, bool allDay ,int? excludeBookingId = null);
        Task<Booking> CreateAsync(Booking booking);
        Task<Booking> UpdateAsync(Booking booking);
        Task<Booking> DeleteAsync(int id);

        Task<int> GetVacationDay(int userId, int year);
        Task<double> GetBookingVacationDay(int userId, int year);
        Task<bool> CheckNewUser(int userId);
        Task<VacationDay> CreateAsync(VacationDay vacationDay);

        Task<List<VacationConfig>> GetVacationConfigAsync();
        Task<VacationConfig> GetVacationConfigAsync(string name);
        Task<VacationConfig> UpdateVacationConfigAsync(VacationConfig _vacationAsync);
        Task<VacationConfig> SetStatusVacationConfigAsync(VacationConfig vacationConfig);

        Task SendMailBooking(ConfigSendEmail configSendEmail, string email, Booking booking);

        Task SendMailCancelBooking(ConfigSendEmail configSendEmail, string email, Booking booking);
    }
}
