using System;
using Shsict.Core;
using Shsict.Reservation.Mvc.Entities;

namespace Shsict.Reservation.Mvc.Models
{
    public class MenuDto
    {
        #region Members and Properties

        public DateTime MenuDate { get; set; }

        public string Duration { get; set; }

        public string Name { get; set; }

        public string Flag { get; set; }

        public string Meat { get; set; }

        public string MeatSmall { get; set; }

        public string Vegetable1 { get; set; }

        public string Vegetable2 { get; set; }

        #endregion
    }
}