using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QLTTDT.Data;
using QLTTDT.Models;

namespace QLTTDT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("Admin")]
    public class CapDoController : Controller
    {
        private readonly QLTTDTDbContext _context;

        public CapDoController(QLTTDTDbContext context)
        {
            _context = context;
        }

        // GET: Admin/CapDo
        public async Task<IActionResult> Index()
        {
            return View(await _context.CapDos.ToListAsync());
        }

        // GET: Admin/CapDo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var capDo = await _context.CapDos
                .FirstOrDefaultAsync(m => m.MaCapDo == id);
            if (capDo == null)
            {
                return NotFound();
            }

            return View(capDo);
        }

        // GET: Admin/CapDo/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/CapDo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaCapDo,TenCapDo")] CapDo capDo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(capDo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(capDo);
        }

        // GET: Admin/CapDo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var capDo = await _context.CapDos.FindAsync(id);
            if (capDo == null)
            {
                return NotFound();
            }
            return View(capDo);
        }

        // POST: Admin/CapDo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("MaCapDo,TenCapDo")] CapDo capDo)
        {
            if (id == null)
            {
                return NotFound();
            }
            var level = await _context.CapDos.FindAsync(id);
            if (level == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    level.TenCapDo = capDo.TenCapDo;
                    _context.Update(level);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!CapDoExists(level.MaCapDo))
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
            return View(capDo);
        }

        // GET: Admin/CapDo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var capDo = await _context.CapDos
                .FirstOrDefaultAsync(m => m.MaCapDo == id);
            if (capDo == null)
            {
                return NotFound();
            }

            return View(capDo);
        }

        // POST: Admin/CapDo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var capDo = await _context.CapDos.FindAsync(id);
            if (capDo != null)
            {
                try
                {
                    _context.CapDos.Remove(capDo);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private bool CapDoExists(int id)
        {
            return _context.CapDos.Any(e => e.MaCapDo == id);
        }
    }
}
