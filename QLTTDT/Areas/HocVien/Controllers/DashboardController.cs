using Microsoft.AspNetCore.Mvc;

namespace QLTTDT.Areas.HocVien.Controllers
{
    [Area("HocVien")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Profile()
        {
            return View();
        }
    }
}
