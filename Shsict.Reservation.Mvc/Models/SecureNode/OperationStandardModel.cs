using System.Text;
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

        #region Members and Properties

        public int ID { get; set; }

        public string SecureNodeNo { get; set; }

        public string SecureNodeName { get; set; }

        public JArray JsonCheckRequirement { get; set; }

        #endregion
    }
}