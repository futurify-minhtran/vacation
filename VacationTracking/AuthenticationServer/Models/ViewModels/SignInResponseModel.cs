using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationServer.Models.ViewModels
{
    public class SignInResponseModel
    {
        public string AccessToken { get; set; }
        public DateTime Expires { get; set; }
        public AccountViewModel Account { get; set; }
    }
}
