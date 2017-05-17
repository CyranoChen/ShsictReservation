using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shsict.Core;
using Shsict.Reservation.Mvc.Entities.Viewer;

namespace Shsict.Reservation.Tests
{
    [TestClass]
    public class ReportViewTest
    {
        [TestMethod]
        public void Test_Single_Viewer()
        {
            var factory = new ReportViewFactory();

            var instance1 = factory.Single(new Criteria
            {
                Parameters = new { DeliveryName = "堆高车" }
            });

            Assert.IsNotNull(instance1);
            Assert.IsInstanceOfType(instance1, typeof(ReportView));
            Assert.IsNotNull(instance1.Menu);
        }

        [TestMethod]
        public void Test_All_Pager_Viewer()
        {
            var factory = new ReportViewFactory();

            IPager pager = new Pager { PagingSize = 5 };

            Assert.IsFalse(pager.TotalCount > 0);

            var query = factory.All(pager, "DeliveryName");

            Assert.IsNotNull(query);
            Assert.IsInstanceOfType(query, typeof(List<ReportView>));
            Assert.IsTrue(query.Any());

            Assert.IsTrue(pager.TotalCount > 0);
            //Assert.AreEqual(pager.PagingSize.ToString(), query.Count.ToString());
        }

        [TestMethod]
        public void Test_Query_Viewer()
        {
            IViewerFactory<ReportView> factory = new ReportViewFactory();

            var criteria = new Criteria
            {
                WhereClause = $"(MenuDate < '{DateTime.Now}')",
                PagingSize = 0
            };

            var query = factory.Query(criteria);

            Assert.IsNotNull(query);
            Assert.IsInstanceOfType(query, typeof(List<ReportView>));
            Assert.IsTrue(query.Any());

            var instance = query.First();

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(ReportView));
            Assert.IsNotNull(instance.Menu);
        }
    }
}
