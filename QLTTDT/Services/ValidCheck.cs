using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using QLTTDT.Data;
using QLTTDT.Models;
using QLTTDT.ViewModels;

namespace QLTTDT.Services
{
    public class ValidCheck
    {
        private QLTTDTDbContext _context;
        public string ErrorKey { get; set; } = null!;
        public string Error { get; set; } = null!;
        public ValidCheck(QLTTDTDbContext context)
        {
            _context = context;
        }
        public async Task<bool> AccountValidation(string username, string password, int role, int userId)
        {
            if (password.Length < 6)
            {
                ErrorKey = "MatKhau";
                Error = "Mật khẩu quá ngắn.";
                return false;
            }
            if (await IsUsernameExist(username))
            {
                ErrorKey = "TenDangNhap";
                Error = "Tên đăng nhập đã tồn tại.";
                return false;
            }
            if (!await IsRoleExist(role))
            {
                ErrorKey = "MaVaiTro";
                Error = "Vai trò không tồn tại.";
                return false;
            }
            if (!await IsUserExist(userId))
            {
                ErrorKey = "MaNguoiDung";
                Error = "Người dùng không tồn tại.";
                return false;
            }
            return true;
        }
        public async Task<bool> UserValidation(string phone, string email)
        {
            if (!Regex.IsMatch(phone, "^(0?)(3[2-9]|5[6|8|9]|7[0|6-9]|8[0-6|8|9]|9[0-4|6-9])[0-9]{7}$"))
            {
                ErrorKey = "SoDienThoai";
                Error = "Số điện thoại không đúng định dạng.";
                return false;
            }
            if (!Regex.IsMatch(email, "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$"))
            {
                ErrorKey = "Email";
                Error = "Địa chỉ email không đúng định dạng.";
                return false;
            }
            if (await IsPhoneNumberExist(phone))
            {
                ErrorKey = "SoDienThoai";
                Error = "Số điện thoại đã tồn tại.";
                return false;
            }
            if (await IsEmailExist(email))
            {
                ErrorKey = "Email";
                Error = "Địa chỉ email đã tồn tại.";
                return false;
            }
            return true;
        }
        public async Task<bool> IsUsernameExist(string username)
        {
            return await _context.TaiKhoans.AnyAsync(i => i.TenDangNhap == username);
        }
        public async Task<bool> IsRoleExist(int role)
        {
            return await _context.VaiTros.AnyAsync(i => i.MaVaiTro == role);
        }
        public async Task<bool> IsUserExist(int user)
        {
            return await _context.NguoiDungs.AnyAsync(i => i.MaNguoiDung == user);
        }
        public async Task<bool> IsPhoneNumberExist(string phone)
        {
            return await _context.NguoiDungs.AnyAsync(i => i.SoDienThoai == phone);
        }
        public async Task<bool> IsEmailExist(string email)
        {
            return await _context.NguoiDungs.AnyAsync(i => i.Email == email);
        }

        public async Task<bool> IsRegisterCourseVaild(int? maHocVien, int? maKhoaHoc)
        {
            ErrorKey = "";
            if (maHocVien == null || maKhoaHoc == null)
            {
                Error = "Mã người dùng hoặc mã khoá học không hợp lệ";
                return false;
            }
            if (await _context.DangKiKhoaHocs
            .AnyAsync(i => i.MaHocVien == maHocVien && i.MaKhoaHoc == maKhoaHoc))
            {
                Error = "Học viên đã đăng kí khoá học này rồi";
                return false;
            }
            if ((await _context.KhoaHocs.FirstOrDefaultAsync(i => i.MaKhoaHoc == maKhoaHoc))?.SoLuongHocVienToiDa
            <= (await _context.DangKiKhoaHocs.CountAsync(i => i.MaKhoaHoc == maKhoaHoc)))
            {
                Error = "Số lượng học viên đăng kí cho khoá học này đã đạt giới hạn";
                return false;
            }
            return true;
        }
        public static bool IsProgressVaild(int progress)
        {
            Console.WriteLine($"Progess: {progress}");
            return progress >= 0 && progress <= 100;
        }
    }
}
