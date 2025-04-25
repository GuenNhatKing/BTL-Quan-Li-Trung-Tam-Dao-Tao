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
    public class DangKiKhoaHocController : Controller
    {
        private readonly QLTTDTDbContext _context;

        public DangKiKhoaHocController(QLTTDTDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? searchString)
        {
            var dkkhs = _context.DangKiKhoaHocs
            .IgnoreQueryFilters()
            .Include(d => d.MaHocVienNavigation)
            .Include(d => d.MaKhoaHocNavigation)
            .Select(i => i);

            if (!String.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToUpper();
                dkkhs = dkkhs
                .Where(i => i.ThoiGianCompute!.ToUpper().Contains(searchString)
                || i.MaHocVienNavigation.Email.ToUpper().Contains(searchString)
                || i.MaKhoaHocNavigation.TenKhoaHoc.ToUpper().Contains(searchString));
            }
            return View(await dkkhs.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dangKiKhoaHoc = await _context.DangKiKhoaHocs
            .IgnoreQueryFilters()
            .Include(d => d.MaHocVienNavigation)
            .Include(d => d.MaKhoaHocNavigation)
            .FirstOrDefaultAsync(m => m.MaDangKi == id);
            if (dangKiKhoaHoc == null)
            {
                return NotFound();
            }

            return View(dangKiKhoaHoc);
        }

        public async Task<IActionResult> Create()
        {
            await LoadDataList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaDangKi,MaHocVien,MaKhoaHoc")] DangKiKhoaHoc dangKiKhoaHoc)
        {
            await LoadDataList();
            if (ModelState.IsValid)
            {
                var validation = new ValidCheck(_context);
                if (!await validation.IsRegisterCourseVaild(dangKiKhoaHoc.MaHocVien, dangKiKhoaHoc.MaKhoaHoc))
                {
                    ModelState.AddModelError(validation.ErrorKey, validation.Error);
                    return View(dangKiKhoaHoc);
                }
                var khoaHoc = await _context.KhoaHocs.FirstOrDefaultAsync(i => i.MaKhoaHoc == dangKiKhoaHoc.MaKhoaHoc);
                if (khoaHoc == null)
                {
                    ModelState.AddModelError("MaKhoaHoc", "Khoá học không tồn tại");
                }
                else
                {
                    dangKiKhoaHoc.HocPhi = khoaHoc.HocPhi;
                }
                _context.Add(dangKiKhoaHoc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dangKiKhoaHoc);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dangKiKhoaHoc = await _context.DangKiKhoaHocs
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(i => i.MaDangKi == id);
            if (dangKiKhoaHoc == null)
            {
                return NotFound();
            }
            await LoadData(dangKiKhoaHoc);
            return View(dangKiKhoaHoc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("MaDangKi,ThoiGianDangKi,TienDo,DaHuy")] DangKiKhoaHoc dangKiKhoaHoc)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courseRegister = await _context.DangKiKhoaHocs
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(i => i.MaDangKi == id);
            if (courseRegister == null)
            {
                return NotFound();
            }
            await LoadData(courseRegister);

            if (ModelState.IsValid)
            {
                try
                {
                    if (!ValidCheck.IsProgressValid(dangKiKhoaHoc.TienDo))
                    {
                        ModelState.AddModelError("TienDo", "Tiến độ phải có giá trị từ 0 đến 100.");
                        return View(dangKiKhoaHoc);
                    }
                    courseRegister.ThoiGianDangKi = dangKiKhoaHoc.ThoiGianDangKi;
                    courseRegister.TienDo = dangKiKhoaHoc.TienDo;
                    courseRegister.DaHuy = dangKiKhoaHoc.DaHuy;
                    _context.Update(courseRegister);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return BadRequest(ex.Message);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(dangKiKhoaHoc);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dangKiKhoaHoc = await _context.DangKiKhoaHocs
                .IgnoreQueryFilters()
                .Include(d => d.MaHocVienNavigation)
                .Include(d => d.MaKhoaHocNavigation)
                .FirstOrDefaultAsync(m => m.MaDangKi == id);
            if (dangKiKhoaHoc == null)
            {
                return NotFound();
            }

            return View(dangKiKhoaHoc);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dangKiKhoaHoc = await _context.DangKiKhoaHocs
                .IgnoreQueryFilters()
                .Include(d => d.MaHocVienNavigation)
                .Include(d => d.MaKhoaHocNavigation)
                .FirstOrDefaultAsync(m => m.MaDangKi == id);
            if (dangKiKhoaHoc != null)
            {
                try
                {
                    _context.DangKiKhoaHocs.Remove(dangKiKhoaHoc);
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
            var hocVienList = await _context.TaiKhoans
            .Include(i => i.MaNguoiDungNavigation)
            .Include(i => i.MaVaiTroNavigation)
            .Where(i => i.MaVaiTroNavigation.TenVaiTro == "HocVien")
            .Select(i => new
            {
                MaHocVien = i.MaNguoiDung,
                DisplayText = i.MaNguoiDung + " - " + i.MaNguoiDungNavigation.Email,
            })
            .AsSplitQuery()
            .ToListAsync();

            var khoaHocList = await _context.KhoaHocs
            .Select(i => new
            {
                MaKhoaHoc = i.MaKhoaHoc,
                DisplayText = i.MaKhoaHoc + " - " + i.TenKhoaHoc
            }).ToListAsync();

            ViewData["MaHocVien"] = new SelectList(hocVienList, "MaHocVien", "DisplayText");
            ViewData["MaKhoaHoc"] = new SelectList(khoaHocList, "MaKhoaHoc", "DisplayText");
        }
        private async Task LoadData(DangKiKhoaHoc dkkh)
        {
            var hocVien = await _context.NguoiDungs
            .Select(i => new
            {
                MaHocVien = i.MaNguoiDung,
                DisplayText = i.MaNguoiDung + " - " + i.Email,
            })
            .FirstOrDefaultAsync(i => i.MaHocVien == dkkh.MaHocVien);

            var khoaHoc = await _context.KhoaHocs
            .Select(i => new
            {
                MaKhoaHoc = i.MaKhoaHoc,
                DisplayText = i.MaKhoaHoc + " - " + i.TenKhoaHoc,
            })
            .FirstOrDefaultAsync(i => i.MaKhoaHoc == dkkh.MaKhoaHoc);

            ViewData["MaHocVien"] = hocVien?.DisplayText;
            ViewData["MaKhoaHoc"] = khoaHoc?.DisplayText;
        }
    }
}
