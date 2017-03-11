using System;
using Shsict.Core;

namespace Shsict.Reservation.Mvc.Entities
{
    [DbSchema("Shsict_UserWeChat", Key = "UserGuid", Sort = "LastAuthorizeDate DESC")]
    public class UserWeChat : Entity<Guid>
    {
        #region Members and Properties

        [DbColumn("UserName")]
        public string UserName { get; set; }

        [DbColumn("LastAuthorizeDate")]
        public DateTime LastAuthorizeDate { get; set; }

        [DbColumn("Gender")]
        public short Gender { get; set; }

        [DbColumn("Avatar")]
        public string Avatar { get; set; }

        [DbColumn("DeviceId")]
        public string DeviceId { get; set; }

        #endregion
    }
}