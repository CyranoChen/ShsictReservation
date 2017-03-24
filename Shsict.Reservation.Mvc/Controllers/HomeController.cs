using System.Web.Mvc;

namespace Shsict.Reservation.Mvc.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
    }
}