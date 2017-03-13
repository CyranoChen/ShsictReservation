using System;
using Shsict.Core;

namespace Shsict.Reservation.Mvc.Entities
{
    [DbSchema("Reservation_Menu", Sort = "MenuDate DESC, MenuType DESC, MenuFlag")]
    public class Menu : Entity<int>
    {
        #region Members and Properties

        [DbColumn("MenuDate")]
        public DateTime MenuDate { get; set; }

        [DbColumn("MenuType")]
        public MenuTypeEnum MenuType { get; set; }

        [DbColumn("MenuFlag")]
        public string MenuFlag { get; set; }

        [DbColumn("Meat")]
        public string Meat { get; set; }

        [DbColumn("MeatSmall")]
        public string MeatSmall { get; set; }

        [DbColumn("Vegetable1")]
        public string Vegetable1 { get; set; }

        [DbColumn("Vegetable2")]
        public string Vegetable2 { get; set; }

        [DbColumn("CreateTime")]
        public DateTime CreateTime { get; set; }

        [DbColumn("CreateUser")]
        public string CreateUser { get; set; }

        [DbColumn("IsActive")]
        public bool IsActive { get; set; }

        [DbColumn("Remark")]
        public string Remark { get; set; }

        #endregion
    }

    public enum MenuTypeEnum
    {
        None = 0,
        Lunch = 1,
        Supper = 2
    }
}