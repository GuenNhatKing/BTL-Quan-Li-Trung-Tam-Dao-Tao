using Microsoft.EntityFrameworkCore;
using QLTTDT.Data;
using QLTTDT.Models;

namespace QLTTDT.Services
{
    public class ValidCheck
    {
        private QLTTDTDbContext _context;
        public string ErrorKey { get; set; }
        public string Error { get; set; }
        public ValidCheck(QLTTDTDbContext context)
        {
            _context = context;
        }
        public async Task<bool> AccountValidation(TaiKhoan taiKhoan)
        {
            if (await IsUsernameExist(taiKhoan.TenDangNhap))
            {
                ErrorKey = "TenDangNhap";
                Error = "Tên đăng nhập đã tồn tại.";
                return false;
            }
            if (!await IsRoleExist(taiKhoan.MaVaiTro))
            {
                ErrorKey = "MaVaiTro";
                Error = "Vai trò không tồn tại.";
                return false;
            }
            if (!await IsUserExist(taiKhoan.MaNguoiDung))
            {
                ErrorKey = "MaNguoiDung";
                Error = "Người dùng không tồn tại.";
                return false;
            }
            return true;
        }
        public async Task<bool> UserValidation(NguoiDung nguoiDung)
        {
            Console.WriteLine("Goi ham user checking");
            if (await IsPhoneNumberExist(nguoiDung.SoDienThoai))
            {
                ErrorKey = "SoDienThoai";
                Error = "Số điện thoại đã tồn tại.";
                return false;
            }
            if (await IsEmailExist(nguoiDung.Email))
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
    }
}
