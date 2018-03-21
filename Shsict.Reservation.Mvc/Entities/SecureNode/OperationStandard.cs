using System.Collections.Generic;
using Shsict.Core;
using Shsict.Core.Dapper;

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

        public static class Cache
        {
            public static List<OperationStandard> OperationStandardList;

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
                IRepository repo = new Repository();

                OperationStandardList = repo.All<OperationStandard>();
            }

            public static OperationStandard Load(int id)
            {
                return OperationStandardList.Find(x => x.ID.Equals(id));
            }
        }
    }
}