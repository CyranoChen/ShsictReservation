using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shsict.Core;
using Shsict.Reservation.Mvc.Entities;

namespace Shsict.Reservation.Tests
{
    [TestClass]
    public class MenuTest
    {
        [TestMethod]
        public void DailyMenu_Test()
        {
            IRepository repo = new Repository();

            if (!repo.Any<Menu>(x => x.MenuDate == DateTime.Today))
            {
                // Today's menu no exist
                var menuL1 = new Menu
                {
                    MenuDate = DateTime.Today,
                    MenuType = MenuTypeEnum.Lunch,
                    MenuFlag = "A",
                    Meat = "红烧大排",
                    MeatSmall = "芙蓉鸡片",
                    Vegetable1 = "酸辣土豆丝",
                    Vegetable2 = "麻婆豆腐",
                    CreateTime = DateTime.Now,
                    CreateUser = "admin",
                    IsActive = true,
                    Remark = "TEST Lunch A"
                };

                var menuL2 = new Menu
                {
                    MenuDate = DateTime.Today,
                    MenuType = MenuTypeEnum.Lunch,
                    MenuFlag = "B",
                    Meat = "咖喱牛肉",
                    MeatSmall = "肉饼蒸蛋",
                    Vegetable1 = "油焖笋",
                    Vegetable2 = "凉拌马兰头",
                    CreateTime = DateTime.Now,
                    CreateUser = "admin",
                    IsActive = true,
                    Remark = "TEST Lunch B"
                };

                var menuS1 = new Menu
                {
                    MenuDate = DateTime.Today,
                    MenuType = MenuTypeEnum.Supper,
                    MenuFlag = "A",
                    Meat = "啤酒鸭",
                    MeatSmall = "番茄炒蛋",
                    Vegetable1 = "上汤菠菜",
                    Vegetable2 = "清炒冬瓜",
                    CreateTime = DateTime.Now,
                    CreateUser = "admin",
                    IsActive = true,
                    Remark = "TEST Supper A"
                };

                var menuS2 = new Menu
                {
                    MenuDate = DateTime.Today,
                    MenuType = MenuTypeEnum.Supper,
                    MenuFlag = "B",
                    Meat = "香煎带鱼",
                    MeatSmall = "雪菜炒鸡",
                    Vegetable1 = "香菇青菜",
                    Vegetable2 = "手撕包心菜",
                    CreateTime = DateTime.Now,
                    CreateUser = "admin",
                    IsActive = true,
                    Remark = "TEST Supper B"
                };

                using (var trans = DapperHelper.MarsConnection.BeginTransaction())
                {
                    try
                    {
                        repo.Insert(menuL1, trans);
                        repo.Insert(menuL2, trans);
                        repo.Insert(menuS1, trans);
                        repo.Insert(menuS2, trans);

                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }

            Assert.IsTrue(repo.Count<Menu>(x => x.MenuDate == DateTime.Today) == 4);
        }
    }
}

