using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Shsict.Core;
using Shsict.Reservation.Mvc.Entities;
using Shsict.Reservation.Mvc.Entities.Relation;
using Shsict.Reservation.Mvc.Entities.Viewer;
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

                // 设置当前用户的默认送餐区域
                if (_authorizedUser != null)
                {
                    var rela = _repo.Single<RelationTeamPositionDelivery>(x =>
                        x.Team == _authorizedUser.Team || x.Position == _authorizedUser.Position);

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
                model.MyCurrentOrder = GetMyCurrentOrder(new[] {menuA.ID, menuB.ID});
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
            var model = new ReservationModels.HistoryDto { MenuDate = DateTime.Today };

            // 获取所有历史订单 （7天内）
            IViewerFactory<OrderView> factory = new OrderViewFactory();

            var criteria = new Criteria
            {
                WhereClause = $@"(PlaceTime >= '{DateTime.Today.AddDays(-7)}')  AND 
                        UserGuid = '{_authorizedUser.ID}'"
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
                    OrderClause = "PlaceTime DESC"
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