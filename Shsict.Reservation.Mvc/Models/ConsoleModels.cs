using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Shsict.Core;
using Shsict.Reservation.Mvc.Entities;

namespace Shsict.Reservation.Mvc.Models
{
    public class ConsoleModels
    {
        public class UserManagementDto
        {
            public List<UserDto> Users { get; set; }
        }
    }
}