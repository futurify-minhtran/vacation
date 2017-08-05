using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VacationServer.Models.BindingModels
{
    public class BookingBindingModel
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public BookingType BookingType { get; set; }
    }
}
