using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationServer.Models
{
    public class Permission
    {
        public string Id { get; set; }
        public string Display { get; set; }
        public virtual List<RolePermission> RolePermissions { get; set; }
        public Permission() { }

        public Permission(string id, string display)
        {
            this.Id = id;
            this.Display = display;
        }
    }
}
