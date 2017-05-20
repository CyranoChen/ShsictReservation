using System.Collections.Generic;
using System.Data;
using System.Linq;
using Shsict.Core;

namespace Shsict.Reservation.Mvc.Entities.Viewer
{
    public class ReportViewFactory : ViewerFactory, IViewerFactory<ReportView>
    {
        public ReportViewFactory()
        {
            Dapper = new DapperHelper();

            ViewerSql = @"SELECT dp.DeliveryGuid, dp.DeliveryName, o.StapleFood, o.ExtraFood, COUNT(o.ID) AS OrderCount, m.ID, m.MenuDate, m.MenuType, m.MenuFlag, m.Meat, m.MeatSmall, m.Vegetable1, m.Vegetable2
                                    FROM Reservation_Order AS o INNER JOIN 
                                    Reservation_Menu AS m ON o.MenuID = m.ID AND m.IsActive = 1 AND m.IsApproved =1 LEFT OUTER JOIN 
                                    Reservation_Delivery AS d ON o.DeliveryGuid = d.DeliveryGuid AND d.IsActive = 1 LEFT OUTER JOIN 
                                    Reservation_Delivery AS dp ON d.ParentID = dp.DeliveryGuid AND d.IsActive = 1 
                                    WHERE (o.IsActive = 1)
                                    GROUP BY dp.DeliveryGuid, dp.DeliveryName, o.StapleFood, o.ExtraFood, m.ID, m.MenuDate, m.MenuType, m.MenuFlag, m.Meat, m.MeatSmall, m.Vegetable1, m.Vegetable2";

            SplitOn = "DeliveryGuid, ID";

            DbSchema = Repository.GetTableAttr<ReportView>();
        }

        public ReportView Single(Criteria criteria, IDbTransaction trans = null)
        {
            return Dapper.Query<ReportView, Menu, ReportView>(BuildSingleSql(criteria),
                        (x, m) =>
                        {
                            x.Menu = m;

                            return x;
                        }, criteria?.Parameters, trans, SplitOn).FirstOrDefault();
        }

        public List<ReportView> All(IDbTransaction trans = null)
        {
            return Dapper.Query<ReportView, Menu, ReportView>(BuildAllSql(),
                        (x, m) =>
                        {
                            x.Menu = m;

                            return x;
                        }, null, trans, SplitOn).ToList();
        }

        public List<ReportView> All(IPager pager, string orderBy = null, IDbTransaction trans = null)
        {
            return Dapper.Query<ReportView, Menu, ReportView>(BuildAllSql(pager, orderBy),
                        (x, m) =>
                        {
                            x.Menu = m;

                            return x;
                        }, null, trans, SplitOn).ToList();
        }

        public List<ReportView> Query(Criteria criteria, IDbTransaction trans = null)
        {
            return Dapper.Query<ReportView, Menu, ReportView>(BuildQuerySql(criteria),
                        (x, m) =>
                        {
                            x.Menu = m;

                            return x;
                        }, criteria?.Parameters, trans, SplitOn).ToList();
        }
    }
}
