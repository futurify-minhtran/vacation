using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationServer.Models;
using AuthenticationServer.Models.BindingModels;
using AuthenticationServer.Models.ViewModels;

namespace AuthenticationServer.Adapters
{
    public static class AccountAdapter
    {
        public static Account ToModel(this RegisterModel model)
        {
            if (model == null)
            {
                return null;
            }

            var account = new Account
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Position = model.Position,
                Gender = model.Gender,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                Password = model.Password
            };

            return account;
        }

        public static AccountViewModel ToViewModel(this Account model)
        {
            if (model == null)
            {
                return null;
            }

            var viewModel = new AccountViewModel
            {
                Id = model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Position = model.Position,
                Gender = model.Gender,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                DateOfBirth = model.DateOfBirth,
                Avatar = model.Avatar,
                RemainingDaysOff = model.RemainingDaysOff,
                CreatedAt = model.CreatedAt,
                ModifiedAt = model.ModifiedAt
            };

            return viewModel;
        }
    }
}
