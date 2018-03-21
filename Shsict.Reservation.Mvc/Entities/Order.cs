using System;
using Shsict.Core;
using Shsict.Core.Extension;

namespace Shsict.Reservation.Mvc.Entities
{
    [DbSchema("Reservation_Order", Sort = "CreateTime DESC")]
    public class Order : Entity<int>
    {
        #region Members and Properties

        [DbColumn("UserGuid")]
        public Guid UserGuid { get; set; }

        [DbColumn("UserName")]
        public string UserName { get; set; }

        [DbColumn("EmployeeNo")]
        public string EmployeeNo { get; set; }

        [DbColumn("MenuID")]
        public int MenuID { get; set; }

        [DbColumn("DeliveryGuid")]
        public Guid DeliveryGuid { get; set; }

        [DbColumn("StapleFood")]
        public StapleFoodEnum StapleFood { get; set; }

        [DbColumn("ExtraFood")]
        public bool ExtraFood { get; set; }

        [DbColumn("CreateTime")]
        public DateTime CreateTime { get; set; }

        [DbColumn("CreateUser")]
        public string CreateUser { get; set; }

        [DbColumn("IsActive")]
        public bool IsActive { get; set; }

        [DbColumn("Remark")]
        public string Remark { get; set; }

        #endregion

        public static void Clean(int dateDiff)
        {
            IRepository repo = new Repository();

            var criteria = new Criteria
            {
                WhereClause = $"CreateTime < '{DateTime.Today.AddDays(dateDiff)}' AND IsActive = 0",
                OrderClause = "CreateTime DESC",
                PagingSize = 0
            };

            var list = repo.Query<Order>(criteria);

            if (list != null && list.Count > 0)
            {
                list.Delete();
            }
        }
    }

    public enum StapleFoodEnum
    {
        Rice = 1,
        Bun = 2
    }
}