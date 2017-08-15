﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Common.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using VacationServer.Adapters;
using VacationServer.Models;
using VacationServer.Models.BindingModels;
using VacationServer.Resources;
using VacationServer.ServiceInterfaces;

namespace VacationServer.Controllers
{
    [Route("api/vacation")]
    public class VacationControler : Controller
    {
        IVacationService _vacationService;

        public VacationControler(IVacationService vacationService)
        {
            _vacationService = vacationService;
        }

        [HttpPost, Route("booking")]
        public async Task<ActionResult> Create([FromBody]BookingBindingModel bookingBindingModel)
        {
            try
            {
                var bookingModel = bookingBindingModel.ToModel();

                if (bookingModel == null || !ModelState.IsValid)
                {
                    throw new CustomException(Error.INVALID_BOOKING, Error.INVALID_BOOKING_MSG);
                }

                bookingModel = await _vacationService.CreateAsync(bookingModel);

                var bookingViewModel = bookingModel.ToViewMode();

                return Json(new { Booking = bookingViewModel });
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message });
            }
        }

        [HttpGet, Route("booking/{id:int}")]
        public async Task<Booking> Read(int id)
        {
            return await _vacationService.GetByIdAsync(id);
        }

        [HttpGet, Route("booking")]
        public async Task<List<Booking>> ReadAll()
        {
            return await _vacationService.GetAllAsync();
        }

        [HttpGet, Route("booking/user/{userId:int}")]
        public async Task<List<Booking>> ReadAll(int userId)
        {
            return await _vacationService.GetAllByUserIdAsync(userId);
        }

        [HttpPut, Route("booking")]
        public async Task<Booking> Update([FromBody]BookingBindingModel bookingBindingModel)
        {
            if (bookingBindingModel == null || !ModelState.IsValid)
            {
                throw new CustomException(Error.INVALID_REQUEST, Error.INVALID_REQUEST_MSG);
            }

            var bookingModel = bookingBindingModel.ToModel();

            var updatedBookingModel = await _vacationService.UpdateAsync(bookingModel);

            return updatedBookingModel;
        }

        [HttpDelete, Route("booking/{id:int}")]
        public async Task Delete(int id)
        {
            await _vacationService.DeleteAsync(id);
        }

        [HttpGet, Route("{userId:int}/{year:int}")]
        public async Task<int> GetVacationDay(int userId, int year)
        {
            return await _vacationService.GetVacationDay(userId, year);
        }

        [HttpGet, Route("get-booking/{userId:int}/{year:int}")]
        public async Task<double> GetBookingVacationDay(int userId, int year)
        {
            return await _vacationService.GetBookingVacationDay(userId, year);
        }

        [HttpGet, Route("check-new-user/{userId:int}")]
        public async Task<bool> CheckNewUser(int userId)
        {
            return await _vacationService.CheckNewUser(userId);
        }

        [HttpPost, Route("init-new-user")]
        public async Task<VacationDay> Create([FromBody]VacationDay vacationDay)
        {
            if (vacationDay == null || !ModelState.IsValid)
            {
                throw new CustomException(Error.INVALID_BOOKING, Error.INVALID_BOOKING_MSG);
            }

            vacationDay = await _vacationService.CreateAsync(vacationDay);

            return vacationDay;
        } 

        [HttpGet, Route("get-vacation-config")]
        public async Task<List<VacationConfig>> GetVacationConfig()
        {
            return await _vacationService.GetVacationConfigAsync();
        }

        [HttpGet, Route("get-vacation-config/{name}")]
        public async Task<VacationConfig> GetVacationConfig(string name)
        {
            return await _vacationService.GetVacationConfigAsync(name);
        }

        [HttpPut, Route("update-vacation-config")]
        public async Task<VacationConfig> UpdateVacationConfig([FromBody]VacationConfig vacationConfig)
        {
            if(vacationConfig == null || !ModelState.IsValid)
            {
                throw new CustomException(Error.INVALID_REQUEST, Error.INVALID_REQUEST_MSG);
            }

            return await _vacationService.UpdateVacationConfigAsync(vacationConfig);
        }

        [HttpPut, Route("vacation-config")]
        public async Task<VacationConfig> SetStatusVacationConfig([FromBody]VacationConfig vacationConfig)
        {
            if (vacationConfig == null || !ModelState.IsValid)
            {
                throw new CustomException(Error.INVALID_REQUEST, Error.INVALID_REQUEST_MSG);
            }

            var result = await _vacationService.SetStatusVacationConfigAsync(vacationConfig);

            return result;
        }
    }
}
