using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Shsict.Core;
using Shsict.Reservation.Mvc.Entities;
using Shsict.Reservation.Mvc.Entities.Viewer;
using Shsict.Reservation.Mvc.Filter;
using Shsict.Reservation.Mvc.Models;
using Shsict.Reservation.Mvc.Services;

namespace Shsict.Reservation.Mvc.Controllers
{
    [Authorize]
    [CanteenRole]
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


        // POST: Console/Menu
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


        // Post: Console/MenuDelete
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
                var factory = new OrderViewFactory();

                var order = factory.Single(id);

                if (order != null)
                {
                    var mapper = OrderDto.ConfigMapper().CreateMapper();

                    model = mapper.Map<OrderDto>(order);

                    model.MenuName = order.Menu.MenuType.ToString();
                    model.Flag = order.Menu.MenuFlag;
                    model.StapleFood = order.StapleFood.ToString();

                    if (model.DeliveryGuid.HasValue)
                    {
                        model.DeliveryPoint = model.DeliveryGuid.Value.ToString();
                        model.DeliveryZone = Delivery.Cache.GetParentZone(model.DeliveryGuid.Value).ID.ToString();
                    }
                }
            }
            else
            {
                model.ID = id;
            }

            return View(model);
        }


        // POST: Console/Order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Order(OrderDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // 选择日期的对应类型与套餐是否存在
                    var menu = _repo.Single<Menu>(x => x.MenuDate == model.MenuDate &&
                                                       x.MenuType == (MenuTypeEnum)Enum.Parse(typeof(MenuTypeEnum), model.MenuName) &&
                                                       // ReSharper disable once RedundantBoolCompare
                                                       // Shsict.Core.ConditionBuilder
                                                       x.MenuFlag == model.Flag && x.IsActive == true);

                    if (menu == null)
                    {
                        ModelState.AddModelError("Error", "所选日期不存在对应类型的套餐，无法添加或修改");

                        return View(model);
                    }

                    var order = _repo.Single<Order>(model.ID);

                    if (order == null)
                    {
                        // 新增订餐记录
                        order = model.MapTo<OrderDto, Order>();
                        order.IsActive = true;
                        order.Remark = "后台补订";
                    }
                    else
                    {
                        // 修改订餐记录
                        order.Remark = "后台修改";
                    }

                    #region 设置用餐人信息 在UserName中传递UserGuid
                    model.UserGuid = new Guid(model.UserName);

                    var user = _repo.Single<User>(model.UserGuid);

                    if (user != null)
                    {
                        order.UserGuid = user.ID;
                        order.UserName = user.EmployeeName;
                        order.EmployeeNo = user.EmployeeNo;
                    }
                    else
                    {
                        order.UserGuid = Guid.Empty;
                        order.UserName = string.Empty;
                        order.EmployeeNo = string.Empty;
                    }
                    #endregion

                    order.MenuID = menu.ID;
                    order.DeliveryGuid = new Guid(model.DeliveryPoint);
                    order.StapleFood = (StapleFoodEnum)Enum.Parse(typeof(StapleFoodEnum), model.StapleFood);
                    order.ExtraFood = model.ExtraFood;

                    order.CreateUser = _authorizedUser.UserId;
                    order.CreateTime = DateTime.Now;

                    // 更新菜单信息
                    object key;

                    _repo.Save(order, out key);

                    ModelState.AddModelError("Success", "保存成功");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Warn", ex.Message);
                }
            }

            return View(model);
        }


        // Post: Console/OrderDelete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult OrderDelete(OrderDto model)
        {
            if (ModelState.IsValid)
            {

                try
                {
                    if (model.ID > 0)
                    {
                        var order = _repo.Single<Order>(model.ID);

                        if (order != null)
                        {
                            order.IsActive = false;
                            _repo.Update(order);

                            return RedirectToAction("OrderManagement", "Console");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Warn", ex.Message);
                }
            }

            return RedirectToAction("Order", "Console", new { model.ID });
        }


        // GET: Console/UserManagement
        [AdminRole]
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


        // AJAX JsonResult
        // POST:  Console/SyncUser
        [AdminRole]
        [HttpPost]
        public JsonResult SyncUser(string id)
        {
            Guid guid;

            if (!string.IsNullOrEmpty(id) && Guid.TryParse(id, out guid))
            {
                var uw = _repo.Single<UserWeChat>(guid);

                if (uw != null)
                {
                    var auth = new AuthorizeManager();

                    var user = auth.SyncUserWithWeChat(uw.UserName);

                    if (user.ID.Equals(guid))
                    {
                        return Json("Success");
                    }
                }
            }

            return Json("Failed");
        }



        // GET: Console/ConfigManagement
        [AdminRole]
        public ActionResult ConfigManagement()
        {
            var model = new ConsoleModels.ConfigManagementDto
            {
                Configs = _repo.All<Config>().FindAll(x =>
                    x.ConfigSystemInfo == ConfigSystem.Reservation && !x.ConfigKey.Contains("Assembly"))
            };

            return View(model);
        }


        // AJAX JsonResult
        // POST:  Console/Config
        [AdminRole]
        [HttpPost]
        public JsonResult Config(string key, string value)
        {
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
            {
                var config = _repo.Single<Config>(x => x.ConfigKey == key && x.ConfigSystem == ConfigSystem.Reservation.ToString());

                if (config != null)
                {
                    config.ConfigValue = value;

                    config.Save();

                    ConfigGlobal.Refresh();

                    return Json("Success");
                }
            }

            return Json("Failed");
        }
    }
}