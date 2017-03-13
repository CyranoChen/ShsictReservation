using System.Web.Mvc;

namespace Shsict.Reservation.Mvc.Controllers
{
    [Authorize]
    public class ReservationController : Controller
    {

        // GET: Reservation
        public ActionResult Index()
        {
            return View();
        }
    }
}