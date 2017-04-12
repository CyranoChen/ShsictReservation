using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shsict.Core;
using Shsict.Reservation.Mvc.Entities.Viewer;

namespace Shsict.Reservation.Tests
{
    [TestClass]
    public class OrderViewTest
    {
        [TestMethod]
        public void Test_Single_Viewer()
        {
            var factory = new OrderViewFactory();

            var key1 = 14;

            var instance1 = factory.Single(key1);

            Assert.IsNotNull(instance1);
            Assert.IsInstanceOfType(instance1, typeof(OrderView));
            Assert.IsNotNull(instance1.User);
            Assert.IsNotNull(instance1.Menu);
            Assert.IsNotNull(instance1.Delivery);
        }

        [TestMethod]
        public void Test_All_Pager_Viewer()
        {
            var factory = new OrderViewFactory();

            IPager pager = new Pager { PagingSize = 20 };

            Assert.IsFalse(pager.TotalCount > 0);

            var query = factory.All(pager, "PlaceTime DESC");

            Assert.IsNotNull(query);
            Assert.IsInstanceOfType(query, typeof(List<OrderView>));
            Assert.IsTrue(query.Any());

            Assert.IsTrue(pager.TotalCount > 0);
            //Assert.AreEqual(pager.PagingSize.ToString(), query.Count.ToString());
        }

        [TestMethod]
        public void Test_Query_Viewer()
        {
            IViewerFactory<OrderView> factory = new OrderViewFactory();

            var criteria = new Criteria
            {
                WhereClause = $"(PlaceTime < '{DateTime.Now}')",
                PagingSize = 0
            };

            var query = factory.Query(criteria);

            Assert.IsNotNull(query);
            Assert.IsInstanceOfType(query, typeof(List<OrderView>));
            Assert.IsTrue(query.Any());

            var instance = query.First();

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(OrderView));
            Assert.IsNotNull(instance.User);
            Assert.IsNotNull(instance.Menu);
            Assert.IsNotNull(instance.Delivery);
        }

        [TestMethod]
        public void Test_Query_Pager_Viewer()
        {
            var factory = new OrderViewFactory();

            var criteria = new Criteria
            {
                PagingSize = 20,
                WhereClause = "StapleFood = 2 AND ExtraFood = 1",
                OrderClause = "CreateTime DESC"
            };

            Assert.IsFalse(criteria.TotalCount > 0);

            var query = factory.Query(criteria);

            Assert.IsNotNull(query);
            Assert.IsInstanceOfType(query, typeof(List<OrderView>));
            Assert.IsTrue(query.Any());

            Assert.IsTrue(criteria.TotalCount > 0);
            //Assert.AreEqual(criteria.PagingSize.ToString(), query.Count.ToString());
        }

    }
}
