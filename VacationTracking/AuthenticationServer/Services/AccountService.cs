using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationServer.Models;
using AuthenticationServer.Models.BindingModels;
using AuthenticationServer.ServicesInterfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using App.Common.Core.Exceptions;
using AuthenticationServer.Resources;
using MailKit.Net.Smtp;
using MimeKit;
using App.Common.Core.Models;
using App.Common.Core;

namespace AuthenticationServer.Services
{
    public class AccountService : IAccountService
    {
        AuthDbContext _context;
        PasswordHasher<Account> _pwdHasher;

        public AccountService(AuthDbContext context)
        {
            _context = context;
            _pwdHasher = new PasswordHasher<Account>();
        }

        public async Task<Account> CheckExistByEmailAsync(string email)
        {
            return await _context.Accounts.FirstOrDefaultAsync(t => t.Email == email);
        }

        public async Task<Account> CheckAsync(string email, string password)
        {
            if (String.IsNullOrEmpty(email) || String.IsNullOrEmpty(password))
            {
                return null;
            }

            var account = await this.FindByEmailAsync(email);

            if(account == null)
            {
                return null;
            }

            if (String.IsNullOrEmpty(account.Password))
            {
                return null;
            }

            var hashedPassword = account.Password;

            var result = _pwdHasher.VerifyHashedPassword(account, hashedPassword, password);
            if(result == PasswordVerificationResult.Failed)
            {
                return null;
            }
            else if(result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                return await this.UpdatePasswordAsync(account.Id,password);
            }
            else
            {
                return account;
            }
        }

        public async Task<Account> CreateAsync(Account account)
        {
            if (account == null)
            {
                throw new ArgumentNullException("account");
            }

            if (String.IsNullOrEmpty(account.Email) || String.IsNullOrEmpty(account.Password))
            {
                throw new CustomException(Errors.INVALID_REGISTRATION_DATA, Errors.INVALID_REGISTRATION_DATA_MSG);
            }

            var existingUserEmail = await this.FindByEmailAsync(account.Email);

            if (existingUserEmail != null)
            {
                throw new CustomException(Errors.EMAIL_ALREADY_IN_USE, Errors.EMAIL_ALREADY_IN_USE_MSG);
            }

            if (!String.IsNullOrEmpty(account.Password))
            {
                var hashedPassword = _pwdHasher.HashPassword(account, account.Password);
                account.Password = hashedPassword;
            }

            account.SecurityStamp = AccountService.GenerateSecurityStamp();

            var now = DateTime.Now;

            account.CreatedAt = now;
            account.ModifiedAt = now;
            account.Status = true;

            _context.Accounts.Add(account);

            await _context.SaveChangesAsync();

            return account;
        }

        public async Task<Account> UpdateAccountAsync(AccountBindingModel accountBindingModel)
        {
            if (accountBindingModel == null)
            {
                throw new CustomException(Errors.INVALID_REQUEST, Errors.INVALID_REQUEST_MSG);
            }

            var existingAccount = await FindByIdAsync(accountBindingModel.Id);
            if(existingAccount == null)
            {
                throw new CustomException(Errors.ACCOUNT_NOT_FOUND, Errors.ACCOUNT_NOT_FOUND_MSG);
            }

            if(existingAccount.IsSystemAdmin != accountBindingModel.IsSystemAdmin)
            {
                var permissionAdmin = await this.GetAccountPermissionAsync(existingAccount.Id, "ADMIN");
                if(permissionAdmin == null)
                {
                    await this.CreateAccountPermission(existingAccount.Id, "ADMIN");
                } else
                {
                    await this.DeleteAccountPermission(existingAccount.Id, "ADMIN");
                }
            }

            existingAccount.IsSystemAdmin = accountBindingModel.IsSystemAdmin;
            existingAccount.FirstName = accountBindingModel.FirstName;
            existingAccount.LastName = accountBindingModel.LastName;
            existingAccount.Position = accountBindingModel.Position;
            existingAccount.Department = accountBindingModel.Department;
            existingAccount.Gender = accountBindingModel.Gender;
            existingAccount.PhoneNumber = accountBindingModel.PhoneNumber;
            existingAccount.DateOfBirth = accountBindingModel.DateOfBirth;
            existingAccount.Avatar = accountBindingModel.Avatar;
            existingAccount.RemainingDaysOff = accountBindingModel.RemainingDaysOff;
            existingAccount.Status = accountBindingModel.Status;
            existingAccount.ModifiedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return existingAccount;

        }

