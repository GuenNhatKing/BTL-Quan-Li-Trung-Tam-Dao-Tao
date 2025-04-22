using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLTTDT.Data;
using QLTTDT.ViewModels;

namespace QLTTDT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("Admin")]
    public class DashboardController : Controller
    {
        private QLTTDTDbContext _context;
        public DashboardController(QLTTDTDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var statistics = await _context.DangKiKhoaHocs
                .Include(i => i.MaKhoaHocNavigation)
                .GroupBy(i => new
                {
                    MaKhoaHoc = i.MaKhoaHoc,
                    TenKhoaHoc = i.MaKhoaHocNavigation.TenKhoaHoc
                })
                .OrderBy(i => i.Key.MaKhoaHoc)
                .Select(i => new Statistics
                {
                    MaKhoaHoc = i.Key.MaKhoaHoc,
                    TenKhoaHoc = i.Key.TenKhoaHoc,
                    SoHocVienDangKi = i.Count(i => true),
                })
                .ToListAsync();
            return View(statistics);
        }
        public async Task<IActionResult> Statistics(string searchString)
        {
            var statistics = _context.DangKiKhoaHocs
                .Include(i => i.MaKhoaHocNavigation)
                .GroupBy(i => new
                {
                    ThoiGian = DateOnly.FromDateTime(i.ThoiGianDangKi),
                    MaKhoaHoc = i.MaKhoaHoc,
                    TenKhoaHoc = i.MaKhoaHocNavigation.TenKhoaHoc
                })
                .OrderBy(i => i.Key.MaKhoaHoc)
                .ThenBy(i => i.Key.ThoiGian)
                .Select(i => new Statistics
                {
                    MaKhoaHoc = i.Key.MaKhoaHoc,
                    TenKhoaHoc = i.Key.TenKhoaHoc,
                    ThoiGian = i.Key.ThoiGian,
                    ThoiGianStr = i.Key.ThoiGian.Day.ToString() + "/" + i.Key.ThoiGian.Month.ToString() + "/" + i.Key.ThoiGian.Year.ToString(),
                    SoHocVienDangKi = i.Count(i => true),
                    DoanhThu = i.Sum(i => i.HocPhi),
                });

            if(!String.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToUpper();
                statistics = statistics.Where(i => i.TenKhoaHoc.ToUpper().Contains(searchString)
                || i.ThoiGianStr.ToUpper().Contains(searchString.ToString()));
            }
            return View(await statistics.ToListAsync());
        }
    }
}
