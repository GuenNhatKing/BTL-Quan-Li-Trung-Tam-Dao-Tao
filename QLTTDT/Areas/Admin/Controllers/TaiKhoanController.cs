using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize("Admin")]
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
            .Select(i => new
            {
                MaNguoiDung = i.MaNguoiDung,
                DisplayText = i.MaNguoiDung + " - " + i.Email
            }).ToList();
            var vaiTroList = _context.VaiTros
            .Select(i => new
            {
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
            var nguoiDungList = _context.NguoiDungs
            .Where(i => !_context.TaiKhoans.Any(j => j.MaNguoiDung == i.MaNguoiDung))
            .Select(i => new
            {
                MaNguoiDung = i.MaNguoiDung,
                DisplayText = i.MaNguoiDung + " - " + i.Email
            }).ToList();
            var vaiTroList = _context.VaiTros
            .Select(i => new
            {
                MaVaiTro = i.MaVaiTro,
                DisplayText = i.MaVaiTro + " - " + i.TenVaiTro
            }).ToList();
            ViewData["MaNguoiDung"] = new SelectList(nguoiDungList, "MaNguoiDung", "DisplayText");
            ViewData["MaVaiTro"] = new SelectList(vaiTroList, "MaVaiTro", "DisplayText");
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
            var nguoiDung = await _context.NguoiDungs
            .Select(i => new
            {
                MaNguoiDung = i.MaNguoiDung,
                DisplayText = i.MaNguoiDung + " - " + i.Email
            }).FirstOrDefaultAsync(i => i.MaNguoiDung == taiKhoan.MaNguoiDung);
            var vaiTro = await _context.VaiTros
            .Select(i => new
            {
                MaVaiTro = i.MaVaiTro,
                DisplayText = i.MaVaiTro + " - " + i.TenVaiTro
            }).FirstOrDefaultAsync(i => i.MaVaiTro == taiKhoan.MaVaiTro);
            ViewData["MaNguoiDung"] = nguoiDung?.DisplayText;
            ViewData["MaVaiTro"] = vaiTro?.DisplayText;
            return View(taiKhoan);
        }

        // POST: Admin/TaiKhoan/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("MaTaiKhoan,TenDangNhap")] TaiKhoan taiKhoan, string? MatKhauMoi = null)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.TaiKhoans.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            var nguoiDung = await _context.NguoiDungs
            .Select(i => new
            {
                MaNguoiDung = i.MaNguoiDung,
                DisplayText = i.MaNguoiDung + " - " + i.Email
            }).FirstOrDefaultAsync(i => i.MaNguoiDung == account.MaNguoiDung);
            var vaiTro = await _context.VaiTros
            .Select(i => new
            {
                MaVaiTro = i.MaVaiTro,
                DisplayText = i.MaVaiTro + " - " + i.TenVaiTro
            }).FirstOrDefaultAsync(i => i.MaVaiTro == account.MaVaiTro);
            ViewData["MaNguoiDung"] = nguoiDung?.DisplayText;
            ViewData["MaVaiTro"] = vaiTro?.DisplayText;

            if (ModelState.IsValid)
            {
                try
                {
                    account.TenDangNhap = taiKhoan.TenDangNhap;
                    if (!string.IsNullOrEmpty(MatKhauMoi))
                    {
                        account.MatKhau = Login.GetHashedPassword(account.Salt, MatKhauMoi);
                    }
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!TaiKhoanExists(account.MaTaiKhoan))
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
                try
                {
                    _context.TaiKhoans.Remove(taiKhoan);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }


            return RedirectToAction(nameof(Index));
        }

        private bool TaiKhoanExists(int id)
        {
            return _context.TaiKhoans.Any(e => e.MaTaiKhoan == id);
        }
    }
}
