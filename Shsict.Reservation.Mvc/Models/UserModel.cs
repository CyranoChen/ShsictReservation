using System;
using System.ComponentModel.DataAnnotations;

namespace Shsict.Reservation.Mvc.Models
{
    public class UserDto
    {
        #region Members and Properties

        public Guid ID { get; set; }

        [Display(Name = "用户名")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "请填写{0}")]
        [StringLength(20, ErrorMessage = "请正确填写{0}")]
        [Display(Name = "员工姓名")]
        public string EmployeeName { get; set; }

        [Required(ErrorMessage = "请填写{0}")]
        [StringLength(20, ErrorMessage = "请正确填写{0}")]
        [Display(Name = "员工工号")]
        public string EmployeeNo { get; set; }

        [Required(ErrorMessage = "请填写{0}")]
        [StringLength(20, ErrorMessage = "请正确填写{0}")]
        [Display(Name = "部门")]
        public string Department { get; set; }

        [Required(ErrorMessage = "请填写{0}")]
        [StringLength(20, ErrorMessage = "请正确填写{0}")]
        [Display(Name = "岗位")]
        public string Position { get; set; }

        [Required(ErrorMessage = "请填写{0}")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "请正确填写{0}")]
        [Display(Name = "手机")]
        public string Mobile { get; set; }

        public string Avatar { get; set; }

        public short Gender { get; set; }

        #endregion
    }
}