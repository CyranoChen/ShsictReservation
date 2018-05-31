using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;
using Newtonsoft.Json.Linq;
using Shsict.Core;
using Shsict.Core.Dapper;
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

        public string DisplayCheckRequirement()
        {
            var result = new StringBuilder();

            if (this.JsonCheckRequirement != null)
            {
                var i = 1;
                foreach (var item in JsonCheckRequirement)
                {
                    result.AppendLine($"({i++}) {item}");
                }
            }

            return result.ToString();
        }

        public static class Cache
        {
            public static List<OperationStandardDto> OperationStandardDtoList;

            static Cache()
            {
                InitCache();
            }

            public static void RefreshCache()
            {
                InitCache();
            }

            private static void InitCache()
            {
                var mapper = OperationStandardDto.ConfigMapper().CreateMapper();

                OperationStandardDtoList = mapper.Map<IEnumerable<OperationStandardDto>>(
                    OperationStandard.Cache.OperationStandardList.AsEnumerable()).ToList();
            }

            public static OperationStandardDto Load(int id)
            {
                return OperationStandardDtoList.Find(x => x.ID.Equals(id));
            }
        }


        #region Members and Properties

        public int ID { get; set; }

        public string SecureNodeNo { get; set; }

        public string SecureNodeName { get; set; }

        public JArray JsonCheckRequirement { get; set; }

        #endregion
    }
}