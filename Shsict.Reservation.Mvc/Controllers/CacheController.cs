using System.Web.Mvc;
using Shsict.Reservation.Mvc.Entities;
using Shsict.Reservation.Mvc.Entities.SecureNode;

namespace Shsict.Reservation.Mvc.Controllers
{
    [Authorize]
    public class CacheController : Controller
    {
        // GET: Cache/MenuRefresh
        public ActionResult MenuRefresh()
        {
            Menu.Cache.RefreshCache();

            return RedirectToAction("MenuManagement", "Console");
        }

        // GET: Cache/OperationStandardRefresh
        public ActionResult OperationStandardRefresh()
        {
            OperationStandard.Cache.RefreshCache();

            return RedirectToAction("CheckListManagement", "SecureNode");
        }
    }
}