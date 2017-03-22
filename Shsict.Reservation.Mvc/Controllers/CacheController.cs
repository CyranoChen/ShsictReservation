using System.Web.Mvc;
using Shsict.Reservation.Mvc.Entities;

namespace Shsict.Reservation.Mvc.Controllers
{
    public class CacheController : Controller
    {
        // GET: Cache/MenuRefresh
        public ActionResult MenuRefresh()
        {
            Menu.Cache.RefreshCache();

            return RedirectToAction("MenuManagement", "Console");
        }
    }
}