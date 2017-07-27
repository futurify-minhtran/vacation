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

            _context.Accounts.Add(account);

            await _context.SaveChangesAsync();

            return account;
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
                throw new ArgumentNullException("oldPassword");
            }

            if (String.IsNullOrEmpty(newPassword))
            {
                throw new ArgumentNullException("newPassword");
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

        public async Task<List<Account>> GetAllAsync()
        {
            return await _context.Accounts.ToListAsync();
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

        public async Task ResetPasswordByEmail(EmailResetPassword model)
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

        private static string GenerateSecurityStamp()
        {
            return Guid.NewGuid().ToString("D");
        }
    }
}
