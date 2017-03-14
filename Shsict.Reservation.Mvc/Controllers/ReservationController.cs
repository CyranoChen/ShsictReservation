using System;
using System.Linq;
using System.Web.Mvc;
using Shsict.Core;
using Shsict.Reservation.Mvc.Entities;
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

            var list = _repo.Query<Menu>(x => x.MenuDate == DateTime.Today);

            var menuA = list.Find(x => x.MenuType == GetMenuLunchOrSupper() && x.MenuFlag == "A" && x.IsActive);
            var menuB = list.Find(x => x.MenuType == GetMenuLunchOrSupper() && x.MenuFlag == "B" && x.IsActive);

            if (menuA != null)
            {
                model.MenuA = MapperMenu(menuA);
            }

            if (menuB != null)
            {
                model.MenuB = MapperMenu(menuB);
            }

            // 可以订餐，无相关订餐历史记录
            if (menuA != null && model.MenuB != null && CanReserveNow(new int[2] { menuA.ID, menuB.ID }))
            {
                model.MenuDate = DateTime.Today;

                if (GetMenuLunchOrSupper().Equals(MenuTypeEnum.Lunch))
                {
                    model.MenuStyle = "green";
                }
                else if (GetMenuLunchOrSupper().Equals(MenuTypeEnum.Supper))
                {
                    model.MenuStyle = "purple";
                }

                model.IsReserveNow = true;
            }
            else
            {
                model.IsReserveNow = false;
            }

            return View(model);
        }


        // POST: Reservation/MenuOrder

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MenuOrder(ReservationModels.MenuOrderDto model)
        {
            if (ModelState.IsValid && _authorizedUser != null)
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
            // TODO
            return View();
        }

        private MenuDto MapperMenu(Menu menu)
        {
            if (menu != null)
            {
                var m = menu.MapTo<Menu, MenuDto>();

                if (menu.MenuType == MenuTypeEnum.Lunch)
                {
                    m.Name = "午餐";
                    m.Duration = "7:30~9:00";
                }
                else if (menu.MenuType == MenuTypeEnum.Supper)
                {
                    m.Name = "夜宵";
                    m.Duration = "18:30~20:00";
                }

                m.Flag = $" {menu.MenuFlag} 套餐";

                return m;
            }

            return null;
        }

        private MenuTypeEnum GetMenuLunchOrSupper()
        {
            // TODO
            if (DateTime.Now.Hour >= 7 && DateTime.Now.Hour < 9)
            {
                return MenuTypeEnum.Lunch;
            }
            else if (DateTime.Now.Hour >= 12 && DateTime.Now.Hour < 20)
            {
                return MenuTypeEnum.Supper;
            }
            else
            {
                return MenuTypeEnum.None;
            }
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
    }
}