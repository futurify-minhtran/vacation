using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationServer.Models.BindingModels
{
    public class PhoneNumberResetPassword
    {
        public string PhoneNumber { get; set; }
        public string PIN { get; set; }
        public string NewPassword { get; set; }
    }

    public class EmailResetPassword
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }

    public class EmailTemplate
    {
        public string Title { get; set; }
        public string Body { get; set; }
    }
}
