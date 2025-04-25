using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
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
    public class ChuDeController : Controller
    {
        private readonly IWebHostEnvironment _webHost;
        private readonly QLTTDTDbContext _context;

        public ChuDeController(QLTTDTDbContext context, IWebHostEnvironment webHost)
        {
            _context = context;
            _webHost = webHost;
        }
        public async Task<IActionResult> Index(string? searchString)
        {
            var chuDes = _context.ChuDes.Select(i => i);
            if (!String.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToUpper();
                chuDes = chuDes.Where(i => i.TenChuDe.ToUpper().Contains(searchString));
            }
            return View(await chuDes.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chuDe = await _context.ChuDes
            .FirstOrDefaultAsync(m => m.MaChuDe == id);
            if (chuDe == null)
            {
                return NotFound();
            }

            return View(chuDe);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaChuDe, TenChuDe, MoTa")] ChuDe chuDe, IFormFile? AnhChuDe = null)
        {
            if (ModelState.IsValid)
            {
                var imageUpload = new ImageUpload(_webHost);
                if (await imageUpload.SaveImageAs(AnhChuDe!))
                {
                    chuDe.UrlAnhChuDe = imageUpload.FileName;
                }
                _context.Add(chuDe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(chuDe);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chuDe = await _context.ChuDes
            .FirstOrDefaultAsync(i => i.MaChuDe == id);
            if (chuDe == null)
            {
                return NotFound();
            }
            return View(chuDe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("MaChuDe,TenChuDe,MoTa")] ChuDe chuDe, IFormFile? AnhChuDe = null)
        {
            if (id == null)
            {
                return NotFound();
            }
            var topic = await _context.ChuDes
            .FirstOrDefaultAsync(i => i.MaChuDe == id);
            if (topic == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    topic.TenChuDe = chuDe.TenChuDe;
                    topic.MoTa = chuDe.MoTa;

                    _context.Update(topic);
                    await _context.SaveChangesAsync();

                    var imageUpload = new ImageUpload(_webHost);
                    if (await imageUpload.SaveImageAs(AnhChuDe!))
                    {
                        imageUpload.DeleteImage(topic.UrlAnhChuDe!);
                        topic.UrlAnhChuDe = imageUpload.FileName;
                    }

                    _context.Update(topic);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return BadRequest(ex.Message);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(chuDe);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chuDe = await _context.ChuDes
            .FirstOrDefaultAsync(m => m.MaChuDe == id);
            if (chuDe == null)
            {
                return NotFound();
            }

            return View(chuDe);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chuDe = await _context.ChuDes
            .FirstOrDefaultAsync(m => m.MaChuDe == id);
            if (chuDe != null)
            {
                try
                {
                    _context.ChuDes.Remove(chuDe);
                    await _context.SaveChangesAsync();
                    var imgDelete = new ImageUpload(_webHost);
                    imgDelete.DeleteImage(chuDe.UrlAnhChuDe!);
                }
                catch (Exception ex)
                {
                    if (_context.KhoaHocs.Any(i => i.MaChuDe == chuDe.MaChuDe))
                    {
                        ModelState.AddModelError("", "Chủ đề đã được sử dụng bởi ít nhất một khoá học.");
                        return View(chuDe);
                    }
                    return BadRequest(ex.Message);
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
