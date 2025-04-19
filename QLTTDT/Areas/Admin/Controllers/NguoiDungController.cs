using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task<IActionResult> Index()
        {
            return View(await _context.NguoiDungs.ToListAsync());
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
                if (await imageUpload.SaveImageAs(AnhDaiDien))
                    nguoiDung.UrlAnhDaiDien = imageUpload.FileName;
                var validation = new ValidCheck(_context);
                if(!await validation.UserValidation(nguoiDung))
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
        public async Task<IActionResult> Edit(int id, [Bind("MaNguoiDung,HoVaTen,NgaySinh,SoDienThoai,Email")] NguoiDung nguoiDung, IFormFile? AnhDaiDien = null)
        {
            if (id != nguoiDung.MaNguoiDung)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var imageUpload = new ImageUpload(_webHost);
                    if (await imageUpload.SaveImageAs(AnhDaiDien))
                    {
                        imageUpload.DeleteImage(nguoiDung.UrlAnhDaiDien);
                        nguoiDung.UrlAnhDaiDien = imageUpload.FileName;
                    }
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NguoiDungExists(nguoiDung.MaNguoiDung))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
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
                var imgDelete = new ImageUpload(_webHost);
                imgDelete.DeleteImage(nguoiDung.UrlAnhDaiDien);
                _context.NguoiDungs.Remove(nguoiDung);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NguoiDungExists(int id)
        {
            return _context.NguoiDungs.Any(e => e.MaNguoiDung == id);
        }
    }
}