        public async Task<Account> UpdateUserAsync(UserBindingModel userBindingModel)
        {
            if (userBindingModel == null)
            {
                throw new CustomException(Errors.INVALID_REQUEST, Errors.INVALID_REQUEST_MSG);
            }

            var existingUser = await FindByIdAsync(userBindingModel.Id);
            if (existingUser == null)
            {
                throw new CustomException(Errors.ACCOUNT_NOT_FOUND, Errors.ACCOUNT_NOT_FOUND_MSG);
            }

            existingUser.FirstName = userBindingModel.FirstName;
            existingUser.LastName = userBindingModel.LastName;
            existingUser.Gender = userBindingModel.Gender;
            existingUser.PhoneNumber = userBindingModel.PhoneNumber;
            existingUser.DateOfBirth = userBindingModel.DateOfBirth;
            existingUser.Avatar = userBindingModel.Avatar;
            existingUser.ModifiedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return existingUser;

        }

        public async Task<Account> UpdatePasswordAsync(int accountId, string password)
        {
            if (String.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("password");
            }

            var account = await this.FindByIdAsync(accountId);

            if (account == null)
            {
                throw new CustomException(Errors.ACCOUNT_NOT_FOUND, Errors.ACCOUNT_NOT_FOUND_MSG);
            }

            var hashedPassword = _pwdHasher.HashPassword(account, password);

            account.Password = hashedPassword;

            account.SecurityStamp = AccountService.GenerateSecurityStamp();

            await _context.SaveChangesAsync();

            return account;
        }

        public async Task ChangePasswordAsync(int accountId, string oldPassword, string newPassword)
        {
            if (String.IsNullOrEmpty(oldPassword))
            {
                throw new CustomException(Errors.OLD_PASSWORD_NOT_NULL, Errors.OLD_PASSWORD_NOT_NULL_MSG);
            }

            if (String.IsNullOrEmpty(newPassword))
            {
                throw new CustomException(Errors.NEW_PASSWORD_NOT_NULL, Errors.NEW_PASSWORD_NOT_NULL_MSG);
            }

            var account = await this.FindByIdAsync(accountId);
            if(account == null)
            {
                throw new CustomException(Errors.ACCOUNT_NOT_FOUND, Errors.ACCOUNT_NOT_FOUND_MSG);
            }

            var checkedAccount = await this.CheckAsync(account.Email, oldPassword);
            if(checkedAccount == null)
            {
                throw new CustomException(Errors.OLD_PASSWORD_INCORRECT, Errors.OLD_PASSWORD_INCORRECT_MSG);
            }

            await this.UpdatePasswordAsync(accountId, newPassword);
        }

        public async Task<Account> FindByEmailAsync(string email)
        {
            if (String.IsNullOrEmpty(email))
            {
                return null;
            }

            return await _context.Accounts.FirstOrDefaultAsync(t => t.Email.ToLower() == email.ToLower());
        }

        public async Task<Account> FindByIdAsync(int id)
        {
            return await _context.Accounts.FirstOrDefaultAsync(t => t.Id == id);
        }

        public IQueryable<Account> _queryAll(string filter = "")
        {
            var q = from x in _context.Accounts
                    select x;

            // If have filter
            if (!String.IsNullOrEmpty(filter))
            {
                q = q.Where(a => a.Email.Contains(filter) || a.FirstName.Contains(filter) || a.LastName.Contains(filter));
            }

            return q;
        }

        public async Task<int> CountAllAsync(string filter = "")
        {
            return await this._queryAll(filter).CountAsync();
        }

        public async Task<List<Account>> GetAllPagingAsync(int pageSize, int page, string sort, string sortType, string filter = "")
        {
            var q = this._queryAll(filter);

            if (sortType == "desc")
            {
                return await q.OrderByDescending(sort).Skip(pageSize * (page - 1)).Take(pageSize).ToListAsync();
            }
            // else sortType == asc
            return await q.OrderBy(sort).Skip(pageSize * (page - 1)).Take(pageSize).ToListAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existingAccount = await this.FindByIdAsync(id);
            if(existingAccount == null)
            {
                throw new CustomException(Errors.ACCOUNT_NOT_FOUND, Errors.ACCOUNT_NOT_FOUND_MSG);
            }

            _context.Accounts.Remove(existingAccount);
            await _context.SaveChangesAsync();
        }

