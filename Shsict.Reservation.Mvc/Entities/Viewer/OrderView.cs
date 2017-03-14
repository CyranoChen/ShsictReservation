using System;
using Shsict.Core;

namespace Shsict.Reservation.Mvc.Entities.Viewer
{
    [DbSchema("Reservation_OrderView", Key = "OrderID", Sort = "PlaceTime DESC")]
    public class OrderView : Core.Viewer
    {
        #region Members and Properties

        [DbColumn("OrderID", IsKey = true)]
        public int OrderID { get; set; }

        [DbColumn("StapleFood")]
        public StapleFoodEnum StapleFood { get; set; }

        [DbColumn("ExtraFood")]
        public bool ExtraFood { get; set; }

        [DbColumn("PlaceTime")]
        public DateTime PlaceTime { get; set; }

        [DbColumn("PlaceUser")]
        public string PlaceUser { get; set; }

        // Complex Object
        [DbColumn("u", Key = "UserGuid")]
        public User User { get; set; }

        [DbColumn("m", Key = "ID")]
        public Menu Menu { get; set; }

        [DbColumn("d", Key = "DeliveryGuid")]
        public Delivery Delivery { get; set; }

        #endregion
    }
}
