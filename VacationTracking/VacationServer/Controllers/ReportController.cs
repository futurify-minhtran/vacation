using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VacationServer.Models;
using VacationServer.ServiceInterfaces;

namespace VacationServer.Controllers
{
    [Route("api/report")]
    public class ReportController : Controller
    {
        private IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet]
        public async Task<List<Booking>> GetAll(){
            return await _reportService.GetAllAsync();
        }

        [HttpGet, Route("{userId}")]
        public async Task<List<Booking>> GetAll(int userId)
        {
            return await _reportService.GetAllAsync(userId);
        }

        [HttpGet, Route("{userId:int}/{year:int}/{month:int}")]
        public async Task<List<Booking>> GetAll(int userId, int year, int month)
        {
            return await _reportService.GetAllAsync(userId,year,month+1);
        }

    }
}
