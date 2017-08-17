using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Common.Core.Models;
using AuthenticationServer.Models;
using AuthenticationServer.Models.BindingModels;

namespace AuthenticationServer.ServicesInterfaces
{
    public interface IAccountService
    {
        Task<Account> CheckExistByEmailAsync(string email);

        Task<Account> CheckAsync(string email, string password);

        Task<Account> CreateAsync(Account account);

        Task<Account> UpdateAccountAsync(AccountBindingModel accountBindingModel);

        Task<Account> UpdateUserAsync(UserBindingModel userBindingModel);

        Task<Account> UpdatePasswordAsync(int accountId, string password);

        Task ChangePasswordAsync(int accountId, string oldPassword, string newPassword);

        Task<Account> FindByEmailAsync(string email);

        Task<Account> FindByIdAsync(int id);

        IQueryable<Account> _queryAll(string filter = "");

        Task<int> CountAllAsync(string filter = "");

        Task<List<Account>> GetAllPagingAsync(int pageSize, int page, string sort, string sortType, string filter = "");

        Task DeleteAsync(int id);

        Task<IEnumerable<string>> GetPermissionsOfAccountAsync(int accountId);

        Task<List<Role>> GetRolesOfAccountAsync(int accountId);
        
        Task SetRoleAsync(int accountId, int roleId);

        Task RemoveRoleAsync(int accountId, int roleId);

        Task<RequestResetPassword> CreateRequestResetPassword(RequestResetPassword request);

        Task ResetPasswordAsync(ResetPassword model);

        Task<Account> SetStatusAccountAsync(Account account, bool status);

        Task SendMail(ConfigSendEmail configSendEmail, EmailTemplate mailTemplate,RequestResetPassword requestResetPassword);

        Task SendMailRegister(ConfigSendEmail configSendEmail, string emailRegister, string passwordRegister);

        Task UpdateRemainingDaysOffAsync(int userId, double remainingDaysOff);

    }
}
