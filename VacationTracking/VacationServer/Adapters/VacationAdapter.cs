using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationServer.Models;
using VacationServer.Models.BindingModels;
using VacationServer.Models.ViewModels;

namespace VacationServer.Adapters
{
    public static class VacationAdapter
    {
        public static Booking ToModel(this BookingBindingModel bookingBindingModel)
        {
            if(bookingBindingModel == null)
            {
                return null;
            }

            var bookingModel = new Booking()
            {
                Id = bookingBindingModel.Id,
                UserId = bookingBindingModel.UserId,
                StartDate = bookingBindingModel.StartDate,
                EndDate = bookingBindingModel.EndDate,
                BookingType = bookingBindingModel.BookingType,
                Reason = bookingBindingModel.Reason,
                AllDay = bookingBindingModel.AllDay
            };

            return bookingModel;
        }

        public static BookingViewModel ToViewMode(this Booking bookingModel)
        {
            if (bookingModel == null)
            {
                return null;
            }

            var bookingViewModel = new BookingViewModel()
            {
                Id = bookingModel.Id,
                UserId = bookingModel.UserId,
                StartDate = bookingModel.StartDate,
                EndDate = bookingModel.EndDate,
                BookingType = bookingModel.BookingType,
                CreatedAt = bookingModel.CreatedAt,
                ModifiedAt = bookingModel.ModifiedAt,
                Reason = bookingModel.Reason,
                AllDay = bookingModel.AllDay,
                TotalHours = bookingModel.TotalHours
            };

            return bookingViewModel;
        }

        public static Team ToModel(this TeamBindingModel teamBindingModel)
        {
            if(teamBindingModel == null)
            {
                return null;
            }

            var teamModel = new Team()
            {
                Id = teamBindingModel.Id,
                Name = teamBindingModel.Name,
                LeaderId = teamBindingModel.LeaderId,
            };

            return teamModel;
        }

        public static TeamViewModel ToViewModel (this Team teamModel)
        {
            if(teamModel == null)
            {
                return null;
            }

            var teamViewModel = new TeamViewModel()
            {
                Id = teamModel.Id,
                Name = teamModel.Name,
                LeaderId = teamModel.LeaderId,
                CreatedAt = teamModel.CreatedAt,
                ModifiedAt = teamModel.ModifiedAt
            };

            return teamViewModel;
        }
    }
}
