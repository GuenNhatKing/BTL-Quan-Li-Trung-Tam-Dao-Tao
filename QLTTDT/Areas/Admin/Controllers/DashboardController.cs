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
        public async Task<IActionResult> Statistics()
        {
            var statistics = await _context.DangKiKhoaHocs
                .Include(i => i.MaKhoaHocNavigation)
                .GroupBy(i => new
                {
                    ThoiGian = i.ThoiGianDangKi.HasValue
                    ? DateOnly.FromDateTime(i.ThoiGianDangKi.Value)
                    : (DateOnly?)null,
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
                    SoHocVienDangKi = i.Count(i => true),
                    DoanhThu = i.Sum(i => i.HocPhi),
                })
                .ToListAsync();
            return View(statistics);
        }
    }
}
