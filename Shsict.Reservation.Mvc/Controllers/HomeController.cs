using System.Web.Mvc;
using Shsict.Reservation.Mvc.Filter;

namespace Shsict.Reservation.Mvc.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        [CanteenRole]
        public ActionResult Index()
        {
            return View();
        }
    }
}