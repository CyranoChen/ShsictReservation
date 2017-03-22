using System;
using System.Collections.Generic;
using Shsict.Core;

namespace Shsict.Reservation.Mvc.Models
{
    public class ConsoleModels
    {
        public class MenuManagementDto
        {
            public DateTime? MenuDate { get; set; }

            public List<MenuDto> Menus { get; set; }
        }

        public class OrderManagementDto
        {
            public DateTime? MenuDate { get; set; }

            public List<OrderDto> Orders { get; set; }
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