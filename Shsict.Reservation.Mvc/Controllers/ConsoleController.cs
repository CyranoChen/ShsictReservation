using System;
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

        public ActionResult MenuManagement()
        {
            // TODO
            var list = _repo.All<Menu>().FindAll(x => x.IsActive);

            return View(list);
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
                    _repo.Save(menu);

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
                try
                {
                    if (model.ID > 0 && _repo.Delete<Menu>(model.ID) > 0)
                    {
                        return RedirectToAction("MenuManagement", "Console");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Warn", ex.Message);
                }
            }

            return RedirectToAction("Menu", "Console", new { model.ID });
        }


        // GET: Console/OrderManagement

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

                var model = mapper.Map<IEnumerable<OrderDto>>(query.AsEnumerable()).ToList();

                return View(model);
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