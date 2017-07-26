using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationServer.Models;
using AuthenticationServer.ServicesInterfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationServer.Services
{
    public class PermissionService: IPermissionService
    {
        AuthDbContext _context;

        public PermissionService(AuthDbContext context)
        {
            _context = context;
        }

        public async Task<List<Permission>> GetAllAsync()
        {
            return await _context.Permissions.ToListAsync();
        }

        public async Task SyncPermissionsAsync(IEnumerable<Permission> permissions)
        {
            var allPermissions = await this.GetAllAsync();

            var syncCodes = permissions.Select(t => t.Id).ToList();

            var updatePermissions = allPermissions.Where(p => syncCodes.Contains(p.Id));
            var deletePermissions = allPermissions.Except(updatePermissions).ToList();
            var addPermission = permissions.Where(p => !updatePermissions.Any(u => u.Id == p.Id)).ToList();

            foreach(var permission in updatePermissions)
            {
                permission.Display = permissions.First(p => p.Id == permission.Id).Display;
            }

            if (deletePermissions.Any())
            {
                _context.RolePermissions.RemoveRange(_context.RolePermissions.Where(r => deletePermissions.Any(d => d.Id == r.PermissionId)));
                _context.Permissions.RemoveRange(deletePermissions);
            }

            if (addPermission.Any()) {
                _context.Permissions.AddRange(addPermission);
            }

            await _context.SaveChangesAsync();
        }
    }
}
