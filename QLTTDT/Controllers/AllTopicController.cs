using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLTTDT.Data;
using QLTTDT.Services;
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
        public async Task<IActionResult> Index(string? searchString)
        {
            int? userId = GetUserId();
            var tmp = _context.ChuDes.Select(i => i);
            if (!String.IsNullOrEmpty(searchString))
            {
                tmp = tmp.Where(i => i.TenChuDe.ToUpper()
                .Contains(searchString.ToUpper()));
            }
            var topicCards = tmp.Select(i => 
            new TopicCard {
                MaChuDe = i.MaChuDe,
                TenChuDe = i.TenChuDe,
                MoTa = i.MoTa,
                UrlAnhChuDe = i.UrlAnhChuDe,
                SoKhoaHocDaDangKi = (userId == null) ? 0 : _context.DangKiKhoaHocs
                .Include(j => j.MaKhoaHocNavigation)
                .Count(j => j.MaKhoaHocNavigation.MaChuDe == i.MaChuDe && j.MaHocVien == userId),
            });
            return View(await topicCards.ToListAsync());
        }
        public async Task<IActionResult> Details(string? topicSlug, int? topicId, string? searchString)
        {
            var slugCheck = new SlugCheck(_context);
            if(!await slugCheck.CheckTopicSlug(topicSlug, topicId))
            {
                return NotFound();
            }
            int? userId = GetUserId();
            var chuDe = await _context.ChuDes.FirstOrDefaultAsync(m => m.MaChuDe == topicId);
            if (chuDe == null)
            {
                return NotFound();
            }
            var khoaHocs = _context.KhoaHocs
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
                });

            if(!String.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToUpper();
                khoaHocs = khoaHocs.Where(i => i.TenKhoaHoc.ToUpper().Contains(searchString)
                || i.CapDo.ToUpper().Contains(searchString));
            }

            var topicDesc = new TopicDescription
            {
                MaChuDe = chuDe.MaChuDe,
                TenChuDe = chuDe.TenChuDe,
                MoTa = chuDe.MoTa,
                UrlAnhChuDe = chuDe.UrlAnhChuDe,
                DanhSachKhoaHoc = await khoaHocs.ToListAsync(),
                SoKhoaHocDaDangKi = (userId == null) ? 0 : _context.DangKiKhoaHocs
                .Count(j => j.MaKhoaHocNavigation.MaChuDe == chuDe.MaChuDe && j.MaHocVien == userId)
            };

            return View(topicDesc);
        }
        private int? GetUserId()
        {
            return Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString());
        }
    }
}
