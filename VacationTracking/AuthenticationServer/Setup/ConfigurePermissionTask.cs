using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationServer.ServicesInterfaces;
using Microsoft.AspNetCore.Builder;

namespace AuthenticationServer.Setup
{
    public static class ConfigurePermissionTask
    {
        public static void ConfigurePermissions(this IApplicationBuilder app)
        {
            var permissionService = (IPermissionService)app.ApplicationServices.GetService(typeof(IPermissionService));

            permissionService.SyncPermissionsAsync(PermissionsList.GroupsPermissions.SelectMany(p => p.Permissions)).Wait();
        }

    }
}
