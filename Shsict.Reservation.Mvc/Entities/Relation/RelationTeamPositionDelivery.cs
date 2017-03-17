using System;
using Shsict.Core;

namespace Shsict.Reservation.Mvc.Entities.Relation
{
    [DbSchema("Relation_Team_Position_Delivery", Sort = "Team, Position")]
    public class RelationTeamPositionDelivery : Dao
    {
        #region Members and Properties

        [DbColumn("Team")]
        public string Team { get; set; }

        [DbColumn("Position")]
        public string Position { get; set; }

        [DbColumn("DeliveryGuid")]
        public Guid DeliveryGuid { get; set; }

        #endregion
    }
}