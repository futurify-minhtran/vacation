﻿using System;
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
using Microsoft.Extensions.Options;
using App.Common.Core.Models;
using System.Security.Claims;
using App.Common.Core.Authentication;

namespace AuthenticationServer.Controllers
{
    [Route("api/account")]
    public class AccountController : Controller
    {
        //private readonly AuthDbContext _context;

        IAccountService _accountService;    
        private readonly EmailTemplate _emailTemplate;
        private readonly ConfigSendEmail _configSendEmail;

        public AccountController(IAccountService accountService, IOptions<EmailTemplate> emailTemplate, IOptions<ConfigSendEmail> configSendEmail)
        {
            _accountService = accountService;
            _emailTemplate = emailTemplate.Value;
            _configSendEmail = configSendEmail.Value;
        }

        [Authorize(Roles = "ADMIN")]
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

                var viewModel = account.ToAccountViewModel();

                // Send email
                await _accountService.SendMailRegister(_configSendEmail, account.Email, model.Password);

                //return viewModel;
                return Json(new { User = viewModel });
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message });
            }
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut, Route("update-account")]
        public async Task<ActionResult> UpdateAccount([FromBody]AccountBindingModel accountBindingModel)
        {
            try
            {
                if (accountBindingModel == null)
                {
                    throw new CustomException(Errors.INVALID_REQUEST, Errors.INVALID_REQUEST_MSG);
                }

                var updatedAccount = await _accountService.UpdateAccountAsync(accountBindingModel);

                return Json(new { User = updatedAccount.ToAccountViewModel() });
            }
            catch(Exception ex)
            {
                return Json(new { Error = ex.Message });
            }
        }

        [Authorize(Roles = "ADMIN, USER")]
        [HttpPut, Route("update-user")]
        public async Task<ActionResult> UpdateUser([FromBody]UserBindingModel userBindingModel)
        {
            try
            {
                if (userBindingModel == null)
                {
                    throw new CustomException(Errors.INVALID_REQUEST, Errors.INVALID_REQUEST_MSG);
                }

                var updatedUser = await _accountService.UpdateUserAsync(userBindingModel);

                return Json(new { User = updatedUser.ToUserViewModel() });
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message });
            }
        }

        [HttpGet, Route("{id:int}")]
        public async Task<AccountViewModel> Get(int id)
        {
            var account = await _accountService.FindByIdAsync(id);

            return account.ToAccountViewModel();
        }

        [HttpGet , Route("count-all")]
        public async Task<int> CountAll([FromQuery] string filter)
        {
            return await _accountService.CountAllAsync(filter);
        }

        [HttpGet, Route("get-all/paging/{pageSize:int}/{page:int}")]
        public async Task<List<AccountViewModel>> GellAllPaging(int pageSize, int page, [FromQuery] string filter, [FromQuery] string sort, [FromQuery] string sortType)
        {
            var accounts = await _accountService.GetAllPagingAsync(pageSize,page,sort,sortType,filter);
            return accounts.Select(a => a.ToAccountViewModel()).ToList();
        }

        [HttpGet, Route("me")]
        public async Task<AccountViewModel> MyAccount()
        {
            var id = User.GetAccountId();

            var account = await _accountService.FindByIdAsync(id.Value);

            return account.ToAccountViewModel();
        }

        [Authorize]
        [HttpPut, Route("me/password")]
        public async Task<ActionResult> ChangePassword([FromBody]ChangePasswordModel model)
        {
            try
            {
                if (model == null || !ModelState.IsValid)
                {
                    throw new CustomException(Errors.INVALID_REQUEST, Errors.INVALID_REQUEST_MSG);
                }

                await _accountService.ChangePasswordAsync(User.GetAccountId().Value, model.OldPassword, model.NewPassword);

                return Json(new { });
            }catch(Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet, Route("me/permissions")]
        public IEnumerable<string> MyPermissions()
        {
            return User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
        }

        [HttpGet, Route("is-me-authenticated")]
        public bool IsAuthenticated()
        {
            return User.Identity.IsAuthenticated;
        }

        [HttpDelete,Route("{id:int}")]
        public async Task Delete(int id)
        {
            await _accountService.DeleteAsync(id);
        }

        [HttpGet, Route("reset-password")]
        public async Task<ActionResult> RequestResetPasswordAsync([FromQuery]string email)
        {
            try
            {
                var account = await _accountService.CheckExistByEmailAsync(email);

                if (account == null)
                {
                    throw new CustomException(Errors.ACCOUNT_NOT_FOUND, Errors.ACCOUNT_NOT_FOUND_MSG);
                }

                var request = await _accountService.CreateRequestResetPassword(
                    new RequestResetPassword
                    {
                        Email = email,
                        ExpiredAt = DateTime.Now.AddDays(1)
                    }
                );

                // send email

                await _accountService.SendMail(_configSendEmail, _emailTemplate, request);

                return Json(new { });
            }catch(Exception ex)
            {
                return Json(new { error = ex.Message });
            }

            //await _rawRabbitBus.PublishAsync(new PushEmail { Title = _emailTemplate.Title, Body = string.Format(_emailTemplate.Body, account.UserName, request.Token), SendTo = email });
        }

        [HttpPut, Route("reset-password")]
        public async Task<ActionResult> ResetPasswordAsync([FromBody] ResetPassword model)
        {
            try
            {
                if (model == null)
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

                return Json(new { });
            }
            catch (Exception ex)
            {
                return Json(new { Error = ex.Message });
            }
        }

        [Authorize(Roles = "USER")]
        [HttpGet, Route("{id:int}/{status:bool}")]
        public async Task<AccountViewModel> SetStatusAccountAsync(int id, bool status)
        {
            var accountExisting = await _accountService.FindByIdAsync(id);

            if(accountExisting == null)
            {
                throw new CustomException(Errors.INVALID_REQUEST, Errors.INVALID_REQUEST_MSG);
            }

            var result = await _accountService.SetStatusAccountAsync(accountExisting, status);

            return result.ToAccountViewModel();
        }

        [Authorize]
        [HttpGet, Route("update-remaining-days-off/{userId:int}/{remainingDaysOff}")]
        public async Task UpdateRemainingDaysOff(int userId, double remainingDaysOff)
        {
            await _accountService.UpdateRemainingDaysOffAsync(userId,remainingDaysOff);
        }
    }
}