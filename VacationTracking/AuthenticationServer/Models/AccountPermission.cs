using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationServer.Models
{
    public class AccountPermission
    {
        public int AccountId { get; set; }
        public virtual Account Account { get; set; }
        public string PermissionId { get; set; }
        public virtual Permission Permission { get; set; }
    }
}
