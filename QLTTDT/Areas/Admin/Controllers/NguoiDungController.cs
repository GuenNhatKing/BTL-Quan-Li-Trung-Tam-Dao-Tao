using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using QLTTDT.Data;
using QLTTDT.Models;
using QLTTDT.Services;

namespace QLTTDT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("Admin")]
    public class NguoiDungController : Controller
    {
        private readonly IWebHostEnvironment _webHost;
        private readonly QLTTDTDbContext _context;

        public NguoiDungController(QLTTDTDbContext context, IWebHostEnvironment webHost)
        {
            _context = context;
            _webHost = webHost;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            var nguoiDungs = _context.NguoiDungs.Select(i => i);
            if (!String.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToUpper();
                nguoiDungs = nguoiDungs.Where(i => i.HoVaTen.ToUpper().Contains(searchString)
                || i.Email.ToUpper().Contains(searchString));
            }
            return View(await nguoiDungs.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoiDung = await _context.NguoiDungs
                .FirstOrDefaultAsync(m => m.MaNguoiDung == id);
            if (nguoiDung == null)
            {
                return NotFound();
            }

            return View(nguoiDung);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaNguoiDung,HoVaTen,NgaySinh,SoDienThoai,Email")] NguoiDung nguoiDung, IFormFile? AnhDaiDien = null)
        {
            if (ModelState.IsValid)
            {
                var validation = new ValidCheck(_context);
                if (!await validation.UserValidation(nguoiDung.SoDienThoai, nguoiDung.Email))
                {
                    ModelState.AddModelError(validation.ErrorKey, validation.Error);
                    return View(nguoiDung);
                }
                var imageUpload = new ImageUpload(_webHost);
                if (await imageUpload.SaveImageAs(AnhDaiDien!))
                {
                    nguoiDung.UrlAnhDaiDien = imageUpload.FileName;
                }
                _context.Add(nguoiDung);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(nguoiDung);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoiDung = await _context.NguoiDungs.FindAsync(id);
            if (nguoiDung == null)
            {
                return NotFound();
            }
            return View(nguoiDung);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("MaNguoiDung,HoVaTen,NgaySinh,SoDienThoai,Email")] NguoiDung nguoiDung, IFormFile? AnhDaiDien = null)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _context.NguoiDungs.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    user.HoVaTen = nguoiDung.HoVaTen;
                    user.NgaySinh = nguoiDung.NgaySinh;
                    user.SoDienThoai = nguoiDung.SoDienThoai;
                    user.Email = nguoiDung.Email;

                    _context.Update(user);
                    await _context.SaveChangesAsync();

                    var imageUpload = new ImageUpload(_webHost);
                    if (await imageUpload.SaveImageAs(AnhDaiDien!))
                    {
                        imageUpload.DeleteImage(user.UrlAnhDaiDien!);
                        user.UrlAnhDaiDien = imageUpload.FileName;
                    }

                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return BadRequest(ex.Message);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(nguoiDung);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nguoiDung = await _context.NguoiDungs
                .FirstOrDefaultAsync(m => m.MaNguoiDung == id);
            if (nguoiDung == null)
            {
                return NotFound();
            }

            return View(nguoiDung);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int? getId = Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var nguoiDung = await _context.NguoiDungs.FirstOrDefaultAsync(m => m.MaNguoiDung == id);
            if (HttpContext.User.IsInRole("Admin") && getId == id)
            {
                ModelState.AddModelError("", "Không được xoá bản thân admin.");
                return View(nguoiDung);
            }
            if (nguoiDung != null)
            {
                try
                {
                    _context.NguoiDungs.Remove(nguoiDung);
                    await _context.SaveChangesAsync();
                    var imgDelete = new ImageUpload(_webHost);
                    imgDelete.DeleteImage(nguoiDung.UrlAnhDaiDien!);
                }
                catch (Exception ex)
                {
                    if (_context.TaiKhoans.Any(i => i.MaNguoiDung == nguoiDung.MaNguoiDung))
                    {
                        ModelState.AddModelError("", "Người dùng đã được sử dụng cho 1 tài khoản.");
                        return View(nguoiDung);
                    }
                    if (_context.DangKiKhoaHocs.Any(i => i.MaHocVien == nguoiDung.MaNguoiDung))
                    {
                        ModelState.AddModelError("", "Người dùng đã đăng ký ít nhất một khoá học.");
                        return View(nguoiDung);
                    }
                    if (_context.KhoaHocs.Any(i => i.MaGiangVien == nguoiDung.MaNguoiDung))
                    {
                        ModelState.AddModelError("", "Người dùng đã làm giảng viên cho ít nhất một khoá học.");
                        return View(nguoiDung);
                    }
                    return BadRequest(ex.Message);
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
