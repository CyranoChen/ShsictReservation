using System;
using Shsict.Core;

namespace Shsict.Reservation.Mvc.Entities
{
    [DbSchema("Shsict_User", Key = "UserGuid", Sort = "CreateDate DESC")]
    public class User : Entity<Guid>
    {
        #region Members and Properties

        public override Guid ID
        {
            get
            {
                if (UserGuid.Equals(Guid.Empty))
                {
                    UserGuid = Guid.NewGuid();
                }

                return UserGuid;
            }
            set { UserGuid = value; }
        }

        private Guid UserGuid { get; set; }

        [DbColumn("UserName")]
        public string UserName { get; set; }

        [DbColumn("Password")]
        public string Password { get; set; }

        [DbColumn("WeChatOpenId")]
        public string WeChatOpenId { get; set; }

        [DbColumn("WeChatNickName")]
        public string WeChatNickName { get; set; }

        [DbColumn("EmployeeName")]
        public string EmployeeName { get; set; }

        [DbColumn("EmployeeNo")]
        public string EmployeeNo { get; set; }

        [DbColumn("Department")]
        public string Department { get; set; }

        [DbColumn("Team")]
        public string Team { get; set; }

        [DbColumn("Position")]
        public string Position { get; set; }

        [DbColumn("Email")]
        public string Email { get; set; }

        [DbColumn("Mobile")]
        public string Mobile { get; set; }

        [DbColumn("Role")]
        public UserRoleEnum Role { get; set; }

        [DbColumn("IsApproved")]
        public bool IsApproved { get; set; }

        [DbColumn("LastLoginDate")]
        public DateTime LastLoginDate { get; set; }

        [DbColumn("CreateDate")]
        public DateTime CreateDate { get; set; }

        [DbColumn("IsActive")]
        public bool IsActive { get; set; }

        [DbColumn("Remark")]
        public string Remark { get; set; }

        #endregion
    }

    public enum UserRoleEnum
    {
        Employee = 0,
        Manager = 1,
        Canteen = 2,
        Admin = 3
    }
}