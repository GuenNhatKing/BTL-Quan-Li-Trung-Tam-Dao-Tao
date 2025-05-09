﻿using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QLTTDT.Data;
using QLTTDT.Models;
using QLTTDT.ViewModels;
namespace QLTTDT.Services
{
    public class Register
    {
        private QLTTDTDbContext _context;
        public RegisterForm RegisterForm { get; set; }
        public string ErrorKey { get; set; } = "";
        public string Error { get; set; } = "";

        public Register(QLTTDTDbContext context, RegisterForm regForm)
        {
            _context = context;
            RegisterForm = regForm;
        }

        public async Task<bool> CreateUser()
        {
            var validation = new ValidCheck(_context);
            if (!Regex.IsMatch(RegisterForm.SoDienThoai, "^(0?)(3[2-9]|5[6|8|9]|7[0|6-9]|8[0-6|8|9]|9[0-4|6-9])[0-9]{7}$"))
            {
                ErrorKey = "SoDienThoai";
                Error = "Số điện thoại không đúng định dạng.";
                return false;
            }
            if (!Regex.IsMatch(RegisterForm.Email, "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$"))
            {
                ErrorKey = "Email";
                Error = "Địa chỉ email không đúng định dạng.";
                return false;
            }
            if (await validation.IsPhoneNumberExist(RegisterForm.SoDienThoai))
            {
                ErrorKey = "SoDienThoai";
                Error = "Số điện thoại đã tồn tại";
                return false;
            }
            if (await validation.IsEmailExist(RegisterForm.Email))
            {
                ErrorKey = "Email";
                Error = "Địa chỉ email đã tồn tại";
                return false;
            }
            if (RegisterForm.Password.Length < 6)
            {
                ErrorKey = "Password";
                Error = "Mật khẩu quá ngắn.";
                return false;
            }
            if (RegisterForm.Password != RegisterForm.RePassword)
            {
                ErrorKey = "RePassword";
                Error = "Không khớp với mật khẩu đã nhập";
                return false;
            }

            if (await validation.IsUsernameExist(RegisterForm.Username))
            {
                ErrorKey = "Username";
                Error = "Tên đăng nhập đã tồn tại";
                return false;
            }
            byte[] saltBytes = CreateSalt(_context);
            string hashedPassword = Login.GetHashedPassword(saltBytes, RegisterForm.Password);
            bool transDone = false;
            using (var trans = _context.Database.BeginTransaction())
            {
                try
                {
                    var vaiTro = await _context.VaiTros.FirstOrDefaultAsync(i => i.TenVaiTro == "HocVien");
                    if (vaiTro == null) throw new InvalidDataException("Vai trò không tồn tại");
                    var nguoiDung = new NguoiDung
                    {
                        HoVaTen = RegisterForm.HoVaTen,
                        NgaySinh = RegisterForm.NgaySinh,
                        SoDienThoai = RegisterForm.SoDienThoai,
                        Email = RegisterForm.Email,
                    };
                    _context.NguoiDungs.Add(nguoiDung);
                    await _context.SaveChangesAsync();
                    var taiKhoan = new TaiKhoan
                    {
                        TenDangNhap = RegisterForm.Username,
                        Salt = saltBytes,
                        MatKhau = hashedPassword,
                        MaNguoiDung = nguoiDung.MaNguoiDung,
                        MaVaiTro = vaiTro.MaVaiTro,
                    };
                    _context.TaiKhoans.Add(taiKhoan);
                    await _context.SaveChangesAsync();
                    await trans.CommitAsync();
                    transDone = true;
                }
                catch (InvalidDataException)
                {
                    ErrorKey = "";
                    Error = "Vai trò HocVien không tồn tại!";
                }
                catch (Exception)
                {
                    await trans.RollbackAsync();
                }
            }
            return transDone;
        }

        public static byte[] CreateSalt(QLTTDTDbContext context)
        {
            byte[] saltBytes = new byte[ConstantValues.SALT_SIZE];
            RandomNumberGenerator.Fill(saltBytes);
            while (context.TaiKhoans.Any(i => i.Salt == saltBytes))
            {
                RandomNumberGenerator.Fill(saltBytes);
            }
            return saltBytes;
        }
    }
}
