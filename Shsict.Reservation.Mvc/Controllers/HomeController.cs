using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Shsict.Core;
using Shsict.Reservation.Mvc.Entities;
using Shsict.Reservation.Mvc.Entities.Viewer;
using Shsict.Reservation.Mvc.Filter;
using Shsict.Reservation.Mvc.Models;

namespace Shsict.Reservation.Mvc.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        [CanteenRole]
        public ActionResult Index()
        {
            var model = new ConsoleModels.IndexDto();

            IViewerFactory<ReportView> factory = new ReportViewFactory();

            var list = factory.Query(new Criteria(new
            {
                MenuDate = DateTime.Today,
                MenuType = (int)MenuDto.GetMenuLunchOrSupper(0, false)
            }));

            if (list != null && list.Count > 0)
            {
                var mapper = ReportDto.ConfigMapper().CreateMapper();

                model.Reports = mapper.Map<IEnumerable<ReportDto>>(list.AsEnumerable()).ToList();

                // 查找今日的菜单，判断是否大班长确认过
                var currentMenus = Menu.Cache.MenuListActiveToday.FindAll(x =>
                    x.MenuType.Equals(MenuDto.GetMenuLunchOrSupper(0, false)));

                model.ApproverInfo = currentMenus.Exists(x => x.IsApproved) ?
                    currentMenus[0].Remark : string.Empty;
            }

            return View(model);
        }
    }
}