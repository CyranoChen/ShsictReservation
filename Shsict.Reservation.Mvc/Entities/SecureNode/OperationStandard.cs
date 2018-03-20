using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Shsict.Core;

namespace Shsict.Reservation.Mvc.Entities.SecureNode
{
    [DbSchema("SecureNode_OperationStandard")]
    public class OperationStandard : Entity<int>
    {
        #region Members and Properties

        [DbColumn("SecureNodeName")]
        public string SecureNodeName { get; set; }

        [DbColumn("CheckRequirement")]
        public string CheckRequirement { get; set; }

        [DbColumn("CheckCount")]
        public int CheckCount { get; set; }

        #endregion
    }
}