        // check
        public async Task<IEnumerable<string>> GetPermissionsOfAccountAsync(int accountId)
        {
            return await _context.AccountsRoles.Where(a => a.AccountId == accountId).SelectMany(a => a.Role.RolePermissions.Select(p => p.PermissionId))
                .Concat(_context.AccountsPermissions.Where(a => a.AccountId == accountId).Select(a => a.PermissionId)).Distinct().ToListAsync();
        }

        // check
        public async Task<List<Role>> GetRolesOfAccountAsync(int accountId)
        {
            return await _context.Roles.Where(r => r.AccountRoles.Any(a => a.AccountId == accountId)).ToListAsync();
        }

        public async Task SetRoleAsync(int accountId, int roleId)
        {
            var account = await this.FindByIdAsync(accountId);
            if (account == null)
            {
                throw new CustomException(Errors.ACCOUNT_NOT_FOUND, Errors.ACCOUNT_NOT_FOUND_MSG);
            }

            if(!_context.Roles.Any(t => t.Id == roleId))
            {
                throw new CustomException(Errors.ROLE_NOT_FOUND, Errors.ROLE_NOT_FOUND_MSG);
            }

            if(!_context.AccountsRoles.Any(t => t.AccountId == accountId && t.RoleId == roleId))
            {
                _context.AccountsRoles.Add(new AccountRole
                {
                    AccountId = accountId,
                    RoleId = roleId
                });

                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveRoleAsync(int accountId, int roleId)
        {
            var accountRole = await _context.AccountsRoles.FirstOrDefaultAsync(t => t.AccountId == accountId && t.RoleId == roleId);

            if(accountRole != null)
            {
                _context.AccountsRoles.Remove(accountRole);

                await _context.SaveChangesAsync();
            }
        }

        public async Task<RequestResetPassword> CreateRequestResetPassword(RequestResetPassword request)
        {
            var existingAccount = await _context.RequestResetPasswords.FirstOrDefaultAsync(t => t.Email == request.Email && t.ExpiredAt > DateTime.Now);
            if(existingAccount != null)
            {
                return existingAccount;
            }

            request.Token = GenerateSecurityStamp();
            _context.RequestResetPasswords.Add(request);
            await _context.SaveChangesAsync();

            return request;
        }

        public async Task ResetPasswordAsync(ResetPassword model)
        {
            var account = await this.CheckExistByEmailAsync(model.Email);
            if(account == null)
            {
                throw new CustomException(Errors.ACCOUNT_NOT_FOUND, Errors.ACCOUNT_NOT_FOUND_MSG);
            }

            var request = await _context.RequestResetPasswords.FirstOrDefaultAsync(t => t.Email == model.Email && t.Token == model.Token && t.ExpiredAt > DateTime.Now);
            if (request == null)
            {
                throw new CustomException(Errors.TOKEN_IS_EXPIRED, Errors.TOKEN_IS_EXPIRED_MSG);
            }

            account.Password = _pwdHasher.HashPassword(account, model.NewPassword);
            account.SecurityStamp = GenerateSecurityStamp();
            account.ModifiedAt = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        public async Task<Account> SetStatusAccountAsync(Account account, bool status)
        {
            if (account == null)
            {
                throw new ArgumentNullException("account");
            }

            var existingAccount = await FindByIdAsync(account.Id);
            if (existingAccount == null)
            {
                throw new CustomException(Errors.ACCOUNT_NOT_FOUND, Errors.ACCOUNT_NOT_FOUND_MSG);
            }

            existingAccount.Status = status;

            await _context.SaveChangesAsync();

            return existingAccount;
        }

        public async Task SendMail(ConfigSendEmail configSendEmail, EmailTemplate mailTemplate, RequestResetPassword requestResetPassword)
        {
            var sender = configSendEmail.Sender;
            var username = configSendEmail.Username;
            var password = configSendEmail.Password;
            var host = configSendEmail.Host;
            var port = configSendEmail.Port.ConvertToInt();

            var receiver = requestResetPassword.Email;

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Vacation Tracking", sender));
            emailMessage.To.Add(new MailboxAddress("", receiver));
            emailMessage.Subject = mailTemplate.Subject;
            emailMessage.Body = new TextPart("html")
            {
                Text = string.Format(mailTemplate.Body, requestResetPassword.Email, requestResetPassword.Token)
            };
            
            using (var client = new SmtpClient())
            {
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await client.ConnectAsync(host, port, false);
                client.AuthenticationMechanisms.Remove("XOAUTH2"); // Must be removed for Gmail SMTP
                await client.AuthenticateAsync(username, password);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }

        }

        public async Task SendMailRegister(ConfigSendEmail configSendEmail, string emailRegister, string passwordRegister)
        {
            var sender = configSendEmail.Sender;
            var username = configSendEmail.Username;
            var password = configSendEmail.Password;
            var host = configSendEmail.Host;
            var port = configSendEmail.Port.ConvertToInt();

            var receiver = emailRegister;

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Vacation Tracking", sender));
            emailMessage.To.Add(new MailboxAddress("", receiver));

            emailMessage.Subject = "Vacation Tracking";
            var mailTemplate = "<p>Dear {0}!</p><p>Your account have been created!</p><p>Your default password: <b><i>{1}</i></b></p>";

            emailMessage.Body = new TextPart("html")
            {
                Text = string.Format(mailTemplate, emailRegister, passwordRegister)
            };

            using (var client = new SmtpClient())
            {
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await client.ConnectAsync(host, port, false);
                client.AuthenticationMechanisms.Remove("XOAUTH2"); // Must be removed for Gmail SMTP
                await client.AuthenticateAsync(username, password);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }

        }

        public async Task UpdateRemainingDaysOffAsync(int userId, double remainingDaysOff)
        {
            var existingAccount = await this.FindByIdAsync(userId);

            if(existingAccount == null)
            {
                throw new CustomException(Errors.ACCOUNT_NOT_FOUND, Errors.ACCOUNT_NOT_FOUND_MSG);
            }

            existingAccount.RemainingDaysOff = remainingDaysOff;
            existingAccount.ModifiedAt = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        public async Task<AccountPermission> GetAccountPermissionAsync(int userId, string permissionId)
        {
            return await _context.AccountsPermissions.FirstOrDefaultAsync(ap => ap.AccountId == userId && ap.PermissionId == permissionId);
        }

        public async Task<AccountPermission> CreateAccountPermission(int userId, string permissionId)
        {
            if (userId == 0 || string.IsNullOrEmpty(permissionId))
            {
                throw new CustomException(Errors.ACCOUNT_PERMISSION_NOT_NULL, Errors.ACCOUNT_PERMISSION_NOT_NULL_MSG);
            }

            var existing = await this.GetAccountPermissionAsync(userId, permissionId);
            if (existing != null)
            {
                throw new CustomException(Errors.ACCOUNT_PERMISSION_EXIST, Errors.ACCOUNT_PERMISSION_EXIST_MSG);
            }

            var accountPermission = new AccountPermission()
            {
                AccountId = userId,
                PermissionId = permissionId
            };

            _context.Add(accountPermission);
            await _context.SaveChangesAsync();

            return accountPermission;
        }

        public async Task DeleteAccountPermission(int userId, string permissionId)
        {
            if(userId == 0 || string.IsNullOrEmpty(permissionId))
            {
                throw new CustomException(Errors.ACCOUNT_PERMISSION_NOT_NULL, Errors.ACCOUNT_PERMISSION_NOT_NULL_MSG);
            }

            var existing = await this.GetAccountPermissionAsync(userId, permissionId);
            if(existing == null)
            {
                throw new CustomException(Errors.ACCOUNT_PERMISSION_NOT_FOUND, Errors.ACCOUNT_PERMISSION_NOT_FOUND_MSG);
            }

            _context.Remove(existing);
            await _context.SaveChangesAsync();
        }

        public static string GenerateSecurityStamp()
        {
            return Guid.NewGuid().ToString("D");
        }
    }
}
