using System;
using System.Collections.Generic;
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

        [DbColumn("IsApproved")]
        public bool IsApproved { get; set; }

        [DbColumn("IsActive")]
        public bool IsActive { get; set; }

        [DbColumn("Remark")]
        public string Remark { get; set; }

        #endregion

        public static class Cache
        {
            public static List<Menu> MenuList;
            public static List<Menu> MenuListActive;
            public static List<Menu> MenuListActiveToday;

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

                MenuList = repo.All<Menu>();
                MenuListActive = MenuList.FindAll(x => x.IsActive);
                MenuListActiveToday = MenuListActive.FindAll(x => x.MenuDate == DateTime.Today);
            }

            public static Menu Load(int id)
            {
                return MenuListActive.Find(x => x.ID.Equals(id));
            }
        }
    }

    public enum MenuTypeEnum
    {
        None = 0,
        Lunch = 1,
        Supper = 2
    }
}