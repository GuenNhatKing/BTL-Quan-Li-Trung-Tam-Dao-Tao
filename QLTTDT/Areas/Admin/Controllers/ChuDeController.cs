using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLTTDT.Data;
using QLTTDT.Models;
using QLTTDT.Services;

namespace QLTTDT.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ChuDeController : Controller
    {
        private readonly IWebHostEnvironment _webHost;
        private readonly QLTTDTDbContext _context;

        public ChuDeController(QLTTDTDbContext context, IWebHostEnvironment webHost)
        {
            _context = context;
            _webHost = webHost;
        }

        // GET: Admin/ChuDe
        public async Task<IActionResult> Index()
        {
            return View(await _context.ChuDes.ToListAsync());
        }

        // GET: Admin/ChuDe/Details/5
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

        // GET: Admin/ChuDe/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/ChuDe/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaChuDe,TenChuDe,MoTa")] ChuDe chuDe, IFormFile? AnhChuDe = null)
        {
            if (ModelState.IsValid)
            {
                var imageUpload = new ImageUpload(_webHost);
                if (await imageUpload.SaveImageAs(AnhChuDe))
                    chuDe.UrlAnhChuDe = imageUpload.FileName;
                _context.Add(chuDe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(chuDe);
        }

        // GET: Admin/ChuDe/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chuDe = await _context.ChuDes.FindAsync(id);
            if (chuDe == null)
            {
                return NotFound();
            }
            return View(chuDe);
        }

        // POST: Admin/ChuDe/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaChuDe,TenChuDe,MoTa")] ChuDe chuDe, IFormFile? AnhChuDe = null)
        {
            if (id != chuDe.MaChuDe)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var imageUpload = new ImageUpload(_webHost);
                    if (await imageUpload.SaveImageAs(AnhChuDe))
                    {
                        imageUpload.DeleteImage(chuDe.UrlAnhChuDe);
                        chuDe.UrlAnhChuDe = imageUpload.FileName;
                    }
                    _context.Update(chuDe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChuDeExists(chuDe.MaChuDe))
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
            return View(chuDe);
        }

        // GET: Admin/ChuDe/Delete/5
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

        // POST: Admin/ChuDe/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chuDe = await _context.ChuDes.FindAsync(id);
            if (chuDe != null)
            {
                var imgDelete = new ImageUpload(_webHost);
                imgDelete.DeleteImage(chuDe.UrlAnhChuDe);
                _context.ChuDes.Remove(chuDe);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChuDeExists(int id)
        {
            return _context.ChuDes.Any(e => e.MaChuDe == id);
        }
    }
}
