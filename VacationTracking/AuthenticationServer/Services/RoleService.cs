using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationServer.Models;
using AuthenticationServer.ServicesInterfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationServer.Services
{
    public class RoleService : IRoleService
    {
        AuthDbContext _context;

        public RoleService(AuthDbContext context)
        {
            _context = context;
        }

        public async Task<Role> FindByIdAsync(int id)
        {
            return await _context.Roles.FirstOrDefaultAsync(t => t.Id == id);
        }
    }
}
