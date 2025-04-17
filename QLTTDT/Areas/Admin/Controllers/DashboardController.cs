using Microsoft.AspNetCore.Mvc;

namespace QLTTDT.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Statistics()
        {
            return View();
        }
    }
}
