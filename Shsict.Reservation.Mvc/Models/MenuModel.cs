using System;
using System.ComponentModel.DataAnnotations;
using Shsict.Core;

namespace Shsict.Reservation.Mvc.Models
{
    public class MenuDto
    {
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