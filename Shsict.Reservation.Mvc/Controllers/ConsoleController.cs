﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Shsict.Core;
using Shsict.Reservation.Mvc.Entities;
using Shsict.Reservation.Mvc.Entities.Viewer;
using Shsict.Reservation.Mvc.Models;
using Shsict.Reservation.Mvc.Services;

namespace Shsict.Reservation.Mvc.Controllers
{
    [Authorize]
    public class ConsoleController : Controller
    {
        private readonly IRepository _repo = new Repository();
        private readonly UserDto _authorizedUser = new AuthorizeManager().GetSession();

        // GET: Console
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }


        // GET: Console/MenuManagement

        public ActionResult MenuManagement(string date)
        {
            var model = new ConsoleModels.MenuManagementDto();

            var list = Entities.Menu.Cache.MenuListActive;

            DateTime menuDate;

            if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out menuDate))
            {
                list = list.FindAll(x => x.MenuDate == menuDate);

                model.MenuDate = menuDate;
            }

            if (list != null && list.Count > 0)
            {
                var mapper = MenuDto.ConfigMapper().CreateMapper();

                model.Menus = mapper.Map<IEnumerable<MenuDto>>(list.AsEnumerable()).ToList();
            }

            return View(model);
        }


        // GET: Console/Menu

        public ActionResult Menu(int id = 0)
        {
            var model = new MenuDto();

            if (id > 0)
            {
                var menu = _repo.Single<Menu>(id);

                if (menu != null)
                {
                    model = menu.MapTo<Menu, MenuDto>();

                    model.Name = menu.MenuType.ToString();
                    model.Flag = menu.MenuFlag;

                    // 计算该菜单已订餐的有效订单数量，如有则不能修改
                    // ReSharper disable once RedundantBoolCompare
                    model.OrderCount = _repo.Count<Order>(x => x.MenuID == id && x.IsActive == true);
                }
            }
            else
            {
                model.ID = id;
            }

            return View(model);
        }


        // POST: /Menu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Menu(MenuDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // 菜单时间在今天之前的菜单不可更新
                    if (model.MenuDate < DateTime.Today)
                    {
                        ModelState.AddModelError("Error", "此菜单日期早于今天，无法添加或修改");

                        return View(model);
                    }

                    // 如存在同天且相同午餐或晚餐的同套餐则提示重复
                    if (model.ID <= 0 && _repo.Any<Menu>(x =>
                        x.MenuDate == model.MenuDate.Value &&
                        x.MenuType == (MenuTypeEnum)Enum.Parse(typeof(MenuTypeEnum), model.Name) &&
                        // ReSharper disable once RedundantBoolCompare
                        // Shsict.Core.ConditionBuilder
                        x.MenuFlag == model.Flag && x.IsActive == true))
                    {
                        ModelState.AddModelError("Error", "已存在同一时间段的重复套餐，无法添加");

                        return View(model);
                    }

                    // 如该菜单已有订单，则不能修改，只能删除
                    // ReSharper disable once RedundantBoolCompare
                    // Shsict.Core.ConditionBuilder
                    if (_repo.Any<Order>(x => x.MenuID == model.ID && x.IsActive == true))
                    {
                        ModelState.AddModelError("Error", "此菜单已存在订餐申请，无法修改");

                        return View(model);
                    }

                    var menu = _repo.Single<Menu>(model.ID);

                    if (menu != null)
                    {
                        menu = model.MapTo(menu);
                    }
                    else
                    {
                        menu = model.MapTo<MenuDto, Menu>();
                        menu.CreateUser = _authorizedUser.UserId;
                        menu.CreateTime = DateTime.Now;
                        menu.IsActive = true;
                        menu.Remark = string.Empty;
                    }

                    menu.MenuType = (MenuTypeEnum)Enum.Parse(typeof(MenuTypeEnum), model.Name);
                    menu.MenuFlag = model.Flag;

                    // 更新菜单信息
                    object key;

                    _repo.Save(menu, out key);

                    Entities.Menu.Cache.RefreshCache();

                    ModelState.AddModelError("Success", "保存成功");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Warn", ex.Message);
                }
            }

            return View(model);
        }


        // Post: /MenuDelete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MenuDelete(MenuDto model)
        {
            if (ModelState.IsValid)
            {
                using (var trans = DapperHelper.MarsConnection.BeginTransaction())
                {
                    try
                    {
                        if (model.ID > 0 && _repo.Delete<Menu>(model.ID, trans) > 0)
                        {
                            var list = _repo.Query<Order>(x => x.MenuID == model.ID, trans);

                            if (list != null && list.Count > 0)
                            {
                                foreach (var o in list)
                                {
                                    o.IsActive = false;
                                }
                            }

                            list.Update(trans);

                            trans.Commit();

                            Entities.Menu.Cache.RefreshCache();

                            return RedirectToAction("MenuManagement", "Console");
                        }
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();

                        ModelState.AddModelError("Warn", ex.Message);
                    }
                }
            }

            return RedirectToAction("Menu", "Console", new { model.ID });
        }


        // GET: Console/OrderManagement

        public ActionResult OrderManagement(string date)
        {
            var model = new ConsoleModels.OrderManagementDto();

            IViewerFactory<OrderView> factory = new OrderViewFactory();

            DateTime menuDate;
            List<OrderView> list;

            if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out menuDate))
            {
                var criteria = new Criteria
                {
                    WhereClause = $"MenuDate = '{menuDate}'"
                };

                list = factory.Query(criteria);

                model.MenuDate = menuDate;
            }
            else
            {
                list = factory.All();
            }

            if (list != null && list.Count > 0)
            {
                var mapper = OrderDto.ConfigMapper().CreateMapper();

                model.Orders = mapper.Map<IEnumerable<OrderDto>>(list.AsEnumerable()).ToList();
            }

            model.DeliveryZones = Delivery.Cache.DeliveryZoneList;

            return View(model);
        }


        // GET: Console/Order

        public ActionResult Order(int id = 0)
        {
            var model = new OrderDto();

            if (id > 0)
            {
                var order = _repo.Single<Order>(id);

                if (order != null)
                {
                    model = order.MapTo<Order, OrderDto>();
                }
            }
            else
            {
                model.ID = id;
            }

            return View(model);
        }



        // GET: Console/UserManagement

        public ActionResult UserManagement()
        {
            var model = new ConsoleModels.UserManagementDto();

            var list = _repo.All<User>().FindAll(x => x.IsActive);

            if (list.Count > 0)
            {
                var users = new List<UserDto>();

                foreach (var user in list)
                {
                    var u = user.MapTo<User, UserDto>();
                    u.UserId = user.UserName;

                    users.Add(u);
                }

                model.Users = users;
            }

            return View(model);
        }


        // GET: Console/ConfigManagement

        public ActionResult ConfigManagement()
        {
            var model = new ConsoleModels.ConfigManagementDto
            {
                Configs = _repo.All<Config>().FindAll(x => 
                    x.ConfigSystemInfo == ConfigSystem.Reservation && !x.ConfigKey.Contains("Assembly"))
            };

            return View(model);
        }
    }
}