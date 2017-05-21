using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Shsict.Core;
using Shsict.Reservation.Mvc.Entities;
using Shsict.Reservation.Mvc.Entities.Relation;
using Shsict.Reservation.Mvc.Entities.Viewer;
using Shsict.Reservation.Mvc.Filter;
using Shsict.Reservation.Mvc.Models;
using Shsict.Reservation.Mvc.Services;
using Menu = Shsict.Reservation.Mvc.Entities.Menu;

namespace Shsict.Reservation.Mvc.Controllers
{
    [Authorize]
    public class ReservationController : Controller
    {
        private readonly IRepository _repo = new Repository();
        private readonly UserDto _authorizedUser = new AuthorizeManager().GetSession();

        // GET: Reservation

        public ActionResult Index()
        {
            var model = new ReservationModels.IndexDto();

            var menuType = GetMenuLunchOrSupper();

            var menuA = Menu.Cache.MenuListActiveToday.Find(x =>
                x.MenuType == menuType && x.MenuFlag == "A");
            var menuB = Menu.Cache.MenuListActiveToday.Find(x =>
                x.MenuType == menuType && x.MenuFlag == "B");

            var mapper = MenuDto.ConfigMapper().CreateMapper();

            if (menuA != null)
            {
                model.MenuA = mapper.Map<MenuDto>(menuA);
            }

            if (menuB != null)
            {
                model.MenuB = mapper.Map<MenuDto>(menuB);
            }

            // 可以订餐，无相关订餐历史记录
            if (menuA != null && menuB != null && CanReserveNow(new[] { menuA.ID, menuB.ID }))
            {
                model.MenuDate = DateTime.Today;
                model.DeliveryZones = Delivery.Cache.DeliveryZoneList;

                // 设置当前用户的默认送餐区域，先判断用户的职务，再判断用户的班组
                if (_authorizedUser != null)
                {
                    var rela = _repo.Single<RelationTeamPositionDelivery>(x => x.Position == _authorizedUser.Position) ??
                               _repo.Single<RelationTeamPositionDelivery>(x => x.Team == _authorizedUser.Team);

                    if (rela != null)
                    {
                        model.MyDefaultDeliveryZone = Delivery.Cache.Load(rela.DeliveryGuid);
                    }
                }

                if (menuType.Equals(MenuTypeEnum.Lunch))
                {
                    model.MenuStyle = "green";
                }
                else if (menuType.Equals(MenuTypeEnum.Supper))
                {
                    model.MenuStyle = "purple";
                }

                model.CanReserveNow = true;
            }
            else
            {
                model.CanReserveNow = false;
            }

            // 获取今天最新订餐记录
            if (menuA != null && menuB != null)
            {
                model.MyCurrentOrder = GetMyCurrentOrder(new[] { menuA.ID, menuB.ID });
            }

            return View(model);
        }

        // AJAX JsonResult
        // GET: Reservation/GetDeliveryPointByZone?zid=911954b1-daf9-48a9-b9b7-f16afec50dde
        public JsonResult GetDeliveryPointByZone(Guid zid)
        {
            var list = Delivery.Cache.GetDeliveryPointsByZone(zid);

            return Json(list, JsonRequestBehavior.AllowGet);
        }


        // POST: Reservation/MenuOrder

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MenuOrder(ReservationModels.MenuOrderDto model)
        {
            if (ModelState.IsValid && _authorizedUser != null && CanReserveNow(new[] { model.MenuID }))
            {
                try
                {
                    var o = new Order
                    {
                        UserGuid = _authorizedUser.ID,
                        UserName = _authorizedUser.EmployeeName,
                        EmployeeNo = _authorizedUser.EmployeeNo,
                        MenuID = model.MenuID,
                        DeliveryGuid = model.DeliveryPoint,
                        StapleFood = (StapleFoodEnum)Enum.Parse(typeof(StapleFoodEnum), model.StapleFood),
                        ExtraFood = !string.IsNullOrEmpty(model.ExtraFood),
                        CreateTime = DateTime.Now,
                        CreateUser = _authorizedUser.UserId,
                        IsActive = true,
                        Remark = string.Empty
                    };

                    _repo.Insert(o);

                    return RedirectToAction("Index", "Reservation");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Warn", ex.Message);
                }
            }

            return RedirectToAction("Index", "Reservation");
        }


        // GET: Reservation/History

