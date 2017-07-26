
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationServer.Models;

namespace AuthenticationServer.ServicesInterfaces
{
    public interface IRoleService
    {
        Task<Role> FindByIdAsync(int id);
    }
}
