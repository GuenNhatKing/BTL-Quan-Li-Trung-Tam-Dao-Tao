﻿using System.Security.Claims;
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
        public async Task<IActionResult> Index(string? topicSlug, int? topicId, string? courseSlug, int? courseId)
        {
            var slugCheck = new SlugCheck(_context);
            if (!await slugCheck.CheckTopicCourseSlug(topicSlug, topicId, courseSlug, courseId))
            {
                return NotFound();
            }
            var khoaHoc = await _context.KhoaHocs
                .FirstOrDefaultAsync(m => m.MaKhoaHoc == courseId);
            if (khoaHoc == null)
            {
                return NotFound();
            }
            int? userId = GetUserId();
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
                ThoiGianCompute = khoaHoc.ThoiGianCompute,
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

        [Authorize(Roles = "HocVien,Admin")]
        [HttpPost]
        public async Task<IActionResult> Index(string topicSlug, int? topicId, string courseSlug, int? courseId, bool noUse)
        {
            var slugCheck = new SlugCheck(_context);
            if (!await slugCheck.CheckTopicCourseSlug(topicSlug, topicId, courseSlug, courseId))
            {
                return NotFound();
            }
            var khoaHoc = await _context.KhoaHocs.FirstOrDefaultAsync(m => m.MaKhoaHoc == courseId);
            if (khoaHoc == null)
            {
                return NotFound();
            }
            int? userId = GetUserId();
            var validation = new ValidCheck(_context);
            if (userId != null && courseId != null && await validation.IsRegisterCourseVaild(userId, courseId))
            {
                var dkkh = await _context.DangKiKhoaHocs.IgnoreQueryFilters().FirstOrDefaultAsync(i => i.MaHocVien == userId && i.MaKhoaHoc == courseId && i.DaHuy == true);
                if (dkkh != null)
                {
                    dkkh.DaHuy = false;
                    dkkh.HocPhi = khoaHoc.HocPhi;
                    dkkh.ThoiGianDangKi = DateTime.Now;
                    try
                    {
                        _context.Update(dkkh);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        return BadRequest(ex.Message);
                    }
                }
                else
                {
                    var register = new DangKiKhoaHoc
                    {
                        MaHocVien = (int)userId,
                        MaKhoaHoc = (int)courseId,
                        HocPhi = khoaHoc.HocPhi,
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
        [Authorize(Roles = "HocVien,Admin")]
        [HttpPost]
        public async Task<IActionResult> IncreaseProgress(string topicSlug, int? topicId, string courseSlug, int? courseId)
        {
            var slugCheck = new SlugCheck(_context);
            if (!await slugCheck.CheckTopicCourseSlug(topicSlug, topicId, courseSlug, courseId))
            {
                return NotFound();
            }
            var khoaHoc = await _context.KhoaHocs.FirstOrDefaultAsync(m => m.MaKhoaHoc == courseId);
            if (khoaHoc == null)
            {
                return NotFound();
            }
            int? userId = GetUserId();
            var dkkh = await _context.DangKiKhoaHocs.FirstOrDefaultAsync(i => i.MaKhoaHoc == khoaHoc.MaKhoaHoc && i.MaHocVien == userId);
            if (dkkh != null)
            {
                if (ValidCheck.IsProgressValid(dkkh.TienDo + 20))
                {
                    dkkh.TienDo += 20;
                }
                try
                {
                    _context.Update(dkkh);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return RedirectToAction("Index", new { topicSlug, topicId, courseSlug, courseId });
        }
        [Authorize(Roles = "HocVien,Admin")]
        [HttpPost]
        public async Task<IActionResult> CancellCourse(string topicSlug, int? topicId, string courseSlug, int? courseId)
        {
            if (courseId == null)
            {
                return NotFound();
            }
            var khoaHoc = await _context.KhoaHocs.FirstOrDefaultAsync(m => m.MaKhoaHoc == courseId);
            if (khoaHoc == null)
            {
                return NotFound();
            }
            int? userId = GetUserId();
            var dkkh = await _context.DangKiKhoaHocs.FirstOrDefaultAsync(i => i.MaKhoaHoc == khoaHoc.MaKhoaHoc && i.MaHocVien == userId);
            if (dkkh != null)
            {
                try
                {
                    dkkh.DaHuy = true;
                    _context.Update(dkkh);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return RedirectToAction("Index", new { topicSlug, topicId, courseSlug, courseId });
        }
        [Authorize(Roles = "HocVien,Admin")]
        public async Task<IActionResult> RegisteredCourse()
        {
            int? userId = GetUserId();
            var courses = await _context.KhoaHocs
                .Include(i => i.MaChuDeNavigation)
                .Include(i => i.MaCapDoNavigation)
                .Where(i => _context.DangKiKhoaHocs.Any(j => j.MaKhoaHoc == i.MaKhoaHoc && j.MaHocVien == userId))
                .Select(i => new CourseCardWithTopic
                {
                    MaChuDe = i.MaChuDe,
                    TenChuDe = i.MaChuDeNavigation.TenChuDe,
                    CapDo = i.MaCapDoNavigation.TenCapDo,
                    MaKhoaHoc = i.MaKhoaHoc,
                    MoTa = i.MoTa,
                    TenKhoaHoc = i.TenKhoaHoc,
                    UrlAnhKhoaHoc = i.UrlAnh,
                })
                .AsSplitQuery()
                .ToListAsync();
            return View(courses);
        }
        [Authorize(Roles = "HocVien,Admin")]
        public async Task<IActionResult> LearningCourse()
        {
            int? userId = GetUserId();
            var courses = await _context.KhoaHocs
                .Include(i => i.MaChuDeNavigation)
                .Include(i => i.MaCapDoNavigation)
                .Where(i => i.ThoiGianKhaiGiang <= DateTime.Now && _context.DangKiKhoaHocs
                .Any(j => j.MaKhoaHoc == i.MaKhoaHoc && j.MaHocVien == userId
                && j.TienDo >= 0 && j.TienDo < 100))
                .Select(i => new CourseCardWithTopic
                {
                    MaChuDe = i.MaChuDe,
                    TenChuDe = i.MaChuDeNavigation.TenChuDe,
                    CapDo = i.MaCapDoNavigation.TenCapDo,
                    MaKhoaHoc = i.MaKhoaHoc,
                    MoTa = i.MoTa,
                    TenKhoaHoc = i.TenKhoaHoc,
                    UrlAnhKhoaHoc = i.UrlAnh,
                })
                .AsSplitQuery()
                .ToListAsync();
            return View(courses);
        }
        [Authorize(Roles = "HocVien,Admin")]
        public async Task<IActionResult> CompletedCourse()
        {
            int? userId = GetUserId();
            var courses = await _context.KhoaHocs
                .Include(i => i.MaChuDeNavigation)
                .Include(i => i.MaCapDoNavigation)
                .Where(i => i.ThoiGianKhaiGiang <= DateTime.Now && _context.DangKiKhoaHocs
                .Any(j => j.MaKhoaHoc == i.MaKhoaHoc && j.MaHocVien == userId
                && j.TienDo == 100))
                .Select(i => new CourseCardWithTopic
                {
                    MaChuDe = i.MaChuDe,
                    TenChuDe = i.MaChuDeNavigation.TenChuDe,
                    CapDo = i.MaCapDoNavigation.TenCapDo,
                    MaKhoaHoc = i.MaKhoaHoc,
                    MoTa = i.MoTa,
                    TenKhoaHoc = i.TenKhoaHoc,
                    UrlAnhKhoaHoc = i.UrlAnh,
                })
                .AsSplitQuery()
                .ToListAsync();
            return View(courses);
        }
        private int? GetUserId()
        {
            return Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString());
        }
    }
}
