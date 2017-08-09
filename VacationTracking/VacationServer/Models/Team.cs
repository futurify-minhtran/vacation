using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VacationServer.Models
{
    public class Team
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int LeaderId { get; set; }

        public virtual List<TeamMember> TeamMembers {get;set;}

        public DateTime CreatedAt { get; set; }

        public DateTime ModifiedAt { get; set; }
    }

    public class TeamMember
    {
        public int TeamId { get; set; }

        public virtual Team Team { get; set; }

        public int MemberId { get; set; }
    }
}
