using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationServer.Models
{
    public class Role
    {
        [Key]
        public  int Id { get; set; }

        public string Name { get; set; }

        public virtual List<RolePermission> RolePermissions { get; set; }

        public virtual List<AccountRole> AccountRoles { get; set; }
    }

    public class AccountRole {
        public int AccountId { get; set; }
        public virtual Account Account { get; set; }
        public int RoleId { get; set; }
        public virtual Role Role { get; set; }
    }

    public class RolePermission
    {
        public int RoleId { get; set; }
        public virtual Role Role { get; set; }
        public string PermissionId { get; set; }
        public virtual Permission Permission { get; set; }
    }
}
