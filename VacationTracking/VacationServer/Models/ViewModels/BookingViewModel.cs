using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VacationServer.Models.ViewModels
{
    public class BookingViewModel
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public BookingType BookingType { get; set; }

        public string Reason { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ModifiedAt { get; set; }
    }
}
