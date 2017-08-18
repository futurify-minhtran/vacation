using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationServer.Models;

namespace VacationServer.ServiceInterfaces
{
    public interface IReportService
    {
        Task<List<Booking>> GetAllAsync();
        Task<List<Booking>> GetAllAsync(int userId);
        Task<List<Booking>> GetAllAsync(int year, int month);
        Task<List<Booking>> GetAllAsync(int userId, int year, int month);
    }
}
