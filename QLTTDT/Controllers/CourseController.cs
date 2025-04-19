using System.Security.Claims;
using AspNetCoreGeneratedDocument;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLTTDT.Data;
using QLTTDT.Models;
using QLTTDT.Services;
using QLTTDT.ViewModels;

namespace QLTTDT.Controllers
{
    public class CourseController : Controller
    {
        private QLTTDTDbContext _context;
        public CourseController(QLTTDTDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(string topicSlug, int? topicId, string courseSlug, int? courseId)
        {
            if (courseId == null)
            {
                return NotFound();
            }
            var khoaHoc = await _context.KhoaHocs
                .FirstOrDefaultAsync(m => m.MaKhoaHoc == courseId);
            if (khoaHoc == null)
            {
                return NotFound();
            }
            int? userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString());
            var courseDesc = new CourseDescription
            {
                MaKhoaHoc = khoaHoc.MaKhoaHoc,
                TenKhoaHoc = khoaHoc.TenKhoaHoc,
                MaCapDo = khoaHoc.MaCapDo,
                MaChuDe = khoaHoc.MaChuDe,
                MaGiangVien = khoaHoc.MaGiangVien,
                MoTa = khoaHoc.MoTa,
                HocPhi = khoaHoc.HocPhi,
                SoLuongHocVienToiDa = khoaHoc.SoLuongHocVienToiDa,
                ThoiGianKhaiGiang = khoaHoc.ThoiGianKhaiGiang,
                UrlAnh = khoaHoc.UrlAnh,
                TenCapDo = (await _context.CapDos.FirstOrDefaultAsync(i => i.MaCapDo == khoaHoc.MaCapDo))?.TenCapDo,
                TenChuDe = (await _context.ChuDes.FirstOrDefaultAsync(i => i.MaChuDe == khoaHoc.MaChuDe))?.TenChuDe,
                TenGiangVien = (await _context.NguoiDungs.FirstOrDefaultAsync(i => i.MaNguoiDung == khoaHoc.MaGiangVien))?.HoVaTen,
                SoLuongHocVienDangKi = await _context.DangKiKhoaHocs.CountAsync(i => i.MaKhoaHoc == khoaHoc.MaKhoaHoc),
                DaDangKi = (userId == null) ? false : await _context.DangKiKhoaHocs
                .AnyAsync(i => i.MaKhoaHoc == khoaHoc.MaKhoaHoc && i.MaHocVien == userId),
                TienDo = await GetProgress(khoaHoc, userId),
            };
            return View(courseDesc);
        }

        [Authorize("HocVien")]
        [HttpPost]
        public async Task<IActionResult> Index(string topicSlug, int? topicId, string courseSlug, int? courseId, bool noUse)
        {
            if (courseId == null)
            {
                return NotFound();
            }
            var khoaHoc = await _context.KhoaHocs
                .FirstOrDefaultAsync(m => m.MaKhoaHoc == courseId);
            if (khoaHoc == null)
            {
                return NotFound();
            }
            int? userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString());
            var validation = new ValidCheck(_context);
            if (await validation.IsRegisterCourseVaild(userId, courseId))
            {
                var dkkh = await _context.DangKiKhoaHocs.IgnoreQueryFilters().FirstOrDefaultAsync(i => i.MaHocVien == userId && i.MaKhoaHoc == courseId && i.DaHuy == true);
                if(dkkh != null)
                {
                    dkkh.DaHuy = false;
                    dkkh.HocPhi = (await _context.KhoaHocs.FirstOrDefaultAsync(i => i.MaKhoaHoc == courseId))?.HocPhi;
                    dkkh.ThoiGianDangKi = DateTime.Now;
                    try
                    {
                        _context.Update(dkkh);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        throw;
                    }
                }
                else
                {
                    var register = new DangKiKhoaHoc
                    {
                        MaHocVien = (int)userId,
                        MaKhoaHoc = (int)courseId,
                        HocPhi = (await _context.KhoaHocs.FirstOrDefaultAsync(i => i.MaKhoaHoc == courseId))?.HocPhi,
                    };
                    _context.Add(register);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction("Index", new { topicSlug, topicId, courseSlug, courseId });
        }
        private async Task<int> GetProgress(KhoaHoc khoaHoc, int? userId)
        {
            if (userId == null) return 0;
            var dkkh = await _context.DangKiKhoaHocs.FirstOrDefaultAsync(i => i.MaKhoaHoc == khoaHoc.MaKhoaHoc && i.MaHocVien == userId);
            if (dkkh == null) return 0;
            return dkkh.TienDo;
        }
        [Authorize("HocVien")]
        [HttpPost]
        public async Task<IActionResult> IncreaseProgress(string topicSlug, int? topicId, string courseSlug, int? courseId)
        {
            if (courseId == null)
            {
                return NotFound();
            }
            var khoaHoc = await _context.KhoaHocs
                .FirstOrDefaultAsync(m => m.MaKhoaHoc == courseId);
            if (khoaHoc == null)
            {
                return NotFound();
            }
            int? userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString());
            var dkkh = await _context.DangKiKhoaHocs.FirstOrDefaultAsync(i => i.MaKhoaHoc == khoaHoc.MaKhoaHoc);
            if(dkkh != null)
            {
                if(ValidCheck.IsProgressVaild(dkkh.TienDo + 20))
                {
                    dkkh.TienDo += 20;
                }
                try
                {
                    _context.Update(dkkh);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }
             return RedirectToAction("Index", new { topicSlug, topicId, courseSlug, courseId });
        }
        [Authorize("HocVien")]
        [HttpPost]
        public async Task<IActionResult> CancellCourse(string topicSlug, int? topicId, string courseSlug, int? courseId)
        {
            if (courseId == null)
            {
                return NotFound();
            }
            var khoaHoc = await _context.KhoaHocs
                .FirstOrDefaultAsync(m => m.MaKhoaHoc == courseId);
            if (khoaHoc == null)
            {
                return NotFound();
            }
            int? userId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString());
            var dkkh = await _context.DangKiKhoaHocs.FirstOrDefaultAsync(i => i.MaKhoaHoc == khoaHoc.MaKhoaHoc);
            if(dkkh != null)
            {
                try
                {
                    dkkh.DaHuy = true;
                    _context.Update(dkkh);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }
             return RedirectToAction("Index", new { topicSlug, topicId, courseSlug, courseId });
        }
    }
}
