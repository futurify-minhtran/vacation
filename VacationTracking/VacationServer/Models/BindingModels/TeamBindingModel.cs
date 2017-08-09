using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VacationServer.Models.BindingModels
{
    public class TeamBindingModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int LeaderId { get; set; }
    }
}
