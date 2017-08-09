using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VacationServer.Models.ViewModels
{
    public class TeamViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int LeaderId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ModifiedAt { get; set; }
    }
}
