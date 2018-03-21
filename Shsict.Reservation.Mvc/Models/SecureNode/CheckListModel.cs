using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using AutoMapper;
using Shsict.Core;
using Shsict.Reservation.Mvc.Entities;
using Shsict.Reservation.Mvc.Entities.SecureNode;
using Shsict.Reservation.Mvc.Entities.Viewer;

namespace Shsict.Reservation.Mvc.Models.SecureNode
{
    public class CheckListDto
    {
        public static MapperConfiguration ConfigMapper()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<CheckList, CheckListDto>()
                .ForMember(d => d.ShiftInfo, opt => opt.ResolveUsing(s =>
                {
                    switch (s.Shift)
                    {
                        case "daytime":
                            return "日班";
                        case "night":
                            return "夜班";
                        default:
                            return string.Empty;
                    }
                }))
                .ForMember(d => d.SecureNode, opt => opt.MapFrom(s => 
                    OperationStandard.Cache.Load(s.SecureNodeId)))
            );

            return config;
        }


        #region Members and Properties

        public int ID { get; set; }

        [Display(Name= "重点危险节点")]
        public OperationStandard SecureNode { get; set; }

        public DateTime OperateDate { get; set; }

        public string Shift { get; set; }

        public string ShiftInfo { get; set; }

        public DateTime CheckTime { get; set; }

        public string CheckLocation { get; set; }

        public int CheckNodePoint { get; set; }

        public bool CheckResult { get; set; }

        public string CheckEmployeeNo { get; set; }

        public string CheckEmployeeName { get; set; }

        public string Remark { get; set; }

        #endregion
    }
}