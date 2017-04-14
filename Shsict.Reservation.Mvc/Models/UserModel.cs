using System;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Shsict.Reservation.Mvc.Entities;

namespace Shsict.Reservation.Mvc.Models
{
    public class UserDto
    {
        public static MapperConfiguration ConfigMapper()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<User, UserDto>()
                .ForMember(d => d.UserId, opt => opt.MapFrom(s => s.UserName))
                );

            return config;
        }


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

        [StringLength(20, ErrorMessage = "请正确填写{0}")]
        [Display(Name = "班组")]
        public string Team { get; set; }

        [Required(ErrorMessage = "请填写{0}")]
        [StringLength(20, ErrorMessage = "请正确填写{0}")]
        [Display(Name = "岗位")]
        public string Position { get; set; }

        [Required(ErrorMessage = "请填写{0}")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "请正确填写{0}")]
        [Display(Name = "手机")]
        public string Mobile { get; set; }

        public UserRoleEnum Role { get; set; }

        public string Avatar { get; set; }

        public short Gender { get; set; }

        #endregion
    }
}