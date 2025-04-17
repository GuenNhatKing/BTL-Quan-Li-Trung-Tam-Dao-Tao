using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLTTDT.Data;
using QLTTDT.Models;

namespace QLTTDT.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DangKiKhoaHocController : Controller
    {
        private readonly QLTTDTDbContext _context;

        public DangKiKhoaHocController(QLTTDTDbContext context)
        {
            _context = context;
        }

        // GET: Admin/DangKiKhoaHoc
        public async Task<IActionResult> Index()
        {
            var qLTTDTDbContext = _context.DangKiKhoaHocs.Include(d => d.MaHocVienNavigation).Include(d => d.MaKhoaHocNavigation);
            return View(await qLTTDTDbContext.ToListAsync());
        }

        // GET: Admin/DangKiKhoaHoc/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dangKiKhoaHoc = await _context.DangKiKhoaHocs
                .Include(d => d.MaHocVienNavigation)
                .Include(d => d.MaKhoaHocNavigation)
                .FirstOrDefaultAsync(m => m.MaDangKi == id);
            if (dangKiKhoaHoc == null)
            {
                return NotFound();
            }

            return View(dangKiKhoaHoc);
        }

        // GET: Admin/DangKiKhoaHoc/Create
        public IActionResult Create()
        {
            ViewData["MaHocVien"] = new SelectList(_context.NguoiDungs, "MaNguoiDung", "MaNguoiDung");
            ViewData["MaKhoaHoc"] = new SelectList(_context.KhoaHocs, "MaKhoaHoc", "MaKhoaHoc");
            return View();
        }

        // POST: Admin/DangKiKhoaHoc/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaDangKi,MaHocVien,MaKhoaHoc,ThoiGianDangKi,TienDo,DaHuy")] DangKiKhoaHoc dangKiKhoaHoc)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dangKiKhoaHoc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaHocVien"] = new SelectList(_context.NguoiDungs, "MaNguoiDung", "MaNguoiDung", dangKiKhoaHoc.MaHocVien);
            ViewData["MaKhoaHoc"] = new SelectList(_context.KhoaHocs, "MaKhoaHoc", "MaKhoaHoc", dangKiKhoaHoc.MaKhoaHoc);
            return View(dangKiKhoaHoc);
        }

        // GET: Admin/DangKiKhoaHoc/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dangKiKhoaHoc = await _context.DangKiKhoaHocs.FindAsync(id);
            if (dangKiKhoaHoc == null)
            {
                return NotFound();
            }
            ViewData["MaHocVien"] = new SelectList(_context.NguoiDungs, "MaNguoiDung", "MaNguoiDung", dangKiKhoaHoc.MaHocVien);
            ViewData["MaKhoaHoc"] = new SelectList(_context.KhoaHocs, "MaKhoaHoc", "MaKhoaHoc", dangKiKhoaHoc.MaKhoaHoc);
            return View(dangKiKhoaHoc);
        }

        // POST: Admin/DangKiKhoaHoc/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaDangKi,MaHocVien,MaKhoaHoc,ThoiGianDangKi,TienDo,DaHuy")] DangKiKhoaHoc dangKiKhoaHoc)
        {
            if (id != dangKiKhoaHoc.MaDangKi)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dangKiKhoaHoc);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DangKiKhoaHocExists(dangKiKhoaHoc.MaDangKi))
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
            ViewData["MaHocVien"] = new SelectList(_context.NguoiDungs, "MaNguoiDung", "MaNguoiDung", dangKiKhoaHoc.MaHocVien);
            ViewData["MaKhoaHoc"] = new SelectList(_context.KhoaHocs, "MaKhoaHoc", "MaKhoaHoc", dangKiKhoaHoc.MaKhoaHoc);
            return View(dangKiKhoaHoc);
        }

        // GET: Admin/DangKiKhoaHoc/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dangKiKhoaHoc = await _context.DangKiKhoaHocs
                .Include(d => d.MaHocVienNavigation)
                .Include(d => d.MaKhoaHocNavigation)
                .FirstOrDefaultAsync(m => m.MaDangKi == id);
            if (dangKiKhoaHoc == null)
            {
                return NotFound();
            }

            return View(dangKiKhoaHoc);
        }

        // POST: Admin/DangKiKhoaHoc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dangKiKhoaHoc = await _context.DangKiKhoaHocs.FindAsync(id);
            if (dangKiKhoaHoc != null)
            {
                _context.DangKiKhoaHocs.Remove(dangKiKhoaHoc);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DangKiKhoaHocExists(int id)
        {
            return _context.DangKiKhoaHocs.Any(e => e.MaDangKi == id);
        }
    }
}
