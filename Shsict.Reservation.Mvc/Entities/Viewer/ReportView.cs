using Shsict.Core;

namespace Shsict.Reservation.Mvc.Entities.Viewer
{
    [DbSchema("Console_ReportView", Sort = "DeliveryName")]
    public class ReportView : Core.Viewer
    {
        #region Members and Properties
        [DbColumn("DeliveryName")]
        public string DeliveryName { get; set; }

        [DbColumn("StapleFood")]
        public StapleFoodEnum StapleFood { get; set; }

        [DbColumn("ExtraFood")]
        public bool ExtraFood { get; set; }

        [DbColumn("OrderCount")]
        public int OrderCount { get; set; }

        // Complex Object
        [DbColumn("m", Key = "ID")]
        public Menu Menu { get; set; }

        #endregion
    }
}