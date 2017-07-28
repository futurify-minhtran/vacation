using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthenticationServer.Models;
using AuthenticationServer.ServicesInterfaces;
using Microsoft.AspNetCore.Authorization;
using AuthenticationServer.Models.ViewModels;
using AuthenticationServer.Models.BindingModels;
using AuthenticationServer.Adapters;
using App.Common.Core.Exceptions;
using AuthenticationServer.Resources;
using AuthenticationServer.Setup;

namespace AuthenticationServer.Controllers
{
    [Route("api/Account")]
    public class AccountController : Controller
    {
        //private readonly AuthDbContext _context;

        IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [AllowAnonymous]
        [HttpPost, Route("register")]
        public async Task<ActionResult> Register([FromBody]RegisterModel model)
        {
            try
            {
                var account = model.ToModel();
                if (account == null || !ModelState.IsValid)
                {
                    throw new CustomException(Errors.INVALID_REGISTRATION_DATA, Errors.INVALID_REGISTRATION_DATA_MSG);
                }

                account.AccountPermissions = new List<AccountPermission>()
                {
                    new AccountPermission
                    {
                        PermissionId = PermissionsList.USER_PERMISSION
                    }
                };

                account = await _accountService.CreateAsync(account);

                // check
                //await _rawRabbitBus.PublishAsync(new AccountCreated { Id = account.Id, PhoneNumber = account.PhoneNumber, Status = account.Status, FirstName = model.FirstName, LastName = model.LastName, Email = model.Email });

                var viewModel = account.ToViewModel();
                //return viewModel;
                return Json(new { Error = false, User = viewModel });
            }
            catch (Exception ex)
            {
                return Json(new { Error = true, Message = ex.Message });
            }
        }
        
        [AllowAnonymous]
        [HttpPut,Route("{id:int}")]
        public async Task<Account> Update(int id, [FromBody]RegisterModel registerModel)
        {
            if(registerModel == null || !ModelState.IsValid)
            {
                throw new CustomException(Errors.INVALID_REQUEST, Errors.INVALID_REQUEST_MSG);
            }

            var model = registerModel.ToModel();

            var updatedUser = await _accountService.UpdateAsync(model);

            return updatedUser;
        }

        [AllowAnonymous]
        [HttpGet, Route("{id:int}")]
        public async Task<AccountViewModel> Get(int id)
        {
            var account = await _accountService.FindByIdAsync(id);

            return account.ToViewModel();
        }

        [AllowAnonymous]
        [HttpGet, Route("me")]
        public async Task<List<AccountViewModel>> GetAll()
        {
            var accounts = await _accountService.GetAllAsync();

            return accounts.Select(a => a.ToViewModel()).ToList();
        }

        [AllowAnonymous]
        [HttpDelete,Route("{id:int}")]
        public async Task Delete(int id)
        {
            await _accountService.DeleteAsync(id);
        }

        [HttpGet, Route("request-reset-password")]
            public async Task RequestResetPasswordAsync()
        {

        }

        [HttpPut, Route("reset-password")]
        public async Task ResetPasswordAsync([FromBody] ResetPassword model)
        {
            if(model == null)
            {
                throw new CustomException(Errors.REQUEST_NOT_NULL, Errors.REQUEST_NOT_NULL_MSG);
            }
            else if (String.IsNullOrEmpty(model.Email))
            {
                throw new CustomException(Errors.EMAIL_NOT_NULL, Errors.EMAIL_NOT_NULL_MSG);
            }
            else if (String.IsNullOrEmpty(model.Token))
            {
                throw new CustomException(Errors.TOKEN_NOT_NULL, Errors.TOKEN_NOT_NULL_MSG);
            }
            else if (String.IsNullOrEmpty(model.NewPassword))
            {
                throw new CustomException(Errors.PASSWORD_NOT_NULL, Errors.PASSWORD_NOT_NULL_MSG);
            }

            await _accountService.ResetPasswordAsync(model);
        }
    }
}