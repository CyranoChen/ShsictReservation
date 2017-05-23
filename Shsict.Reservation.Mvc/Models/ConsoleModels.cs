using System;
using System.Collections.Generic;
using Shsict.Core;
using Shsict.Reservation.Mvc.Entities;

namespace Shsict.Reservation.Mvc.Models
{
    public class ConsoleModels
    {
        public class IndexDto
        {
            public List<ReportDto> Reports { get; set; }

            public string ApproverInfo { get; set; }
        }

        public class MenuManagementDto
        {
            public DateTime? MenuDate { get; set; }

            public List<MenuDto> Menus { get; set; }
        }

        public class OrderManagementDto
        {
            public DateTime? MenuDate { get; set; }

            public List<Delivery> DeliveryZones { get; set; }

            public List<OrderDto> Orders { get; set; }

            public bool IsMenuApproved { get; set; }

            public string ApproverInfo { get; set; }
        }

        public class UserManagementDto
        {
            public List<UserDto> Users { get; set; }
        }

        public class ConfigManagementDto
        {
            public List<Config> Configs { get; set; }
        }
    }
}