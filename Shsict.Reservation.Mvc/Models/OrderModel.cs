using System;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Shsict.Core;
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
                    DeliveryZone = s.Delivery != null ?
                        Delivery.Cache.GetParentZone(s.Delivery.ID).DeliveryName : string.Empty,
                    DeliveryPointGuid = s.Delivery?.ID,
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

        [Display(Name = "用餐人")]
        public string UserName { get; set; }

        public string EmployeeNo { get; set; }

        public int MenuID { get; set; }

        [Required(ErrorMessage = "请填写{0}")]
        [DataType(DataType.DateTime, ErrorMessage = "请正确填写{0}")]
        [Display(Name = "日期")]
        public DateTime MenuDate { get; set; }

        public MenuTypeEnum MenuType { get; set; }

        [Required(ErrorMessage = "请填写{0}")]
        [Domain("Lunch", "Supper")]
        [Display(Name = "时段")]
        public string MenuName { get; set; }

        [Required(ErrorMessage = "请填写{0}")]
        [Domain("A", "B")]
        [Display(Name = "类型")]
        public string Flag { get; set; }

        public string Meat { get; set; }

        public string MeatSmall { get; set; }

        public string Vegetable1 { get; set; }

        public string Vegetable2 { get; set; }

        [Required(ErrorMessage = "请选择{0}")]
        [Display(Name = "送餐区域")]
        public string DeliveryZone { get; set; }

        public Guid? DeliveryPointGuid { get; set; }

        [Required(ErrorMessage = "请选择{0}")]
        [Display(Name = "送餐点")]
        public string DeliveryPoint { get; set; }

        [Required(ErrorMessage = "请选择{0}")]
        [Domain("Rice", "Bun")]
        [Display(Name = "主食")]
        public string StapleFood { get; set; }

        [Display(Name = "加饭")]
        public bool ExtraFood { get; set; }

        public DateTime CreateTime { get; set; }

        public string CreateUser { get; set; }

        public bool CanCancelNow { get; set; }

        #endregion
    }
}