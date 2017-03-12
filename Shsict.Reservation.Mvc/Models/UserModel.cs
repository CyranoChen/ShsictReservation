using System;

namespace Shsict.Reservation.Mvc.Models
{
    public class UserDto
    {
        #region Members and Properties

        public string UserId { get; set; }

        public string EmployeeName { get; set; }

        public string EmployeeNo { get; set; }

        public string Department { get; set; }

        public string Position { get; set; }

        public string Mobile { get; set; }

        public string Avatar { get; set; }

        public short Gender { get; set; }

        #endregion
    }
}