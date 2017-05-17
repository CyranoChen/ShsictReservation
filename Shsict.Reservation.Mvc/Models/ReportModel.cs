using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Shsict.Core;
using Shsict.Reservation.Mvc.Entities;
using Shsict.Reservation.Mvc.Entities.Viewer;

namespace Shsict.Reservation.Mvc.Models
{
    public class ReportDto
    {
        public static MapperConfiguration ConfigMapper()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<ReportView, ReportDto>()
                .ConstructUsing(s => new ReportDto
                {
                    DeliveryZone = s.DeliveryName,
                    MenuFlag = s.Menu.MenuFlag,
                    ExtraFood = s.ExtraFood,
                    OrderCount = s.OrderCount
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

        #region Members and Properties
        [Display(Name = "送餐区域")]
        public string DeliveryZone { get; set; }

        [Domain("午餐", "夜宵")]
        [Display(Name = "时段")]
        public string MenuName { get; set; }

        [Domain("A", "B")]
        [Display(Name = "套餐")]
        public string MenuFlag { get; set; }

        [Domain("米饭", "馒头")]
        [Display(Name = "主食")]
        public string StapleFood { get; set; }

        [Display(Name = "加饭")]
        public bool ExtraFood { get; set; }

        [Display(Name = "数量")]
        public int OrderCount { get; set; }

        #endregion
    }
}