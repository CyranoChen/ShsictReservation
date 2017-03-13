
using System;

namespace Shsict.Reservation.Mvc.Models
{
    public class ReservationModels
    {
        public class IndexDto
        {
            public DateTime MenuDate { get; set; }

            public MenuDto MenuA { get; set; }

            public MenuDto MenuB { get; set; }
        }
    }
}