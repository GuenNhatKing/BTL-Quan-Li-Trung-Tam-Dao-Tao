using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLTTDT.Data;
using QLTTDT.Models;
using QLTTDT.Services;
using QLTTDT.ViewModels;

namespace QLTTDT.Controllers
{
    [Authorize(Roles = "HocVien, Admin")]
    public class ProfilesController : Controller
    {
        private QLTTDTDbContext _context;
        private IWebHostEnvironment _webHost;
        public ProfilesController(QLTTDTDbContext context, IWebHostEnvironment webHost)
        {
            _context = context;
            _webHost = webHost;
        }
        public async Task<IActionResult> Index(string? username, int? id)
        {
            if (username == null || id == null)
            {
                return NotFound();
            }
            int? currUserId = GetUserId();
            string? currUsername = HttpContext.User.Identity?.Name;
            if (currUserId == null || currUsername == null || username != currUsername || id != currUserId)
            {
                return RedirectToAction("AccessDenied", "Authentication", new { area = "" });
            }
            var user = await _context.NguoiDungs.FirstOrDefaultAsync(i => i.MaNguoiDung == id);
            if (user == null)
            {
                return NotFound();
            }
            var profiles = new Profiles
            {
                MaNguoiDung = user.MaNguoiDung,
                Email = user.Email,
                HoVaTen = user.HoVaTen,
                NgaySinh = user.NgaySinh,
                NgaySinhCompute = user.ThoiGianCompute,
                SoDienThoai = user.SoDienThoai,
                UrlAnhDaiDien = user.UrlAnhDaiDien,
                SoKhoaHocDaDangKi = await _context.DangKiKhoaHocs.CountAsync(i => i.MaHocVien == user.MaNguoiDung),
                SoKhoaHocDangHoc = await _context.DangKiKhoaHocs.CountAsync(i => i.MaHocVien == user.MaNguoiDung
                && (_context.KhoaHocs.FirstOrDefault(j => j.MaKhoaHoc == i.MaKhoaHoc)!).ThoiGianKhaiGiang <= DateTime.Now && i.TienDo < 100),
                SoKhoaHocDaHoanThanh = await _context.DangKiKhoaHocs.CountAsync(i => i.MaHocVien == user.MaNguoiDung && i.TienDo == 100),
            };
            return View(profiles);
        }
        public async Task<IActionResult> ChangeProfiles(string? username, int? id)
        {
            if (username == null || id == null)
            {
                return NotFound();
            }
            int? currUserId = GetUserId();
            string? currUsername = HttpContext.User.Identity?.Name;
            if (currUserId == null || currUsername == null || username != currUsername || id != currUserId)
            {
                return RedirectToAction("AccessDenied", "Authentication", new { area = "" });
            }
            var user = await _context.NguoiDungs.FirstOrDefaultAsync(i => i.MaNguoiDung == id);
            if (user == null)
            {
                return NotFound();
            }
            var profiles = new Profiles
            {
                MaNguoiDung = user.MaNguoiDung,
                Email = user.Email,
                HoVaTen = user.HoVaTen,
                NgaySinh = user.NgaySinh,
                NgaySinhCompute = user.ThoiGianCompute,
                SoDienThoai = user.SoDienThoai,
                UrlAnhDaiDien = user.UrlAnhDaiDien,
                SoKhoaHocDaDangKi = await _context.DangKiKhoaHocs.CountAsync(i => i.MaHocVien == user.MaNguoiDung),
                SoKhoaHocDangHoc = await _context.DangKiKhoaHocs.CountAsync(i => i.MaHocVien == user.MaNguoiDung
                && (_context.KhoaHocs.FirstOrDefault(j => j.MaKhoaHoc == i.MaKhoaHoc)!).ThoiGianKhaiGiang <= DateTime.Now && i.TienDo < 100),
                SoKhoaHocDaHoanThanh = await _context.DangKiKhoaHocs.CountAsync(i => i.MaHocVien == user.MaNguoiDung && i.TienDo == 100),
            };
            return View(profiles);
        }
        [HttpPost]
        public async Task<IActionResult> ChangeProfiles(string? username, int? id, Profiles profiles, IFormFile? AnhDaiDien = null)
        {
            if (username == null || id == null)
            {
                return NotFound();
            }
            int? currUserId = GetUserId();
            string? currUsername = HttpContext.User.Identity?.Name;
            if (currUserId == null || currUsername == null || username != currUsername || id != currUserId)
            {
                return RedirectToAction("AccessDenied", "Authentication", new { area = "" });
            }
            var user = await _context.NguoiDungs.FirstOrDefaultAsync(i => i.MaNguoiDung == id);
            if (user == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    user.HoVaTen = profiles.HoVaTen;
                    user.NgaySinh = profiles.NgaySinh;
                    user.Email = profiles.Email;
                    user.SoDienThoai = profiles.SoDienThoai;
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                    var imageUpload = new ImageUpload(_webHost);
                    if (await imageUpload.SaveImageAs(AnhDaiDien!))
                    {
                        imageUpload.DeleteImage(user.UrlAnhDaiDien!);
                        user.UrlAnhDaiDien = imageUpload.FileName;
                    }
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return BadRequest(ex.Message);
                }
                return RedirectToAction("Index", new { username = username, id = id });
            }
            return View(profiles);
        }
        private int? GetUserId()
        {
            return Convert.ToInt32(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value.ToString());
        }
    }
}
