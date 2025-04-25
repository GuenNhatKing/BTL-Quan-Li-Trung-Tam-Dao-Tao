using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLTTDT.Data;
using QLTTDT.Models;
using QLTTDT.Services;

namespace QLTTDT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("Admin")]
    public class KhoaHocController : Controller
    {
        private readonly QLTTDTDbContext _context;
        private readonly IWebHostEnvironment _webHost;
        public KhoaHocController(QLTTDTDbContext context, IWebHostEnvironment webHost)
        {
            _context = context;
            _webHost = webHost;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var khoaHocs = _context.KhoaHocs.Include(k => k.MaCapDoNavigation)
                .Include(k => k.MaChuDeNavigation).Include(k => k.MaGiangVienNavigation)
                .Select(i => i);
            if (!String.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToUpper();
                khoaHocs = khoaHocs.Where(i => i.TenKhoaHoc.ToUpper().Contains(searchString)
                || i.MaCapDoNavigation.TenCapDo.ToUpper().Contains(searchString)
                || i.MaChuDeNavigation.TenChuDe.ToUpper().Contains(searchString));
            }
            return View(await khoaHocs.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khoaHoc = await _context.KhoaHocs
                .Include(k => k.MaCapDoNavigation)
                .Include(k => k.MaChuDeNavigation)
                .Include(k => k.MaGiangVienNavigation)
                .FirstOrDefaultAsync(m => m.MaKhoaHoc == id);
            if (khoaHoc == null)
            {
                return NotFound();
            }

            return View(khoaHoc);
        }

        public async Task<IActionResult> Create()
        {
            await LoadDataList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaKhoaHoc,MaGiangVien,MaChuDe,MaCapDo,UrlAnh,TenKhoaHoc,MoTa,ThoiGianKhaiGiang,HocPhi,SoLuongHocVienToiDa")] KhoaHoc khoaHoc, IFormFile? AnhKhoaHoc = null)
        {
            await LoadDataList();
            if (ModelState.IsValid)
            {
                var validation = new ValidCheck(_context);
                if (!validation.IsCourseValid(khoaHoc))
                {
                    ModelState.AddModelError(validation.ErrorKey, validation.Error);
                    return View(khoaHoc);
                }
                var imageUpload = new ImageUpload(_webHost);
                if (await imageUpload.SaveImageAs(AnhKhoaHoc!))
                {
                    khoaHoc.UrlAnh = imageUpload.FileName;
                }
                _context.Add(khoaHoc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(khoaHoc);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khoaHoc = await _context.KhoaHocs.FindAsync(id);
            if (khoaHoc == null)
            {
                return NotFound();
            }
            await LoadDataList();
            return View(khoaHoc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("MaKhoaHoc, MaGiangVien, MaChuDe, MaCapDo, TenKhoaHoc,MoTa,ThoiGianKhaiGiang,HocPhi,SoLuongHocVienToiDa")] KhoaHoc khoaHoc, IFormFile? AnhKhoaHoc)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.KhoaHocs.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            await LoadDataList();
            if (ModelState.IsValid)
            {
                try
                {
                    var validation = new ValidCheck(_context);
                    if (!validation.IsCourseValid(khoaHoc))
                    {
                        ModelState.AddModelError(validation.ErrorKey, validation.Error);
                        return View(khoaHoc);
                    }
                    var count = await _context.DangKiKhoaHocs.CountAsync(i => _context.KhoaHocs.Any(j => j.MaKhoaHoc == i.MaKhoaHoc));
                    if (count > khoaHoc.SoLuongHocVienToiDa)
                    {
                        ModelState.AddModelError("", "Không thể thay đổi số học viên tối đa nhỏ hơn số học viên hiện tại.");
                        return View(khoaHoc);
                    }
                    course.MaCapDo = khoaHoc.MaCapDo;
                    course.MaChuDe = khoaHoc.MaChuDe;
                    course.MaGiangVien = khoaHoc.MaGiangVien;
                    course.TenKhoaHoc = khoaHoc.TenKhoaHoc;
                    course.ThoiGianKhaiGiang = khoaHoc.ThoiGianKhaiGiang;
                    course.HocPhi = khoaHoc.HocPhi;
                    course.SoLuongHocVienToiDa = khoaHoc.SoLuongHocVienToiDa;
                    course.MoTa = khoaHoc.MoTa;

                    _context.Update(course);
                    await _context.SaveChangesAsync();

                    var imageUpload = new ImageUpload(_webHost);
                    if (await imageUpload.SaveImageAs(AnhKhoaHoc!))
                    {
                        imageUpload.DeleteImage(course.UrlAnh!);
                        course.UrlAnh = imageUpload.FileName;
                    }

                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return BadRequest(ex.Message);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(khoaHoc);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khoaHoc = await _context.KhoaHocs
                .Include(k => k.MaCapDoNavigation)
                .Include(k => k.MaChuDeNavigation)
                .Include(k => k.MaGiangVienNavigation)
                .FirstOrDefaultAsync(m => m.MaKhoaHoc == id);
            if (khoaHoc == null)
            {
                return NotFound();
            }

            return View(khoaHoc);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var khoaHoc = await _context.KhoaHocs
                .Include(k => k.MaCapDoNavigation)
                .Include(k => k.MaChuDeNavigation)
                .Include(k => k.MaGiangVienNavigation)
                .FirstOrDefaultAsync(m => m.MaKhoaHoc == id);
            if (khoaHoc != null)
            {
                try
                {
                    _context.KhoaHocs.Remove(khoaHoc);
                    await _context.SaveChangesAsync();

                    var imgDelete = new ImageUpload(_webHost);
                    imgDelete.DeleteImage(khoaHoc.UrlAnh!);
                }
                catch (Exception ex)
                {
                    if (_context.DangKiKhoaHocs.IgnoreQueryFilters().Any(i => i.MaKhoaHoc == khoaHoc.MaKhoaHoc))
                    {
                        ModelState.AddModelError("", "Khoá học đã được đăng ký bởi ít nhất một người dùng.");
                        return View(khoaHoc);
                    }
                    return BadRequest(ex.Message);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDataList()
        {
            var giangVienList = await _context.TaiKhoans
            .Include(i => i.MaNguoiDungNavigation)
            .Include(i => i.MaVaiTroNavigation)
            .Where(i => i.MaVaiTroNavigation.TenVaiTro == "GiangVien")
            .Select(i => new
            {
                MaGiangVien = i.MaNguoiDung,
                DisplayText = i.MaNguoiDung + " - " + i.MaNguoiDungNavigation.Email
            })
            .AsSplitQuery()
            .ToListAsync();

            var chuDeList = await _context.ChuDes
            .Select(i => new
            {
                MaChuDe = i.MaChuDe,
                DisplayText = i.MaChuDe + " - " + i.TenChuDe,
            }).ToListAsync();

            var capDoList = await _context.CapDos
            .Select(i => new
            {
                MaCapDo = i.MaCapDo,
                DisplayText = i.MaCapDo + " - " + i.TenCapDo,
            }).ToListAsync();

            ViewData["MaCapDo"] = new SelectList(capDoList, "MaCapDo", "DisplayText");
            ViewData["MaChuDe"] = new SelectList(chuDeList, "MaChuDe", "DisplayText");
            ViewData["MaGiangVien"] = new SelectList(giangVienList, "MaGiangVien", "DisplayText");
        }
    }
}
