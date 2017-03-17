using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Shsict.Core;
using Shsict.Reservation.Mvc.Entities;
using Shsict.Reservation.Mvc.Entities.Viewer;
using Shsict.Reservation.Mvc.Models;

namespace Shsict.Reservation.Mvc.Controllers
{
    public class ConsoleController : Controller
    {
        private readonly IRepository _repo = new Repository();

        // GET: Console
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }


        // GET: Console/Menu

        public ActionResult MenuManagement()
        {
            // TODO
            var list = _repo.All<Menu>().FindAll(x => x.IsActive);

            return View(list);
        }


        // GET: Console/Order

        public ActionResult OrderManagement()
        {
            IViewerFactory<OrderView> factory = new OrderViewFactory();

            // TODO
            var criteria = new Criteria
            {
                WhereClause = $"MenuDate = '{DateTime.Today}'"
            };

            var query = factory.Query(criteria);

            if (query.Any())
            {
                var mapper = OrderDto.ConfigMapper().CreateMapper();

                var result = mapper.Map<IEnumerable<OrderDto>>(query.AsEnumerable()).ToList();

                return View(result);
            }

            return View();
        }


        // GET: Console/UserManage

        public ActionResult UserManagement()
        {
            var model = new ConsoleModels.UserManagementDto();

            var list = _repo.All<User>();

            if (list != null && list.Count > 0)
            {
                model.Users = list.Select(user => user.MapTo<User, UserDto>()).ToList();
            }

            return View(model);
        }
    }
}