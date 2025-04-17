using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLTTDT.Data;
using QLTTDT.Models;
using QLTTDT.Services;

namespace QLTTDT.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TaiKhoanController : Controller
    {
        private readonly QLTTDTDbContext _context;

        public TaiKhoanController(QLTTDTDbContext context)
        {
            _context = context;
        }

        // GET: Admin/TaiKhoan
        public async Task<IActionResult> Index()
        {
            var qLTTDTDbContext = _context.TaiKhoans.Include(t => t.MaNguoiDungNavigation).Include(t => t.MaVaiTroNavigation);
            return View(await qLTTDTDbContext.ToListAsync());
        }

        // GET: Admin/TaiKhoan/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.MaNguoiDungNavigation)
                .Include(t => t.MaVaiTroNavigation)
                .FirstOrDefaultAsync(m => m.MaTaiKhoan == id);
            if (taiKhoan == null)
            {
                return NotFound();
            }

            return View(taiKhoan);
        }

        // GET: Admin/TaiKhoan/Create
        public IActionResult Create()
        {
            var nguoiDungList = _context.NguoiDungs
            .Where(i => !_context.TaiKhoans.Any(j => j.MaNguoiDung == i.MaNguoiDung))
            .Select(i => new {
                MaNguoiDung = i.MaNguoiDung,
                DisplayText = i.MaNguoiDung + " - " + i.Email
            }).ToList();
            var vaiTroList = _context.VaiTros
            .Select(i => new {
                MaVaiTro = i.MaVaiTro,
                DisplayText = i.MaVaiTro + " - " + i.TenVaiTro
            }).ToList();
            ViewData["MaNguoiDung"] = new SelectList(nguoiDungList, "MaNguoiDung", "DisplayText");
            ViewData["MaVaiTro"] = new SelectList(vaiTroList, "MaVaiTro", "DisplayText");
            return View();
        }

        // POST: Admin/TaiKhoan/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaTaiKhoan,MaNguoiDung,MaVaiTro,TenDangNhap,MatKhau")] TaiKhoan taiKhoan)
        {
            if (taiKhoan.Salt == null)
            {
                taiKhoan.Salt = Register.CreateSalt(_context);
                ModelState.Remove("Salt");
            }
            if (ModelState.IsValid)
            {
                var validation = new ValidCheck(_context);
                if (!await validation.AccountValidation(taiKhoan))
                {
                    ModelState.AddModelError(validation.ErrorKey, validation.Error);
                    return View(taiKhoan);
                }
                taiKhoan.MatKhau = Login.GetHashedPassword(taiKhoan.Salt, taiKhoan.MatKhau);
                _context.Add(taiKhoan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    var key = state.Key; // Tên của field
                    var errors = state.Value.Errors;

                    if (errors.Count > 0)
                    {
                        Console.WriteLine($"Field '{key}' has errors:");

                        foreach (var error in errors)
                        {
                            Console.WriteLine($" - {error.ErrorMessage}");
                        }
                    }
                }
            }
            var nguoiDungList = _context.NguoiDungs
            .Where(i => !_context.TaiKhoans.Any(j => j.MaNguoiDung == i.MaNguoiDung))
            .Select(i => new {
                MaNguoiDung = i.MaNguoiDung,
                DisplayText = i.MaNguoiDung + " - " + i.Email
            }).ToList();
            var vaiTroList = _context.VaiTros
            .Select(i => new {
                MaVaiTro = i.MaVaiTro,
                DisplayText = i.MaVaiTro + " - " + i.TenVaiTro
            }).ToList();
            ViewData["MaNguoiDung"] = new SelectList(nguoiDungList, "MaNguoiDung", "DisplayText", nguoiDungList.Find(i => i.MaNguoiDung == taiKhoan.MaNguoiDung).DisplayText);
            ViewData["MaVaiTro"] = new SelectList(vaiTroList, "MaVaiTro", "DisplayText", vaiTroList.Find(i => i.MaVaiTro == taiKhoan.MaVaiTro).DisplayText);
            return View(taiKhoan);
        }

        // GET: Admin/TaiKhoan/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan == null)
            {
                return NotFound();
            }
            var nguoiDung = _context.NguoiDungs
            .Select(i => new {
                MaNguoiDung = i.MaNguoiDung,
                DisplayText = i.MaNguoiDung + " - " + i.Email
            }).Single(i => i.MaNguoiDung == taiKhoan.MaNguoiDung);
            var vaiTroList = _context.VaiTros
            .Select(i => new {
                MaVaiTro = i.MaVaiTro,
                DisplayText = i.MaVaiTro + " - " + i.TenVaiTro
            }).ToList();
            ViewData["MaNguoiDung"] = nguoiDung.DisplayText;
            ViewData["MaVaiTro"] = new SelectList(vaiTroList, "MaVaiTro", "DisplayText", vaiTroList.Find(i => i.MaVaiTro == taiKhoan.MaVaiTro).DisplayText);
            return View(taiKhoan);
        }

        // POST: Admin/TaiKhoan/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaTaiKhoan,MaNguoiDung,MaVaiTro,TenDangNhap,Salt,MatKhau")] TaiKhoan taiKhoan, string MatKhauMoi = null)
        {
            if (id != taiKhoan.MaTaiKhoan)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if(!string.IsNullOrEmpty(MatKhauMoi))
                    {
                        Console.WriteLine($"Da cap nhat mat khau cho {taiKhoan.TenDangNhap}");
                        taiKhoan.MatKhau = Login.GetHashedPassword(taiKhoan.Salt, MatKhauMoi);
                    }
                    _context.Update(taiKhoan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaiKhoanExists(taiKhoan.MaTaiKhoan))
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
            ViewData["MaNguoiDung"] = new SelectList(_context.NguoiDungs, "MaNguoiDung", "MaNguoiDung", taiKhoan.MaNguoiDung);
            ViewData["MaVaiTro"] = new SelectList(_context.VaiTros, "MaVaiTro", "MaVaiTro", taiKhoan.MaVaiTro);
            return View(taiKhoan);
        }

        // GET: Admin/TaiKhoan/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.MaNguoiDungNavigation)
                .Include(t => t.MaVaiTroNavigation)
                .FirstOrDefaultAsync(m => m.MaTaiKhoan == id);
            if (taiKhoan == null)
            {
                return NotFound();
            }

            return View(taiKhoan);
        }

        // POST: Admin/TaiKhoan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan != null)
            {
                _context.TaiKhoans.Remove(taiKhoan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaiKhoanExists(int id)
        {
            return _context.TaiKhoans.Any(e => e.MaTaiKhoan == id);
        }
    }
}
