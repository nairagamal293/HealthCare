using Microsoft.AspNetCore.Mvc;

namespace HeathCare.Controllers
{
    public class BookingsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
