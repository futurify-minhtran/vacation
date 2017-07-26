using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthenticationServer.Models;

namespace AuthenticationServer.Setup
{
    public class PermissionsGroup
    {
        public string Name { get; set; }
        public Permission[] Permissions { get; set; }
    }
}
