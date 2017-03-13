using System;
using System.Web.Mvc;
using Shsict.Core;
using Shsict.Reservation.Mvc.Entities;
using Shsict.Reservation.Mvc.Models;
using Menu = Shsict.Reservation.Mvc.Entities.Menu;

namespace Shsict.Reservation.Mvc.Controllers
{
    [Authorize]
    public class ReservationController : Controller
    {
        private readonly IRepository _repo = new Repository();

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

            model.MenuDate = DateTime.Today;

            return View(model);
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
            if (DateTime.Now.Hour >= 7 && DateTime.Now.Hour < 9)
            {
                return MenuTypeEnum.Lunch;
            }
            else if (DateTime.Now.Hour >= 18 && DateTime.Now.Hour < 20)
            {
                return MenuTypeEnum.Supper;
            }
            else
            {
                return MenuTypeEnum.None;
            }
        }
    }
}