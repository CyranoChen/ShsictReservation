﻿using System;
using Shsict.Core;

namespace Shsict.Reservation.Mvc.Entities.SecureNode
{
    [DbSchema("SecureNode_CheckList", Sort = "CheckTime DESC")]
    public class CheckList : Entity<int>
    {
        #region Members and Properties

        [DbColumn("SecureNodeId")]
        public int SecureNodeId { get; set; }

        [DbColumn("OperateDate")]
        public DateTime OperateDate { get; set; }

        [DbColumn("Shift")]
        public string Shift { get; set; }

        [DbColumn("CheckTime")]
        public DateTime CheckTime { get; set; }

        [DbColumn("CheckLocation")]
        public string CheckLocation { get; set; }

        [DbColumn("CheckNodePoint")]
        public int CheckNodePoint { get; set; }

        [DbColumn("CheckResult")]
        public bool CheckResult { get; set; }

        [DbColumn("CheckEmployeeNo")]
        public string CheckEmployeeNo { get; set; }

        [DbColumn("IsActive")]
        public bool IsActive { get; set; }

        [DbColumn("Remark")]
        public string Remark { get; set; }

        #endregion

        //public static void Clean(int dateDiff)
        //{
        //    IRepository repo = new Repository();

        //    var criteria = new Criteria
        //    {
        //        WhereClause = $"CreateTime < '{DateTime.Today.AddDays(dateDiff)}' AND IsActive = 0",
        //        OrderClause = "CreateTime DESC",
        //        PagingSize = 0
        //    };

        //    var list = repo.Query<Order>(criteria);

        //    if (list != null && list.Count > 0)
        //    {
        //        list.Delete();
        //    }
        //}
    }
}