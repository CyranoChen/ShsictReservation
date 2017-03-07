using System;
using Shsict.Core;

namespace Shsict.Reservation.Mvc.Entities
{
    [DbSchema("Reservation_Delivery", Key = "DeliveryGuid", Sort = "PointName")]
    public class Delivery : Entity<Guid>
    {
        #region Members and Properties

        [DbColumn("PointName")]
        public string PointName { get; set; }

        [DbColumn("PointDetail")]
        public string PointDetail { get; set; }

        #endregion
    }
}