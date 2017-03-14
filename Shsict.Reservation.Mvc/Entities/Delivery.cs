using System;
using Shsict.Core;

namespace Shsict.Reservation.Mvc.Entities
{
    [DbSchema("Reservation_Delivery", Key = "DeliveryGuid", Sort = "PointName")]
    public class Delivery : Entity<Guid>
    {
        #region Members and Properties

        public override Guid ID
        {
            get
            {
                if (DeliveryGuid.Equals(Guid.Empty))
                {
                    DeliveryGuid = Guid.NewGuid();
                }

                return DeliveryGuid;
            }
            set { DeliveryGuid = value; }
        }

        private Guid DeliveryGuid { get; set; }

        [DbColumn("PointName")]
        public string PointName { get; set; }

        [DbColumn("PointDetail")]
        public string PointDetail { get; set; }

        #endregion
    }
}