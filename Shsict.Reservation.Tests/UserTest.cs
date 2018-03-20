using System;
using System.Linq;
using Shsict.Core;
using Shsict.Reservation.Mvc.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shsict.Core.Utility;
using Shsict.Reservation.Mvc.Models;
using Shsict.Reservation.Mvc.Services;

namespace Shsict.Reservation.Tests
{
    [TestClass]
    public class UserTest
    {
        [TestMethod]
        public void Test_Insert_Update_Delete()
        {
            var user = new User
            {
                UserName = "cyrano",
                Password = Encrypt.GetMd5Hash("shsict"),
                EmployeeName = "陈继麟",
                EmployeeNo = "2607",
                Department = "智慧交通事业部",
                Team = "民航与港口行业",
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

            repo.Insert(user, out key);

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

        [TestMethod]
        public void Test_AutoMapper_User_UserDto()
        {
            var user = new User
            {
                UserName = "cyrano",
                Password = Encrypt.GetMd5Hash("shsict"),
                EmployeeName = "陈继麟",
                EmployeeNo = "2607",
                Department = "智慧交通事业部",
                Team = "民航与港口行业",
                Position = "产品总监",
                Email = "cyrano@arsenalcn.com",
                Mobile = "13818059707",
                IsApproved = true,
                LastLoginDate = DateTime.Today,
                CreateDate = DateTime.Now,
                IsActive = true,
                Remark = "Test Data"
            };

            var u = user.MapTo<User, UserDto>();

            u.UserId = user.UserName;

            Assert.IsNotNull(u.UserId);
            Assert.IsNotNull(u.EmployeeName);
        }

        [TestMethod]
        public void Test_SyncUsersWithWeChat()
        {
            IRepository repo = new Repository();

            var list = repo.All<UserWeChat>().Take(10).ToList();

            var auth = new AuthorizeManager();

            if (list.Count > 0)
            {
                foreach (var uw in list)
                {
                    var user = auth.SyncUserWithWeChat(uw.UserName);

                    Assert.IsNotNull(user);
                    Assert.AreEqual(user.ID, uw.ID);
                }
            }
        }

        [Ignore]
        [TestMethod]
        public void Test_BatchAddUser()
        {
            var usernames = new[]
            {
                "yehong116", "zhangwei178", "xiezhaqin321", "xujianming325",
                "jiangfeng552", "gewenjun598", "luozhenhua631", "xieshendong635"
            };

            IRepository repo = new Repository();

            foreach (var name in usernames)
            {
                var userWeChat = new UserWeChat
                {
                    UserName = name,
                    LastAuthorizeDate = DateTime.Now,
                    Gender = 1,
                    Avatar = string.Empty,
                    DeviceId = string.Empty
                };

                object uid;

                repo.Insert(userWeChat, out uid);

                if (uid != null)
                {
                    var user = new User
                    {
                        ID = (Guid)uid,
                        UserName = name,
                        Password = Encrypt.GetMd5Hash("shsict"),
                        WeChatOpenId = string.Empty,
                        WeChatNickName = string.Empty,
                        EmployeeName = string.Empty,
                        EmployeeNo = string.Empty,
                        Department = string.Empty,
                        Team = string.Empty,
                        Position = string.Empty,
                        Email = string.Empty,
                        Mobile = string.Empty,
                        // important: role
                        Role = UserRoleEnum.Manager,
                        IsApproved = true,
                        LastLoginDate = DateTime.Now,
                        CreateDate = DateTime.Now,
                        IsActive = true,
                        Remark = string.Empty
                    };

                    repo.Insert(user);
                }
            }
        }
    }
}
