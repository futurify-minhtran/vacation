﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationServer.Models;
using AuthenticationServer.Models.BindingModels;

namespace AuthenticationServer.ServicesInterfaces
{
    public interface IAccountService
    {
        Task<Account> CheckExistByEmailAsync(string email);

        Task<Account> CheckAsync(string email, string password);

        Task<Account> CreateAsync(Account account);
        Task<Account> UpdatePasswordAsync(int accountId, string password);
        Task ChangePasswordAsync(int accountId, string oldPassword, string newPassword);

        Task<Account> FindByEmailAsync(string email);

        Task<Account> FindByIdAsync(int id);

        Task<IEnumerable<string>> GetPermissionsOfAccountAsync(int accountId);

        Task<List<Role>> GetRolesOfAccountAsync(int accountId);
        
        Task SetRoleAsync(int accountId, int roleId);

        Task RemoveRoleAsync(int accountId, int roleId);

        Task<RequestResetPassword> CreateRequestResetPassword(RequestResetPassword request);

        Task ResetPasswordByEmail(EmailResetPassword model);
    }
}
