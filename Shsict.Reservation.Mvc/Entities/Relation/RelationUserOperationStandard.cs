using System;
using Shsict.Core;

namespace Shsict.Reservation.Mvc.Entities.Relation
{
    [DbSchema("Relation_User_OperationStandard", Sort = "OperationStandardId")]
    public class RelationUserOperationStandard : Dao
    {
        #region Members and Properties

        [DbColumn("UserGuid")]
        public Guid UserGuid { get; set; }

        [DbColumn("OperationStandardId")]
        public int OperationStandardId { get; set; }

        #endregion
    }
}