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
    public class KhoaHocController : Controller
    {
        private readonly QLTTDTDbContext _context;
        private readonly IWebHostEnvironment _webHost;
        public KhoaHocController(QLTTDTDbContext context, IWebHostEnvironment webHost)
        {
            _context = context;
            _webHost = webHost;
        }

        // GET: Admin/KhoaHoc
        public async Task<IActionResult> Index()
        {
            var qLTTDTDbContext = _context.KhoaHocs.Include(k => k.MaCapDoNavigation).Include(k => k.MaChuDeNavigation).Include(k => k.MaGiangVienNavigation);
            return View(await qLTTDTDbContext.ToListAsync());
        }

        // GET: Admin/KhoaHoc/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khoaHoc = await _context.KhoaHocs
                .Include(k => k.MaCapDoNavigation)
                .Include(k => k.MaChuDeNavigation)
                .Include(k => k.MaGiangVienNavigation)
                .FirstOrDefaultAsync(m => m.MaKhoaHoc == id);
            if (khoaHoc == null)
            {
                return NotFound();
            }

            return View(khoaHoc);
        }

        // GET: Admin/KhoaHoc/Create
        public IActionResult Create()
        {
            var giangVienList = _context.TaiKhoans
            .Include(i => i.MaNguoiDungNavigation)
            .Include(i => i.MaVaiTroNavigation)
            .Where(i => i.MaVaiTroNavigation.TenVaiTro == "GiangVien")
            .Select(i => new
            {
                MaGiangVien = i.MaNguoiDung,
                DisplayText = i.MaNguoiDung + " - " + i.MaNguoiDungNavigation.Email
            })
            .AsSplitQuery()
            .ToList();
            var chuDeList = _context.ChuDes
            .Select(i => new
            {
                MaChuDe = i.MaChuDe,
                DisplayText = i.MaChuDe + " - " + i.TenChuDe,
            }).ToList();
            var capDoList = _context.CapDos
            .Select(i => new
            {
                MaCapDo = i.MaCapDo,
                DisplayText = i.MaCapDo + " - " + i.TenCapDo,
            }).ToList();
            ViewData["MaCapDo"] = new SelectList(capDoList, "MaCapDo", "DisplayText");
            ViewData["MaChuDe"] = new SelectList(chuDeList, "MaChuDe", "DisplayText");
            ViewData["MaGiangVien"] = new SelectList(giangVienList, "MaGiangVien", "DisplayText");
            return View();
        }

        // POST: Admin/KhoaHoc/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaKhoaHoc,MaGiangVien,MaChuDe,MaCapDo,UrlAnh,TenKhoaHoc,MoTa,ThoiGianKhaiGiang,HocPhi,SoLuongHocVienToiDa")] KhoaHoc khoaHoc, IFormFile? AnhKhoaHoc = null)
        {
            if (ModelState.IsValid)
            {
                var imageUpload = new ImageUpload(_webHost);
                if (await imageUpload.SaveImageAs(AnhKhoaHoc))
                {
                    khoaHoc.UrlAnh = imageUpload.FileName;
                }
                _context.Add(khoaHoc);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaCapDo"] = new SelectList(_context.CapDos, "MaCapDo", "MaCapDo", khoaHoc.MaCapDo);
            ViewData["MaChuDe"] = new SelectList(_context.ChuDes, "MaChuDe", "MaChuDe", khoaHoc.MaChuDe);
            ViewData["MaGiangVien"] = new SelectList(_context.NguoiDungs, "MaNguoiDung", "MaNguoiDung", khoaHoc.MaGiangVien);
            return View(khoaHoc);
        }

        // GET: Admin/KhoaHoc/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khoaHoc = await _context.KhoaHocs.FindAsync(id);
            if (khoaHoc == null)
            {
                return NotFound();
            }
            var giangVienList = _context.TaiKhoans
            .Include(i => i.MaNguoiDungNavigation)
            .Include(i => i.MaVaiTroNavigation)
            .Where(i => i.MaVaiTroNavigation.TenVaiTro == "GiangVien")
            .Select(i => new
            {
                MaGiangVien = i.MaNguoiDung,
                DisplayText = i.MaNguoiDung + " - " + i.MaNguoiDungNavigation.Email
            })
            .AsSplitQuery()
            .ToList();
            var chuDeList = _context.ChuDes
            .Select(i => new
            {
                MaChuDe = i.MaChuDe,
                DisplayText = i.MaChuDe + " - " + i.TenChuDe,
            }).ToList();
            var capDoList = _context.CapDos
            .Select(i => new
            {
                MaCapDo = i.MaCapDo,
                DisplayText = i.MaCapDo + " - " + i.TenCapDo,
            }).ToList();
            ViewData["MaCapDo"] = new SelectList(capDoList, "MaCapDo", "DisplayText", capDoList.Find(i => i.MaCapDo == khoaHoc.MaCapDo).DisplayText);
            ViewData["MaChuDe"] = new SelectList(chuDeList, "MaChuDe", "DisplayText", chuDeList.Find(i => i.MaChuDe == khoaHoc.MaChuDe).DisplayText);
            ViewData["MaGiangVien"] = new SelectList(giangVienList, "MaGiangVien", "DisplayText", giangVienList.Find(i => i.MaGiangVien == khoaHoc.MaGiangVien).DisplayText);
            return View(khoaHoc);
        }

        // POST: Admin/KhoaHoc/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("MaKhoaHoc,TenKhoaHoc,MoTa,ThoiGianKhaiGiang,HocPhi,SoLuongHocVienToiDa")] KhoaHoc khoaHoc, IFormFile? AnhKhoaHoc)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.KhoaHocs.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            var giangVienList = _context.TaiKhoans
            .Include(i => i.MaNguoiDungNavigation)
            .Include(i => i.MaVaiTroNavigation)
            .Where(i => i.MaVaiTroNavigation.TenVaiTro == "GiangVien")
            .Select(i => new
            {
                MaGiangVien = i.MaNguoiDung,
                DisplayText = i.MaNguoiDung + " - " + i.MaNguoiDungNavigation.Email
            })
            .AsSplitQuery()
            .ToList();
            var chuDeList = _context.ChuDes
            .Select(i => new
            {
                MaChuDe = i.MaChuDe,
                DisplayText = i.MaChuDe + " - " + i.TenChuDe,
            }).ToList();
            var capDoList = _context.CapDos
            .Select(i => new
            {
                MaCapDo = i.MaCapDo,
                DisplayText = i.MaCapDo + " - " + i.TenCapDo,
            }).ToList();
            ViewData["MaCapDo"] = new SelectList(capDoList, "MaCapDo", "DisplayText", capDoList.Find(i => i.MaCapDo == khoaHoc.MaCapDo).DisplayText);
            ViewData["MaChuDe"] = new SelectList(chuDeList, "MaChuDe", "DisplayText", chuDeList.Find(i => i.MaChuDe == khoaHoc.MaChuDe).DisplayText);
            ViewData["MaGiangVien"] = new SelectList(giangVienList, "MaGiangVien", "DisplayText", giangVienList.Find(i => i.MaGiangVien == khoaHoc.MaGiangVien).DisplayText);

            if (ModelState.IsValid)
            {
                try
                {
                    course.TenKhoaHoc = khoaHoc.TenKhoaHoc;
                    course.ThoiGianKhaiGiang = khoaHoc.ThoiGianKhaiGiang;
                    course.HocPhi = khoaHoc.HocPhi;
                    course.SoLuongHocVienToiDa = khoaHoc.SoLuongHocVienToiDa;
                    course.MoTa = khoaHoc.MoTa;
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                    var imageUpload = new ImageUpload(_webHost);
                    if (await imageUpload.SaveImageAs(AnhKhoaHoc!))
                    {
                        imageUpload.DeleteImage(course.UrlAnh!);
                        course.UrlAnh = imageUpload.FileName;
                    }
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!KhoaHocExists(course.MaKhoaHoc))
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
            return View(khoaHoc);
        }

        // GET: Admin/KhoaHoc/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var khoaHoc = await _context.KhoaHocs
                .Include(k => k.MaCapDoNavigation)
                .Include(k => k.MaChuDeNavigation)
                .Include(k => k.MaGiangVienNavigation)
                .FirstOrDefaultAsync(m => m.MaKhoaHoc == id);
            if (khoaHoc == null)
            {
                return NotFound();
            }

            return View(khoaHoc);
        }

        // POST: Admin/KhoaHoc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var khoaHoc = await _context.KhoaHocs.FindAsync(id);
            if (khoaHoc != null)
            {
                try
                {
                    _context.KhoaHocs.Remove(khoaHoc);
                    await _context.SaveChangesAsync();
                    var imgDelete = new ImageUpload(_webHost);
                    imgDelete.DeleteImage(khoaHoc.UrlAnh);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        private bool KhoaHocExists(int id)
        {
            return _context.KhoaHocs.Any(e => e.MaKhoaHoc == id);
        }
    }
}
