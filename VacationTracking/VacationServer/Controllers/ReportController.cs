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
        public async Task<ActionResult> GetAll(){
            var bookings = await _reportService.GetAllAsync();


            var result = new List<Object>();

            foreach(var bookingsPerUserId in bookings)
            {
                double totalHours = 0;

                foreach (var booking in bookingsPerUserId)
                {
                    totalHours += booking.TotalHours;
                }
                result.Add(new
                {
                    Bookings = bookingsPerUserId,
                    TotalHours = totalHours
                });
            }

            return Json(result);
        }

        [HttpGet, Route("{userId}")]
        public async Task<ActionResult> GetAll(int userId)
        {
            var bookings = await _reportService.GetAllAsync(userId);

            double totalHours = 0;

            foreach (var booking in bookings)
            {
                totalHours += booking.TotalHours;
            }

            return Json(new
            {
                Bookings = bookings,
                TotalHours = totalHours
            });
        }

        [HttpGet, Route("{year:int}/{month:int}")]
        public async Task<ActionResult> GetAll(int year, int month)
        {
            var bookings = await _reportService.GetAllAsync(year, month + 1);


            var result = new List<Object>();

            foreach (var bookingsPerUserId in bookings)
            {
                double totalHours = 0;

                foreach (var booking in bookingsPerUserId)
                {
                    totalHours += booking.TotalHours;
                }
                result.Add(new
                {
                    Bookings = bookingsPerUserId,
                    TotalHours = totalHours
                });
            }

            return Json(result);
        }

        [HttpGet, Route("{userId:int}/{year:int}/{month:int}")]
        public async Task<ActionResult> GetAll(int userId, int year, int month)
        {
            var bookings = await _reportService.GetAllAsync(userId,year,month+1);

            double totalHours = 0;

            foreach (var booking in bookings)
            {
                totalHours += booking.TotalHours;
            }

            return Json(new
            {
                Bookings = bookings,
                TotalHours = totalHours
            });
        }

    }
}
