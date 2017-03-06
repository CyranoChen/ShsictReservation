using System;
using Shsict.Core;
using Shsict.Reservation.Mvc.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shsict.Core.Utility;

namespace Shsict.Reservation.Tests
{
    [TestClass]
    public class UserTest
    {
        [TestMethod]
        public void Test_Insert_Update_Delete()
        {
            var u = new User
            {
                UserName = "陈继麟", 
                Password = Encrypt.GetMd5Hash("shsict"), 
                EmployeeNo = "2607", 
                Position = "产品总监", 
                Email = "cyrano@arsenalcn.com",
                Mobile = "13818059707",
                IsApproved = true,
                LastLoginDate = DateTime.Today,
                CreateDate = DateTime.Now,
                IsActive = true, 
                Remark = "Test Data"
            };

            IRepository repo = new Repository();

            object key;

            repo.Insert(u, out key);

            Assert.IsNotNull(key);

            var res = repo.Single<User>(key);

            Assert.IsNotNull(res);
            Assert.IsInstanceOfType(res, typeof(User));

            res.IsApproved = false;
            res.IsActive = false;

            repo.Update(res);

            var resUpdated = repo.Single<User>(key);

            Assert.IsNotNull(resUpdated);
            Assert.IsInstanceOfType(resUpdated, typeof(User));
            Assert.IsTrue(resUpdated.Equals(res));
            Assert.IsFalse(resUpdated.IsActive);

            repo.Delete(res);

            var resDeleted = repo.Single<User>(key);

            Assert.IsNull(resDeleted);

            repo.Insert(res);
            repo.Delete<User>(key);

            Assert.IsNull(repo.Single<User>(key));
        }
    }
}