        public ActionResult History()
        {
            var model = new ReservationModels.HistoryDto
            {
                MenuDate = DateTime.Today,
                // 设置当前是否在订餐时间内，不判断是否有菜单
                CanReserveNow = GetMenuLunchOrSupper() != MenuTypeEnum.None
            };


            // 获取所有历史订单 （7天内）
            IViewerFactory<OrderView> factory = new OrderViewFactory();

            var criteria = new Criteria
            {
                WhereClause = $@"(PlaceTime >= '{DateTime.Today.AddDays(-7)}')  AND 
                        UserGuid = '{_authorizedUser.ID}'",
                PagingSize = 0
            };

            var query = factory.Query(criteria);

            if (query.Any())
            {
                var mapper = OrderDto.ConfigMapper().CreateMapper();

                model.MyHistroyOrders = mapper.Map<IEnumerable<OrderDto>>(query.AsEnumerable()).ToList();

                // 今天的午餐订单
                model.MyOrderLunch = model.MyHistroyOrders.Find(x =>
                    x.MenuDate == model.MenuDate && x.MenuType == MenuTypeEnum.Lunch);

                // 设置可取消
                if (model.MyOrderLunch != null && CanCancelNow(model.MyOrderLunch.MenuID))
                { model.MyOrderLunch.CanCancelNow = true; }

                // 今天的夜宵订单
                model.MyOrderSupper = model.MyHistroyOrders.Find(x =>
                    x.MenuDate == model.MenuDate && x.MenuType == MenuTypeEnum.Supper);

                // 设置可取消
                if (model.MyOrderSupper != null && CanCancelNow(model.MyOrderSupper.MenuID))
                { model.MyOrderSupper.CanCancelNow = true; }
            }

            return View(model);
        }


        // GET: Reservation/Cancel/id

        public ActionResult Cancel(int id)
        {
            var o = _repo.Single<Order>(id);

            // 有对应订单，并可取消，且是当前用户的订单
            if (o != null && CanCancelNow(o.MenuID) && o.UserGuid == _authorizedUser.ID)
            {
                o.IsActive = false;
                _repo.Update(o);
            }

            return RedirectToAction("History", "Reservation");
        }


        // GET: Reservation/TodayOrders
        [ManagerRole]
        public ActionResult TodayOrders()
        {
            var model = new ReservationModels.TodayOrdersDto();

            IViewerFactory<OrderView> factory = new OrderViewFactory();

            var list = factory.Query(new Criteria(new { MenuDate = DateTime.Today }));

            if (list != null && list.Count > 0)
            {
                var mapper = OrderDto.ConfigMapper().CreateMapper();

                model.Orders = mapper.Map<IEnumerable<OrderDto>>(list.AsEnumerable()).ToList();
            }

            model.DeliveryZones = Delivery.Cache.DeliveryZoneList;

            // 查找今日的菜单
            model.IsMenuApproved = Menu.Cache.MenuListActiveToday.Exists(x => x.IsApproved);

            return View(model);
        }

        // AJAX JsonResult
        // GET: Reservation/BatchOrdersDelete?ids=172,171
        [HttpPost]
        [ManagerRole]
        public JsonResult BatchOrdersDelete(int[] ids)
        {
            foreach (var id in ids)
            {
                var order = _repo.Single<Order>(id);

                order.IsActive = false;

                _repo.Update(order);
            }

            return Json(ids.Length);
        }

        // GET: Reservation/Order
        [ManagerRole]
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


        // POST: Reservation/Order
        [HttpPost]
        [ManagerRole]
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
                        // 判断补登记的订餐是否重复
                        // 判断登记的用户订餐记录中是否有对应表单中选择的菜单ID
                        var todayMenus = _repo.Query<Menu>(x => x.MenuDate == model.MenuDate &&
                                                       x.MenuType == (MenuTypeEnum)Enum.Parse(typeof(MenuTypeEnum), model.MenuName) &&
                                                       // ReSharper disable once RedundantBoolCompare
                                                       // Shsict.Core.ConditionBuilder
                                                       x.IsActive == true);

                        if (_repo.Query<Order>(x => x.UserGuid == new Guid(model.UserName))
                            .FindAll(x => x.IsActive).Any(x => todayMenus.Any(m => m.ID == x.MenuID)))
                        {
                            ModelState.AddModelError("Error", "所选用户已经成功预订了此时段套餐，无法重复添加订餐");

                            return View(model);
                        }

