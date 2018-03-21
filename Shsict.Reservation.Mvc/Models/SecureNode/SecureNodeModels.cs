using System;
using System.Collections.Generic;

namespace Shsict.Reservation.Mvc.Models.SecureNode
{
    public class SecureNodeModels
    {
        public class CheckListManagementDto
        {
            public DateTime? OperateDate { get; set; }

            public List<CheckListDto> CheckLists { get; set; }
        }

        public class HistoryDto
        {
            public DateTime? OperateDate { get; set; }

            public List<CheckListDto> MyCheckLists { get; set; }
        }
    }
}