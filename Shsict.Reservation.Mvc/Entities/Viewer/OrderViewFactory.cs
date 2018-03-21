using System.Collections.Generic;
using System.Linq;
using Shsict.Core;
using Shsict.Core.Dapper;


namespace Shsict.Reservation.Mvc.Entities.Viewer
{
    public class OrderViewFactory : ViewerFactory, IViewerFactory<OrderView>
    {
        private IDapperHelper _dapper;

        public OrderViewFactory(IDapperHelper dapper = null)
        {
            _dapper = dapper ?? DapperHelper.GetInstance();

            ViewerSql = @"SELECT o.ID AS OrderID, o.StapleFood, o.ExtraFood, o.CreateTime AS PlaceTime, o.CreateUser AS PlaceUser, 
                                    u.UserGuid, u.UserName, u.EmployeeName, u.EmployeeNo, m.*, d.DeliveryGuid, d.DeliveryName, d.ParentID 
                                    FROM Reservation_Order AS o LEFT OUTER JOIN 
                                              Shsict_User AS u ON o.UserGuid = u.UserGuid LEFT OUTER JOIN 
                                              Reservation_Menu AS m ON o.MenuID = m.ID LEFT OUTER JOIN 
                                              Reservation_Delivery AS d ON o.DeliveryGuid = d.DeliveryGuid 
                                    WHERE  (o.IsActive = 1) ";

            SplitOn = "OrderID, UserGuid, ID, DeliveryGuid";

            DbSchema = Repository.GetTableAttr<OrderView>();
        }

        public OrderView Single(object key)
        {
            return _dapper.Query<OrderView, User, Menu, Delivery, OrderView>(BuildSingleSql(),
                        (x, u, m, d) =>
                        {
                            x.User = u;
                            x.Menu = m;
                            x.Delivery = d;

                            return x;
                        }, new { key }, SplitOn).FirstOrDefault();
        }

        public OrderView Single(Criteria criteria)
        {
            return _dapper.Query<OrderView, User, Menu, Delivery, OrderView>(BuildSingleSql(criteria),
                        (x, u, m, d) =>
                        {
                            x.User = u;
                            x.Menu = m;
                            x.Delivery = d;

                            return x;
                        }, criteria?.Parameters, SplitOn).FirstOrDefault();
        }

        public List<OrderView> All()
        {
            return _dapper.Query<OrderView, User, Menu, Delivery, OrderView>(BuildAllSql(),
                        (x, u, m, d) =>
                        {
                            x.User = u;
                            x.Menu = m;
                            x.Delivery = d;

                            return x;
                        }, null, SplitOn).ToList();
        }

        public List<OrderView> All(IPager pager, string orderBy = null)
        {
            return _dapper.Query<OrderView, User, Menu, Delivery, OrderView>(BuildAllSql(pager, orderBy),
                        (x, u, m, d) =>
                        {
                            x.User = u;
                            x.Menu = m;
                            x.Delivery = d;

                            return x;
                        }, null, SplitOn).ToList();
        }

        public List<OrderView> Query(Criteria criteria)
        {
            return _dapper.Query<OrderView, User, Menu, Delivery, OrderView>(BuildQuerySql(criteria),
                        (x, u, m, d) =>
                        {
                            x.User = u;
                            x.Menu = m;
                            x.Delivery = d;

                            return x;
                        }, criteria?.Parameters, SplitOn).ToList();
        }
    }
}
