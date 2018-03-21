using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Newtonsoft.Json.Linq;
using Shsict.Reservation.Mvc.Entities.SecureNode;

namespace Shsict.Reservation.Mvc.Models.SecureNode
{
    public class OperationStandardDto
    {
        public static MapperConfiguration ConfigMapper()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<OperationStandard, OperationStandardDto>()
                .ForMember(d => d.JsonCheckRequirement, opt => opt.MapFrom(s => JArray.Parse(s.CheckRequirement)))
            );

            return config;
        }

        public static OperationStandardDto Load(int id)
        {
            var mapper = OperationStandardDto.ConfigMapper().CreateMapper();

            return mapper.Map<OperationStandardDto>(OperationStandard.Cache.Load(id));
        }

        #region Members and Properties

        public int ID { get; set; }

        public string SecureNodeName { get; set; }

        public JArray JsonCheckRequirement { get; set; }

        #endregion
    }
}