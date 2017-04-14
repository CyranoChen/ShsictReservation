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

        [Display(Name = "�û���")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "����д{0}")]
        [StringLength(20, ErrorMessage = "����ȷ��д{0}")]
        [Display(Name = "Ա������")]
        public string EmployeeName { get; set; }

        [Required(ErrorMessage = "����д{0}")]
        [StringLength(20, ErrorMessage = "����ȷ��д{0}")]
        [Display(Name = "Ա������")]
        public string EmployeeNo { get; set; }

        [Required(ErrorMessage = "����д{0}")]
        [StringLength(20, ErrorMessage = "����ȷ��д{0}")]
        [Display(Name = "����")]
        public string Department { get; set; }

        [StringLength(20, ErrorMessage = "����ȷ��д{0}")]
        [Display(Name = "����")]
        public string Team { get; set; }

        [Required(ErrorMessage = "����д{0}")]
        [StringLength(20, ErrorMessage = "����ȷ��д{0}")]
        [Display(Name = "��λ")]
        public string Position { get; set; }

        [Required(ErrorMessage = "����д{0}")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "����ȷ��д{0}")]
        [Display(Name = "�ֻ�")]
        public string Mobile { get; set; }

        public UserRoleEnum Role { get; set; }

        public string Avatar { get; set; }

        public short Gender { get; set; }

        #endregion
    }
}