using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class DangKiKhoaHocController : Controller
    {
        private readonly QLTTDTDbContext _context;

        public DangKiKhoaHocController(QLTTDTDbContext context)
        {
            _context = context;
        }

        // GET: Admin/DangKiKhoaHoc
        public async Task<IActionResult> Index(string searchString)
        {
            var dkkhs = _context.DangKiKhoaHocs.IgnoreQueryFilters().Include(d => d.MaHocVienNavigation)
                .Include(d => d.MaKhoaHocNavigation).Select(i => i);
            if (!String.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToUpper();
                dkkhs = dkkhs.Where(i => i.ThoiGianDangKi.ToString().ToUpper().Contains(searchString)
                || i.MaHocVienNavigation.Email.ToUpper().Contains(searchString)
                || i.MaKhoaHocNavigation.TenKhoaHoc.ToUpper().Contains(searchString));
            }
            return View(await dkkhs.ToListAsync());
        }

        // GET: Admin/DangKiKhoaHoc/Details/5
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

        // GET: Admin/DangKiKhoaHoc/Create
        public IActionResult Create()
        {
            var hocVienList = _context.TaiKhoans
            .Include(i => i.MaNguoiDungNavigation)
            .Include(i => i.MaVaiTroNavigation)
            .Where(i => i.MaVaiTroNavigation.TenVaiTro == "HocVien")
            .Select(i => new
            {
                MaHocVien = i.MaNguoiDung,
                DisplayText = i.MaNguoiDung + " - " + i.MaNguoiDungNavigation.Email,
            })
            .AsSplitQuery()
            .ToList();
            var khoaHocList = _context.KhoaHocs
            .Select(i => new
            {
                MaKhoaHoc = i.MaKhoaHoc,
                DisplayText = i.MaKhoaHoc + " - " + i.TenKhoaHoc
            }).ToList();
            ViewData["MaHocVien"] = new SelectList(hocVienList, "MaHocVien", "DisplayText");
            ViewData["MaKhoaHoc"] = new SelectList(khoaHocList, "MaKhoaHoc", "DisplayText");
            return View();
        }

        // POST: Admin/DangKiKhoaHoc/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaDangKi,MaHocVien,MaKhoaHoc,ThoiGianDangKi,TienDo,DaHuy")] DangKiKhoaHoc dangKiKhoaHoc)
        {
            var hocVienList = _context.TaiKhoans
            .Include(i => i.MaNguoiDungNavigation)
            .Include(i => i.MaVaiTroNavigation)
            .Where(i => i.MaVaiTroNavigation.TenVaiTro == "HocVien")
            .Select(i => new
            {
                MaHocVien = i.MaNguoiDung,
                DisplayText = i.MaNguoiDung + " - " + i.MaNguoiDungNavigation.Email,
            })
            .AsSplitQuery()
            .ToList();
            var khoaHocList = _context.KhoaHocs
            .Select(i => new
            {
                MaKhoaHoc = i.MaKhoaHoc,
                DisplayText = i.MaKhoaHoc + " - " + i.TenKhoaHoc
            }).ToList();
            ViewData["MaHocVien"] = new SelectList(hocVienList, "MaHocVien", "DisplayText");
            ViewData["MaKhoaHoc"] = new SelectList(khoaHocList, "MaKhoaHoc", "DisplayText");
            if (ModelState.IsValid)
            {
                var validation = new ValidCheck(_context);
                if (!await validation.IsRegisterCourseVaild(dangKiKhoaHoc.MaHocVien, dangKiKhoaHoc.MaKhoaHoc))
                {
                    ModelState.AddModelError("", validation.Error);
                    return View(dangKiKhoaHoc);
                }
                dangKiKhoaHoc.HocPhi = (await _context.KhoaHocs.FirstOrDefaultAsync(i => i.MaKhoaHoc == dangKiKhoaHoc.MaKhoaHoc))?.HocPhi;
                _context.Add(dangKiKhoaHoc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dangKiKhoaHoc);
        }

        // GET: Admin/DangKiKhoaHoc/Edit/5
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
            var hocVien = await _context.NguoiDungs
            .Select(i => new
            {
                MaHocVien = i.MaNguoiDung,
                DisplayText = i.MaNguoiDung + " - " + i.Email,
            })
            .FirstOrDefaultAsync(i => i.MaHocVien == dangKiKhoaHoc.MaHocVien);
            var khoaHoc = await _context.KhoaHocs
            .Select(i => new
            {
                MaKhoaHoc = i.MaKhoaHoc,
                DisplayText = i.MaKhoaHoc + " - " + i.TenKhoaHoc,
            })
            .FirstOrDefaultAsync(i => i.MaKhoaHoc == dangKiKhoaHoc.MaKhoaHoc);
            ViewData["MaHocVien"] = hocVien?.DisplayText;
            ViewData["MaKhoaHoc"] = khoaHoc?.DisplayText;
            return View(dangKiKhoaHoc);
        }

        // POST: Admin/DangKiKhoaHoc/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
            var hocVien = await _context.NguoiDungs
             .Select(i => new
             {
                 MaHocVien = i.MaNguoiDung,
                 DisplayText = i.MaNguoiDung + " - " + i.Email,
             })
             .FirstOrDefaultAsync(i => i.MaHocVien == courseRegister.MaHocVien);
            var khoaHoc = await _context.KhoaHocs
            .Select(i => new
            {
                MaKhoaHoc = i.MaKhoaHoc,
                DisplayText = i.MaKhoaHoc + " - " + i.TenKhoaHoc,
            })
            .FirstOrDefaultAsync(i => i.MaKhoaHoc == courseRegister.MaKhoaHoc);
            ViewData["MaHocVien"] = hocVien?.DisplayText;
            ViewData["MaKhoaHoc"] = khoaHoc?.DisplayText;

            if (ModelState.IsValid)
            {
                try
                {
                    courseRegister.ThoiGianDangKi = dangKiKhoaHoc.ThoiGianDangKi;
                    courseRegister.TienDo = dangKiKhoaHoc.TienDo;
                    courseRegister.DaHuy = dangKiKhoaHoc.DaHuy;
                    if (!ValidCheck.IsProgressVaild(dangKiKhoaHoc.TienDo))
                    {
                        ModelState.AddModelError("TienDo", "Tiến độ phải có giá trị từ 0 đến 100.");
                        return View(dangKiKhoaHoc);
                    }
                    _context.Update(courseRegister);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!DangKiKhoaHocExists(courseRegister.MaDangKi))
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

        // POST: Admin/DangKiKhoaHoc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dangKiKhoaHoc = await _context.DangKiKhoaHocs.IgnoreQueryFilters().FirstOrDefaultAsync(i => i.MaDangKi == id);
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

        private bool DangKiKhoaHocExists(int id)
        {
            return _context.DangKiKhoaHocs.IgnoreQueryFilters().Any(e => e.MaDangKi == id);
        }
    }
}
