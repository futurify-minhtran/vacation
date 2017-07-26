using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationServer.Models;

namespace AuthenticationServer.ServicesInterfaces
{
    public interface IPermissionService
    {
        Task<List<Permission>> GetAllAsync();
        Task SyncPermissionsAsync(IEnumerable<Permission> permissions);
    }
}
