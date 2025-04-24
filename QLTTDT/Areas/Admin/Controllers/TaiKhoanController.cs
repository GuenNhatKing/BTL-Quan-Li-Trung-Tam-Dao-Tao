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

        public async Task<IActionResult> Index(string searchString)
        {
            var taiKhoans = _context.TaiKhoans.Include(t => t.MaNguoiDungNavigation)
                .Include(t => t.MaVaiTroNavigation).Select(i => i);
            if (!String.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToUpper();
                taiKhoans = taiKhoans.Where(i => i.TenDangNhap.ToUpper().Contains(searchString)
                || i.MaNguoiDungNavigation.Email.ToUpper().Contains(searchString)
                || i.MaVaiTroNavigation.TenVaiTro.ToUpper().Contains(searchString));
            }
            return View(await taiKhoans.ToListAsync());
        }

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

        public async Task<IActionResult> Create()
        {
            await LoadDataList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaTaiKhoan,MaNguoiDung,MaVaiTro,TenDangNhap,MatKhau")] TaiKhoan taiKhoan)
        {
            await LoadDataList();
            if (taiKhoan.Salt == null)
            {
                taiKhoan.Salt = Register.CreateSalt(_context);
                ModelState.Remove("Salt");
            }
            if (ModelState.IsValid)
            {
                var validation = new ValidCheck(_context);
                if (!await validation.AccountValidation(taiKhoan.TenDangNhap, taiKhoan.MatKhau, taiKhoan.MaVaiTro, taiKhoan.MaNguoiDung))
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
            await LoadData(taiKhoan);
            return View(taiKhoan);
        }

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
            await LoadData(account);

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
                    return BadRequest(ex.Message);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(taiKhoan);
        }

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

        private async Task LoadDataList()
        {
             var nguoiDungList = await _context.NguoiDungs
            .Where(i => !_context.TaiKhoans.Any(j => j.MaNguoiDung == i.MaNguoiDung))
            .Select(i => new
            {
                MaNguoiDung = i.MaNguoiDung,
                DisplayText = i.MaNguoiDung + " - " + i.Email
            }).ToListAsync();
            var vaiTroList = await _context.VaiTros
            .Select(i => new
            {
                MaVaiTro = i.MaVaiTro,
                DisplayText = i.MaVaiTro + " - " + i.TenVaiTro
            }).ToListAsync();
            ViewData["MaNguoiDung"] = new SelectList(nguoiDungList, "MaNguoiDung", "DisplayText");
            ViewData["MaVaiTro"] = new SelectList(vaiTroList, "MaVaiTro", "DisplayText");
        }
        private async Task LoadData(TaiKhoan taiKhoan)
        {
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
        }
    }
}
