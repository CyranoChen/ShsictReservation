using System.ComponentModel.DataAnnotations;

namespace Shsict.Reservation.Mvc.Models
{
    public class AccountModels
    {
        public class PasswordDto
        {
            [Required(ErrorMessage = "请填写{0}")]
            [DataType(DataType.Password, ErrorMessage = "请正确填写{0}")]
            [Display(Name = "当前密码")]
            public string OldPassword { get; set; }

            [Required(ErrorMessage = "请填写{0}")]
            [StringLength(100, ErrorMessage = "{0}长度至少需要{2}位", MinimumLength = 6)]
            [DataType(DataType.Password, ErrorMessage = "请正确填写{0}")]
            [Display(Name = "新密码")]
            public string NewPassword { get; set; }

            [Compare("NewPassword", ErrorMessage = "新密码与确认密码不一致")]
            [DataType(DataType.Password, ErrorMessage = "请正确填写{0}")]
            [Display(Name = "确认密码")]
            public string ConfirmPassword { get; set; }
        }

        public class LoginDto
        {
            [Required(ErrorMessage = "请填写{0}")]
            [Display(Name = "用户名")]
            public string UserId { get; set; }

            [Required(ErrorMessage = "请填写{0}")]
            [DataType(DataType.Password)]
            [Display(Name = "密码")]
            public string Password { get; set; }

            [Display(Name = "记住密码")]
            public string RememberMe { get; set; }
        }
    }
}