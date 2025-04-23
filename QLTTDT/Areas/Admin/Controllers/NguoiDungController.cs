using System;
using System.Collections.Generic;
using System.Linq;
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

        // GET: Admin/NguoiDung
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

        // GET: Admin/NguoiDung/Details/5
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

        // GET: Admin/NguoiDung/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/NguoiDung/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaNguoiDung,HoVaTen,NgaySinh,SoDienThoai,Email")] NguoiDung nguoiDung, IFormFile? AnhDaiDien = null)
        {
            if (ModelState.IsValid)
            {
                var imageUpload = new ImageUpload(_webHost);
                if (await imageUpload.SaveImageAs(AnhDaiDien!))
                    nguoiDung.UrlAnhDaiDien = imageUpload.FileName;
                var validation = new ValidCheck(_context);
                if (!await validation.UserValidation(nguoiDung))
                {
                    ModelState.AddModelError(validation.ErrorKey, validation.Error);
                    return View(nguoiDung);
                }
                _context.Add(nguoiDung);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(nguoiDung);
        }

        // GET: Admin/NguoiDung/Edit/5
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

        // POST: Admin/NguoiDung/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                    if (!NguoiDungExists(user.MaNguoiDung))
                    {
                        return NotFound();
                    }
                    else
                    {
                        return BadRequest(ex.Message);
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(nguoiDung);
        }

        // GET: Admin/NguoiDung/Delete/5
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

        // POST: Admin/NguoiDung/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nguoiDung = await _context.NguoiDungs.FindAsync(id);
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

        private bool NguoiDungExists(int id)
        {
            return _context.NguoiDungs.Any(e => e.MaNguoiDung == id);
        }
    }
}
