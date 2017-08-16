using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationServer.Models.BindingModels
{
    public class AccountBindingModel
    {
        public int Id { get; set; }

        public bool IsSystemAdmin { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Position Position { get; set; }

        public Department Department { get; set; }

        public Gender Gender { get; set; }

        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Avatar { get; set; }

        public int RemainingDaysOff { get; set; }

        public bool Status { get; set; }
    }
}
