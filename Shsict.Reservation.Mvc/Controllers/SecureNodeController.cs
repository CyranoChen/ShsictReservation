using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Shsict.Core;
using Shsict.Core.Dapper;
using Shsict.Reservation.Mvc.Models.SecureNode;
using Shsict.Reservation.Mvc.Entities.SecureNode;
using Shsict.Reservation.Mvc.Models;
using Shsict.Reservation.Mvc.Services;

namespace Shsict.Reservation.Mvc.Controllers
{
    [Authorize]
    public class SecureNodeController : Controller
    {
        private readonly UserDto _authorizedUser = new AuthorizeManager().GetSession();

        // GET: SecureNode
        public ActionResult Index()
        {
            return View();
        }

        // GET: SecureNode/History
        public ActionResult History(string date)
        {
            var model = new SecureNodeModels.HistoryDto();

            using (IRepository repo = new Repository())
            {
                var list = repo.Query<CheckList>(x => x.UserGuid == _authorizedUser.ID);

                if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out var operateDate))
                {
                    list = list.FindAll(x => x.OperateDate.Date == operateDate.Date && x.IsActive);

                    model.OperateDate = operateDate;
                }
                else
                {
                    list = list.FindAll(x => x.OperateDate.Date == DateTime.Today.Date && x.IsActive);

                    model.OperateDate = DateTime.Today;
                }

                if (list.Count > 0)
                {
                    var mapper = CheckListDto.ConfigMapper().CreateMapper();

                    model.MyCheckLists = mapper.Map<IEnumerable<CheckListDto>>(list.AsEnumerable()).ToList();
                }
            }

            return View(model);
        }


        // GET: SecureNode/CheckListManagement
        public ActionResult CheckListManagement(string date)
        {
            var model = new SecureNodeModels.CheckListManagementDto();

            using (IRepository repo = new Repository())
            {
                List<CheckList> list;

                if (!string.IsNullOrEmpty(date) && DateTime.TryParse(date, out var operateDate))
                {
                    list = repo.Query<CheckList>(x => x.OperateDate >= operateDate.AddDays(-2) && x.OperateDate <= operateDate.AddDays(2))
                        .FindAll(x => x.OperateDate.Date == operateDate.Date && x.IsActive);

                    model.OperateDate = operateDate;
                }
                else
                {
                    // 为避免数据量过大，只显示7天内的检查记录
                    list = repo.Query<CheckList>(x => x.OperateDate >= DateTime.Now.AddDays(-7));

                    model.OperateDate = null;
                }

                if (list.Count > 0)
                {
                    var mapper = CheckListDto.ConfigMapper().CreateMapper();

                    model.CheckLists = mapper.Map<IEnumerable<CheckListDto>>(list.AsEnumerable()).ToList();
                }
            }

            return View(model);
        }

        public ActionResult ExportCheckLists(string date)
        {
            // TODO

            return RedirectToAction("CheckListManagement", "SecureNode", new { date });
        }

    }
}
