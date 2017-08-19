using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationServer.Models;

namespace AuthenticationServer.Setup
{
    public static class PermissionsList
    {
        public const string ADMIN_PERMISSION = "ADMIN";
        public const string USER_PERMISSION = "USER";

        static PermissionsList()
        {
            GroupsPermissions = new PermissionsGroup[]
           {
                new PermissionsGroup
                {
                    Name = "Manage Accounts",
                    Permissions =
                        new Permission[]
                        {
                            new Permission("VIEW_ACCOUNTS", "View accounts"),
                            new Permission("CREATE_ACCOUNT", "Create account"),
                            new Permission("EDIT_ACCOUNT", "Edit account"),
                            new Permission("ADMIN_EDIT_ACCOUNT", "Admin can edit account"),
                            new Permission("ADMIN_REMOVE_AVATAR", "Admin can delete avatar"),
                            new Permission("ADMIN_REMOVE_VIDEO", "Admin can delete video"),
                            new Permission("DELETE_ACCOUNT", "Delete account")
                        }
                },
                
                new PermissionsGroup
                {
                    Name = "Admin",
                    Permissions =
                        new Permission[]
                        {
                            new Permission(ADMIN_PERMISSION, "Admin")
                        }
                },
                new PermissionsGroup
                {
                    Name = "User",
                    Permissions =
                        new Permission[]
                        {
                            new Permission(USER_PERMISSION, "User")
                        }
                }
           };
        }

        public static PermissionsGroup[] GroupsPermissions { get; }
    }
}
