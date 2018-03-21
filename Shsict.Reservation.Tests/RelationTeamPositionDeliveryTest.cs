using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shsict.Core;
using Shsict.Core.Dapper;
using Shsict.Reservation.Mvc.Entities.Relation;

namespace Shsict.Reservation.Tests
{
    [TestClass]
    public class RelationTeamPositionDeliveryTest
    {
        [TestMethod]
        public void Test_Insert_Update_Delete()
        {
            var rela = new RelationTeamPositionDelivery
            {
                Team = "民航与港口行业",
                Position = "产品总监",
                DeliveryGuid = new Guid("911954b1-daf9-48a9-b9b7-f16afec50dde")
            };

            IRepository repo = new Repository();

            repo.Insert(rela);

            var res = repo.Single<RelationTeamPositionDelivery>(x => 
                x.Team == "民航与港口行业" && x.Position == "产品总监");

            Assert.IsNotNull(res);
            Assert.IsInstanceOfType(res, typeof(RelationTeamPositionDelivery));
            Assert.AreEqual(new Guid("911954b1-daf9-48a9-b9b7-f16afec50dde"), res.DeliveryGuid);

            res.DeliveryGuid = new Guid("1b14ae1e-1a10-47b2-8451-d7e49976d26c");

            repo.Update(res, x => x.Team == "民航与港口行业" && x.Position == "产品总监");

            var resUpdated = repo.Single<RelationTeamPositionDelivery>(x =>
                x.Team == "民航与港口行业" && x.Position == "产品总监");

            Assert.IsNotNull(resUpdated);
            Assert.IsInstanceOfType(resUpdated, typeof(RelationTeamPositionDelivery));
            //Assert.IsTrue(resUpdated.Equals(res));
            Assert.AreEqual(new Guid("1b14ae1e-1a10-47b2-8451-d7e49976d26c"), resUpdated.DeliveryGuid);

            repo.Delete< RelationTeamPositionDelivery>(x => 
                x.Team == "民航与港口行业" && x.Position == "产品总监");

            var resDeleted = repo.Single<RelationTeamPositionDelivery>(x =>
                x.Team == "民航与港口行业" && x.Position == "产品总监");

            Assert.IsNull(resDeleted);
        }
    }
}
