﻿using System;
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

        public async Task<IActionResult> Index(string? searchString)
        {
            var capDos = _context.CapDos
            .Select(i => i);
            if (!String.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToUpper();
                capDos = capDos.Where(i => i.TenCapDo.ToUpper().Contains(searchString));
            }
            return View(await capDos.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var capDo = await _context.CapDos.FirstOrDefaultAsync(m => m.MaCapDo == id);
            if (capDo == null)
            {
                return NotFound();
            }

            return View(capDo);
        }

        public IActionResult Create()
        {
            return View();
        }

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
                    return BadRequest(ex.Message);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(capDo);
        }
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
                    if (_context.KhoaHocs.Any(i => i.MaCapDo == capDo.MaCapDo))
                    {
                        ModelState.AddModelError("", "Cấp độ đã được sử dụng cho ít nhất một khoá học.");
                        return View(capDo);
                    }
                    return BadRequest(ex.Message);
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
