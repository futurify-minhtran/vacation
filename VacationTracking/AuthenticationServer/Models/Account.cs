using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationServer.Models
{
    public class Account
    {
        [Key]
        public int Id { get; set; }

        public bool IsSystemAdmin { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Position Position { get; set; }

        public Gender Gender { get; set; }

        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Avatar { get; set; }

        public int RemainingDaysOff { get; set; }

        [Required]
        public string SecurityStamp { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public virtual List<AccountRole> AccountRoles { get; set; }

        public virtual List<AccountPermission> AccountPermissions { get; set; }

        public bool Status { get; set; }
    }

    public enum Position { Staff }

    public enum Gender { Undefine, Male, Female }
}