                        // 新增订餐记录
                        order = model.MapTo<OrderDto, Order>();
                        order.IsActive = true;
                        order.Remark = "大班长补订";
                    }
                    else
                    {
                        // 修改订餐记录
                        order.Remark = "大班长修改";
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


        // Post: Reservation/OrderDelete
        [HttpPost]
        [ManagerRole]
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

                            return RedirectToAction("TodayOrders", "Reservation");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Warn", ex.Message);
                }
            }

            return RedirectToAction("Order", "Reservation", new { model.ID });
        }

        // AJAX JsonResult
        // Post: Reservation/ApproveTodayMenus
        [HttpPost]
        [ManagerRole]
        public ActionResult ApproveTodayMenus()
        {
            var menus = Menu.Cache.MenuListActiveToday;

            if (menus != null && menus.Count > 0)
            {
                foreach (var m in menus)
                {
                    m.IsApproved = true;
                    _repo.Update(m);
                }

                Menu.Cache.RefreshCache();
            }

            return Json(menus?.Count ?? 0);
        }

        // GET: Reservation/ExportOrders
        [ManagerRole]
        public ActionResult ExportOrders()
        {
            IViewerFactory<OrderView> factory = new OrderViewFactory();

            var list = factory.Query(new Criteria(new { MenuDate = DateTime.Today }));

            if (list != null && list.Count > 0)
            {
                var mapper = OrderDto.ConfigMapper().CreateMapper();
                var orders = mapper.Map<IEnumerable<OrderDto>>(list.AsEnumerable()).ToList();

                var book = ExcelManager.BuildOrderWorkbook(orders);

                byte[] file;
                using (var ms = new MemoryStream())
                {
                    book.Write(ms);
                    file = ms.GetBuffer();
                }

                return File(file, "application/vnd.ms-excel", $@"今日订餐记录表-{DateTime.Today.ToString("yyyyMMdd")}.xls");

                //return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                //    $"今日订餐记录表-{DateTime.Today.ToString("yyyyMMdd")}.xlsx");
            }

            return RedirectToAction("TodayOrders", "Reservation");
        }


        private MenuTypeEnum GetMenuLunchOrSupper(int deadlineOffset = 0)
        {
            if (DateTime.Now.Hour >= ConfigGlobal.MenuDuration[0]
                && DateTime.Now.Hour < ConfigGlobal.MenuDuration[1] + deadlineOffset)
            {
                return MenuTypeEnum.Lunch;
            }
            else if (DateTime.Now.Hour >= ConfigGlobal.MenuDuration[2]
                && DateTime.Now.Hour < ConfigGlobal.MenuDuration[3] + deadlineOffset)
            {
                return MenuTypeEnum.Supper;
            }
            else
            {
                return MenuTypeEnum.None;
            }
        }

        private bool CanCancelNow(int id)
        {
            var menuType = GetMenuLunchOrSupper();

            if (menuType != MenuTypeEnum.None)
            {
                // 判断是否今天当前订餐时间段里菜单，是可取消，不是则不可取消
                var todayMenus = _repo.Query<Menu>(x => x.MenuDate == DateTime.Today && x.MenuType == menuType);

                if (todayMenus.Count > 0)
                {
                    return todayMenus.Exists(x => x.ID == id);
                }
            }

            return false;
        }

        private bool CanReserveNow(int[] ids)
        {
            if (GetMenuLunchOrSupper() != MenuTypeEnum.None)
            {
                // 已经预订了对应的套餐
                return !_repo.Query<Order>(x => x.UserGuid == _authorizedUser.ID).FindAll(x => x.IsActive)
                    .Any(x => ids.Any(id => id == x.MenuID));
            }

            return false;
        }

        private OrderDto GetMyCurrentOrder(int[] ids)
        {
            if (GetMenuLunchOrSupper() != MenuTypeEnum.None)
            {
                // 获得预订了对应的套餐
                var factory = new OrderViewFactory();

                var critria = new Criteria
                {
                    WhereClause = $"UserGuid = '{_authorizedUser.ID}'",
                    OrderClause = "PlaceTime DESC",
                    PagingSize = 0
                };

                var order = factory.Query(critria).Find(x => ids.Any(id => id == x.Menu.ID));

                if (order != null)
                {
                    var mapper = OrderDto.ConfigMapper().CreateMapper();

                    return mapper.Map<OrderDto>(order);
                }
            }

            return null;
        }
    }
}