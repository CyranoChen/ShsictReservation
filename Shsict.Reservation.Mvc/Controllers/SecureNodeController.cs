using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Shsict.Core;
using Shsict.Core.Dapper;
using Shsict.Reservation.Mvc.Entities;
using Shsict.Reservation.Mvc.Entities.Relation;
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
        public ActionResult Index(string date)
        {
            var model = new SecureNodeModels.IndexDto();

            using (IRepository repo = new Repository())
            {
                var relations = repo.Query<RelationUserOperationStandard>(x => x.UserGuid == _authorizedUser.ID);

                if (relations.Count > 0)
                {
                    var secureNodes = OperationStandard.Cache.OperationStandardList
                        .FindAll(x => relations.Exists(rela => rela.OperationStandardId.Equals(x.ID)));

                    var mapper = OperationStandardDto.ConfigMapper().CreateMapper();

                    model.SecureNodes = mapper.Map<IEnumerable<OperationStandardDto>>(secureNodes.AsEnumerable())
                        .ToList();
                }

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


        // AJAX JsonResult
        // POST: SecureNode/CheckList
        [HttpPost]
        public JsonResult CheckList(CheckListDto model, int secureNodeId)
        {
            if (model != null && secureNodeId > 0 && model.CheckNodePoint > 0
                              && !string.IsNullOrEmpty(model.CheckLocation) && model.CheckTime > DateTime.MinValue)
            {
                try
                {
                    var cl = new CheckList
                    {
                        SecureNodeId = secureNodeId,
                        // 如果在0~7点，属于前一天的晚班
                        OperateDate = model.CheckTime.Hour >= 0 &&
                                      model.CheckTime.Hour < ConfigGlobalSecureNode.ShiftDuration[0]
                            ? model.CheckTime.Date.AddDays(-1) : model.CheckTime.Date,
                        Shift = model.CheckTime.Hour >= ConfigGlobalSecureNode.ShiftDuration[0] &&
                                model.CheckTime.Hour <= ConfigGlobalSecureNode.ShiftDuration[1]
                            ? "daytime" : "night",
                        CheckTime = model.CheckTime,
                        CheckLocation = model.CheckLocation.Trim(),
                        CheckNodePoint = model.CheckNodePoint,
                        CheckResult = model.CheckResult,
                        UserGuid = _authorizedUser.ID,
                        EmployeeName = _authorizedUser.EmployeeName,
                        EmployeeNo = _authorizedUser.EmployeeNo,
                        IsActive = true,
                        Remark = !model.CheckResult ? model.Remark : string.Empty
                    };

                    using (IRepository repo = new Repository())
                    {
                        repo.Insert(cl);
                    }

                    return Json("success");
                }
                catch (Exception ex)
                {
                    return Json(ex.Message);
                }
            }

            return Json("failed");
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

        public ActionResult ExportCheckLists(DateTime date)
        {
            using (IRepository repo = new Repository())
            {
                var list = repo.Query<CheckList>(x => x.OperateDate >= date.AddDays(-2) && x.OperateDate <= date.AddDays(2))
                    .FindAll(x => x.OperateDate.Date == date.Date && x.IsActive);

                if (list.Count > 0)
                {
                    var mapper = CheckListDto.ConfigMapper().CreateMapper();
                    var checklists = mapper.Map<IEnumerable<CheckListDto>>(list.AsEnumerable()).ToList();

                    var book = ExcelManager.BuildCheckListWorkbook(checklists, date);

                    byte[] file;
                    using (var ms = new MemoryStream())
                    {
                        book.Write(ms);
                        file = ms.GetBuffer();
                    }

                    return File(file, "application/vnd.ms-excel", $@"今日安全检查表-{date:yyyyMMdd}.xls");
                }
            }

            return RedirectToAction("CheckListManagement", "SecureNode", new { date });
        }

    }
}
