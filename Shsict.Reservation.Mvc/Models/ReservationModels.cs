using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Shsict.Core;
using Shsict.Reservation.Mvc.Entities;

namespace Shsict.Reservation.Mvc.Models
{
    public class ReservationModels
    {
        public class IndexDto
        {
            public DateTime MenuDate { get; set; }

            public string MenuStyle { get; set; }

            public bool CanReserveNow { get; set; }

            public MenuDto MenuA { get; set; }

            public MenuDto MenuB { get; set; }

            public List<Delivery> DeliveryZones { get; set; }

            public Delivery MyDefaultDeliveryZone { get; set; }

            public OrderDto MyCurrentOrder { get; set; }
        }

        public class MenuOrderDto
        {
            [Required(ErrorMessage = "请选择{0}")]
            [Display(Name = "套餐")]
            public int MenuID { get; set; }

            [Required(ErrorMessage = "请选择{0}")]
            [Domain("Rice", "Bun")]
            [Display(Name = "主食")]
            public string StapleFood { get; set; }

            [Display(Name = "加饭")]
            public string ExtraFood { get; set; }

            [Required(ErrorMessage = "请选择{0}")]
            [Display(Name = "送餐区域")]
            public Guid DeliveryZone { get; set; }

            [Required(ErrorMessage = "请选择{0}")]
            [Display(Name = "送餐点")]
            public Guid DeliveryPoint { get; set; }
        }

        public class HistoryDto
        {
            public DateTime MenuDate { get; set; }

            public OrderDto MyOrderLunch { get; set; }

            public OrderDto MyOrderSupper { get; set; }

            public List<OrderDto> MyHistroyOrders { get; set; }
        }
    }
}