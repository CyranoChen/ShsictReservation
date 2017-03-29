using System;
using AutoMapper;
using Shsict.Reservation.Mvc.Entities;
using Shsict.Reservation.Mvc.Entities.Viewer;

namespace Shsict.Reservation.Mvc.Models
{
    public class OrderDto
    {
        public static MapperConfiguration ConfigMapper()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<OrderView, OrderDto>()
                .ConstructUsing(s => new OrderDto
                {
                    ID = s.OrderID,
                    UserGuid = s.User.ID,
                    UserName = s.User.EmployeeName,
                    EmployeeNo = s.User.EmployeeNo,
                    MenuID = s.Menu.ID,
                    MenuDate = s.Menu.MenuDate,
                    MenuType = s.Menu.MenuType,
                    Flag = $" {s.Menu.MenuFlag} 套餐",
                    Meat = s.Menu.Meat,
                    MeatSmall = s.Menu.MeatSmall,
                    Vegetable1 = s.Menu.Vegetable1,
                    Vegetable2 = s.Menu.Vegetable2,
                    DeliveryZone = s.Delivery?.ParentID != null ?
                        Delivery.Cache.GetParentZone(s.Delivery.ParentID.Value).DeliveryName : null,
                    DeliveryPoint = s.Delivery?.DeliveryName,
                    ExtraFood = s.ExtraFood,
                    CreateTime = s.PlaceTime,
                    CreateUser = s.PlaceUser
                })
                .ForMember(d => d.MenuName, opt => opt.ResolveUsing(s =>
                {
                    switch (s.Menu.MenuType)
                    {
                        case MenuTypeEnum.Lunch:
                            return "午餐";
                        case MenuTypeEnum.Supper:
                            return "夜宵";
                        default:
                            return string.Empty;
                    }
                }))
                .ForMember(d => d.StapleFood, opt =>
                    opt.MapFrom(s => s.StapleFood == StapleFoodEnum.Rice ? "米饭" : "馒头"))
                );

            return config;
        }

        public override string ToString()
        {
            var extra = ExtraFood ? "(加饭)" : string.Empty;
            return $@"{MenuDate.ToString("2017年3月29日")}，{MenuName}{Flag}，
                            主食：{StapleFood}{extra}，送餐点：{DeliveryZone}-{DeliveryPoint}";
        }

        #region Members and Properties

        public int ID { get; set; }

        public Guid UserGuid { get; set; }

        public string UserName { get; set; }

        public string EmployeeNo { get; set; }

        public int MenuID { get; set; }

        public DateTime MenuDate { get; set; }

        public MenuTypeEnum MenuType { get; set; }

        public string MenuName { get; set; }

        public string Flag { get; set; }

        public string Meat { get; set; }

        public string MeatSmall { get; set; }

        public string Vegetable1 { get; set; }

        public string Vegetable2 { get; set; }

        public string DeliveryZone { get; set; }

        public string DeliveryPoint { get; set; }

        public string StapleFood { get; set; }

        public bool ExtraFood { get; set; }

        public DateTime CreateTime { get; set; }

        public string CreateUser { get; set; }

        public bool CanCancelNow { get; set; }

        #endregion
    }
}