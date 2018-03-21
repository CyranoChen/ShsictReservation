using System.Collections.Generic;
using System.Data;
using System.Linq;
using Shsict.Core;
using Shsict.Core.Dapper;

namespace Shsict.Reservation.Mvc.Entities.Viewer
{
    public class ReportViewFactory : ViewerFactory, IViewerFactory<ReportView>
    {
        private IDapperHelper _dapper;

        public ReportViewFactory(IDapperHelper dapper = null)
        {
            _dapper = dapper ?? DapperHelper.GetInstance();

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

        public ReportView Single(Criteria criteria)
        {
            return _dapper.Query<ReportView, Menu, ReportView>(BuildSingleSql(criteria),
                        (x, m) =>
                        {
                            x.Menu = m;

                            return x;
                        }, criteria?.Parameters, SplitOn).FirstOrDefault();
        }

        public List<ReportView> All()
        {
            return _dapper.Query<ReportView, Menu, ReportView>(BuildAllSql(),
                        (x, m) =>
                        {
                            x.Menu = m;

                            return x;
                        }, null, SplitOn).ToList();
        }

        public List<ReportView> All(IPager pager, string orderBy = null)
        {
            return _dapper.Query<ReportView, Menu, ReportView>(BuildAllSql(pager, orderBy),
                        (x, m) =>
                        {
                            x.Menu = m;

                            return x;
                        }, null, SplitOn).ToList();
        }

        public List<ReportView> Query(Criteria criteria)
        {
            return _dapper.Query<ReportView, Menu, ReportView>(BuildQuerySql(criteria),
                        (x, m) =>
                        {
                            x.Menu = m;

                            return x;
                        }, criteria?.Parameters, SplitOn).ToList();
        }
    }
}
