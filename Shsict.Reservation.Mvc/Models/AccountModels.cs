using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json.Linq;
using Shsict.Core;
using Shsict.Reservation.Mvc.Entities;
using Shsict.Reservation.Mvc.Services;

namespace Shsict.Reservation.Mvc.Models
{
    public class MyAvatarDto
    {
        public IEnumerable<UserDto> Avatars { get; set; }
    }

    public class UserProfileDto
    {
        [Required(ErrorMessage = "请填写{0}")]
        [StringLength(20, ErrorMessage = "请正确填写{0}")]
        [Display(Name = "真实姓名")]
        public string RealName { get; set; }

        [Required(ErrorMessage = "请填写{0}")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "请正确填写{0}")]
        [Display(Name = "手机")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "请填写{0}")]
        [DataType(DataType.EmailAddress, ErrorMessage = "请正确填写{0}")]
        [Display(Name = "邮箱")]
        public string Email { get; set; }
    }

    public class ChangePasswordModel
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

    public class LoginModel
    {
        [Required(ErrorMessage = "请填写{0}")]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "请填写{0}")]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Display(Name = "记住密码")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required(ErrorMessage = "请填写{0}")]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "请填写{0}")]
        [Display(Name = "邮箱")]
        [DataType(DataType.EmailAddress, ErrorMessage = "请正确填写{0}")]
        public string Email { get; set; }

        [Required(ErrorMessage = "请填写{0}")]
        [StringLength(100, ErrorMessage = "{0}长度至少需要{2}位", MinimumLength = 7)]
        [DataType(DataType.Password, ErrorMessage = "请正确填写{0}")]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "新密码与确认密码不一致")]
        [DataType(DataType.Password, ErrorMessage = "请正确填写{0}")]
        [Display(Name = "密码确认")]
        public string ConfirmPassword { get; set; }
    }
}