using System;
using System.Collections.Generic;
using Shsict.Core;

namespace Shsict.Reservation.Mvc.Entities
{
    [DbSchema("Reservation_Delivery", Key = "DeliveryGuid", Sort = "DeliveryName")]
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

        [DbColumn("DeliveryName")]
        public string DeliveryName { get; set; }

        [DbColumn("PointDetail")]
        public string PointDetail { get; set; }

        [DbColumn("ParentID")]
        public Guid? ParentID { get; set; }

        [DbColumn("IsActive")]
        public bool IsActive { get; set; }

        [DbColumn("Remark")]
        public string Remark { get; set; }

        #endregion

        public static class Cache
        {
            public static List<Delivery> DeliveryList;
            public static List<Delivery> DeliveryListActive;
            public static List<Delivery> DeliveryZoneList;

            static Cache()
            {
                InitCache();
            }

            public static void RefreshCache()
            {
                InitCache();
            }

            private static void InitCache()
            {
                IRepository repo = new Repository();

                DeliveryList = repo.All<Delivery>();
                DeliveryListActive = DeliveryList.FindAll(x => x.IsActive);
                DeliveryZoneList = DeliveryList.FindAll(x => !x.ParentID.HasValue);
            }

            public static Delivery Load(Guid guid)
            {
                return DeliveryListActive.Find(x => x.ID.Equals(guid));
            }

            public static Delivery GetParentZone(Guid parentId)
            {
                return DeliveryZoneList.Find(x => x.ID == parentId);
            }

            public static List<Delivery> GetDeliveryPointByZone(Guid guid)
            {
                return DeliveryListActive.FindAll(x => x.ParentID.HasValue && x.ParentID.Value == guid);
            }
        }

    }
}