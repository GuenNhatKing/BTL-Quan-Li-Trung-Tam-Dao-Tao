using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLTTDT.Data;
using QLTTDT.Models;
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
                    SoHocVienDangKi = i.Count(),
                })
                .ToListAsync();
            return View(statistics);
        }
        public async Task<IActionResult> Statistics(string searchString, MyDateType? startTime = null, MyDateType? endTime = null, bool statistic = false)
        {
            DateTypes type = (startTime != null && endTime != null) ? startTime.Type : DateTypes.DATE;
            var dkkh = _context.DangKiKhoaHocs.Select(i => i);
            if (!String.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToUpper();
                dkkh = dkkh.Where(i => i.MaKhoaHocNavigation.TenKhoaHoc.ToUpper().Contains(searchString));
            }
            if (statistic && startTime != null && endTime != null)
            {
                dkkh = dkkh
                .Where(i => i.ThoiGianDangKi >= (DateTime)startTime && i.ThoiGianDangKi < (DateTime)endTime);
            }
            IQueryable<Statistics> statistics;
            if (type == DateTypes.YEAR)
            {
                statistics = dkkh.GroupBy(i => new
                {
                    MaKhoaHoc = i.MaKhoaHoc,
                    TenKhoaHoc = i.MaKhoaHocNavigation.TenKhoaHoc,
                    Year = i.ThoiGianDangKi.Year,
                })
                .OrderBy(i => i.Key.MaKhoaHoc)
                .ThenBy(i => i.Key.Year)
                .Select(i => new Statistics
                {
                    MaKhoaHoc = i.Key.MaKhoaHoc,
                    TenKhoaHoc = i.Key.TenKhoaHoc,
                    ThoiGian = new MyDateType
                    {
                        Year = i.Key.Year,
                        Type = type
                    },
                    SoHocVienDangKi = i.Count(i => true),
                    DoanhThu = i.Sum(i => i.HocPhi),
                });
            }
            else if (type == DateTypes.MONTH)
            {
                statistics = dkkh.GroupBy(i => new
                {
                    MaKhoaHoc = i.MaKhoaHoc,
                    TenKhoaHoc = i.MaKhoaHocNavigation.TenKhoaHoc,
                    Year = i.ThoiGianDangKi.Year,
                    Month = i.ThoiGianDangKi.Month,
                })
                .OrderBy(i => i.Key.MaKhoaHoc)
                .ThenBy(i => i.Key.Year)
                .ThenBy(i => i.Key.Month)
                .Select(i => new Statistics
                {
                    MaKhoaHoc = i.Key.MaKhoaHoc,
                    TenKhoaHoc = i.Key.TenKhoaHoc,
                    ThoiGian = new MyDateType
                    {
                        Year = i.Key.Year,
                        Month = i.Key.Month,
                        Type = type
                    },
                    SoHocVienDangKi = i.Count(i => true),
                    DoanhThu = i.Sum(i => i.HocPhi),
                });
            }
            else
            {
                statistics = dkkh.GroupBy(i => new
                {
                    MaKhoaHoc = i.MaKhoaHoc,
                    TenKhoaHoc = i.MaKhoaHocNavigation.TenKhoaHoc,
                    Year = i.ThoiGianDangKi.Year,
                    Month = i.ThoiGianDangKi.Month,
                    Day = i.ThoiGianDangKi.Day,
                })
                .OrderBy(i => i.Key.MaKhoaHoc)
                .ThenBy(i => i.Key.Year)
                .ThenBy(i => i.Key.Month)
                .ThenBy(i => i.Key.Day)
                .Select(i => new Statistics
                {
                    MaKhoaHoc = i.Key.MaKhoaHoc,
                    TenKhoaHoc = i.Key.TenKhoaHoc,
                    ThoiGian = new MyDateType
                    {
                        Year = i.Key.Year,
                        Month = i.Key.Month,
                        Day = i.Key.Day,
                        Type = type
                    },
                    SoHocVienDangKi = i.Count(i => true),
                    DoanhThu = i.Sum(i => i.HocPhi),
                });
            }
            ViewBag.startTime = (startTime != null) ? ("Thời gian bắt đầu: " + startTime?.ToString()) : null;
            ViewBag.endTime = (endTime != null) ? ("Thời gian kết thúc: " + endTime?.ToString()) : null;
            return View(await statistics.ToListAsync());
        }
    }
}