using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Common.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using VacationServer.Adapters;
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

        public async Task Create([FromBody]BookingBindingModel bookingBindingModel)
        {
            var bookingModel = bookingBindingModel.ToModel();

            if(bookingModel == null || !ModelState.IsValid)
            {
                throw new CustomException(Error.INVALID_BOOKING, Error.INVALID_BOOKING_MSG);
            }

            bookingModel = await _vacationService.CreateAsync(bookingModel);

            var bookingViewModel = bookingModel.ToViewMode();
            //return bookingViewModel;
        }
    }
}
