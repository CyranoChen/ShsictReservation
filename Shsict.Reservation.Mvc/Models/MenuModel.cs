using System;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Shsict.Core;
using Shsict.Reservation.Mvc.Entities;

namespace Shsict.Reservation.Mvc.Models
{
    public class MenuDto
    {
        public static MapperConfiguration ConfigMapper()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Menu, MenuDto>()
                .ForMember(d => d.Name, opt => opt.ResolveUsing(s =>
                {
                    switch (s.MenuType)
                    {
                        case MenuTypeEnum.Lunch:
                            return "午餐";
                        case MenuTypeEnum.Supper:
                            return "夜宵";
                        default:
                            return string.Empty;
                    }
                }))
                .ForMember(d => d.Duration, opt => opt.ResolveUsing(s =>
                {
                    // TODO 从配置中取值
                    //var duration = ConfigGlobal.MenuDuration;

                    switch (s.MenuType)
                    {
                        case MenuTypeEnum.Lunch:
                            return $"7:00~9:00";
                        case MenuTypeEnum.Supper:
                            return $"18:00~20:00";
                        default:
                            return string.Empty;
                    }
                }))
                .ForMember(d => d.Flag, opt => opt.MapFrom(s => s.MenuFlag))
                );

            return config;
        }


        #region Members and Properties

        public int ID { get; set; }

        [Required(ErrorMessage = "请填写{0}")]
        [DataType(DataType.DateTime, ErrorMessage = "请正确填写{0}")]
        [Display(Name = "日期")]
        public DateTime? MenuDate { get; set; }

        public string Duration { get; set; }

        [Required(ErrorMessage = "请填写{0}")]
        [Domain("Lunch", "Supper")]
        [Display(Name = "时段")]
        public string Name { get; set; }

        [Required(ErrorMessage = "请填写{0}")]
        [Domain("A", "B")]
        [Display(Name = "类型")]
        public string Flag { get; set; }

        [Required(ErrorMessage = "请填写{0}")]
        [StringLength(10, ErrorMessage = "请正确填写{0}")]
        [Display(Name = "大荤")]
        public string Meat { get; set; }

        [Required(ErrorMessage = "请填写{0}")]
        [StringLength(10, ErrorMessage = "请正确填写{0}")]
        [Display(Name = "小荤")]
        public string MeatSmall { get; set; }

        [Required(ErrorMessage = "请填写{0}")]
        [StringLength(10, ErrorMessage = "请正确填写{0}")]
        [Display(Name = "素菜(一)")]
        public string Vegetable1 { get; set; }

        [Required(ErrorMessage = "请填写{0}")]
        [StringLength(10, ErrorMessage = "请正确填写{0}")]
        [Display(Name = "素菜(二)")]
        public string Vegetable2 { get; set; }

        #endregion
    }
}