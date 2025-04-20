using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLTTDT.Data;
using QLTTDT.ViewModels;

namespace QLTTDT.Controllers
{
    public class AllTopicController : Controller
    {
        private QLTTDTDbContext _context;
        public AllTopicController(QLTTDTDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            int? userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString());
            var topicCards = await _context.ChuDes
                .Select(i => new TopicCard
                {
                    MaChuDe = i.MaChuDe,
                    TenChuDe = i.TenChuDe,
                    MoTa = i.MoTa,
                    UrlAnhChuDe = i.UrlAnhChuDe,
                    SoKhoaHocDaDangKi = (userId == null) ? 0 : _context.DangKiKhoaHocs
                    .Include(j => j.MaKhoaHocNavigation)
                    .Count(j => j.MaKhoaHocNavigation.MaChuDe == i.MaChuDe && j.MaHocVien == userId),
                })
                .ToListAsync();
            return View(topicCards);
        }
        public async Task<IActionResult> Details(string topicSlug, int? topicId)
        {
            if (topicId == null)
            {
                return NotFound();
            }
            var chuDe = await _context.ChuDes
                .FirstOrDefaultAsync(m => m.MaChuDe == topicId);
            if (chuDe == null)
            {
                return NotFound();
            }
            int? userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString());
            var topicDesc = new TopicDescription
            {
                MaChuDe = chuDe.MaChuDe,
                TenChuDe = chuDe.TenChuDe,
                MoTa = chuDe.MoTa,
                UrlAnhChuDe = chuDe.UrlAnhChuDe,
                DanhSachKhoaHoc = await _context.KhoaHocs
                .Include(i => i.MaCapDoNavigation)
                .Where(i => i.MaChuDe == chuDe.MaChuDe)
                .Select(i => new CourseCard
                {
                    MaKhoaHoc = i.MaKhoaHoc,
                    TenKhoaHoc = i.TenKhoaHoc,
                    CapDo = i.MaCapDoNavigation.TenCapDo,
                    MoTa = i.MoTa,
                    UrlAnhKhoaHoc = i.UrlAnh,
                    DaDangKi = (userId == null) ? false : _context.DangKiKhoaHocs
                    .Any(j => j.MaKhoaHoc == i.MaKhoaHoc && j.MaHocVien == userId),
                })
                .ToListAsync(),
                SoKhoaHocDaDangKi = (userId == null) ? 0 : _context.DangKiKhoaHocs
                .Count(j => j.MaKhoaHocNavigation.MaChuDe == chuDe.MaChuDe && j.MaHocVien == userId)
            };
            return View(topicDesc);
        }
    }
}
