using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using QLTTDT.Data;
using QLTTDT.Models;
using QLTTDT.ViewModels;
namespace QLTTDT.Services
{
    public class Login
    {
        private QLTTDTDbContext _context;
        public string ErrorKey = "";
        public string Error { get; set; }
        public LoginForm LoginForm { get; set; }

        public Login(QLTTDTDbContext context, LoginForm loginForm)
        {
            _context = context;
            LoginForm = loginForm;
        }

        public async Task<bool> IsLoginVaild()
        {
            var validation = new ValidCheck(_context);
            var taikhoan = await _context.TaiKhoans.FirstOrDefaultAsync(i => i.TenDangNhap == LoginForm.Username);
            if (taikhoan == null)
            {
                ErrorKey = "";
                Error = "Mật khẩu hoặc tài khoản không chính xác";
                return false;
            }
            if(!(taikhoan.MatKhau == GetHashedPassword(taikhoan.Salt, LoginForm.Password)))
            {
                ErrorKey = "";
                Error = "Mật khẩu hoặc tài khoản không chính xác";
                return false;
            }
            return true;
        }
        public static string GetHashedPassword(byte[] salt, string password)
        {
            string saltedPassword = password + Convert.ToBase64String(salt);
            string hashedPassword;
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                StringBuilder strbd = new StringBuilder();
                foreach (byte i in bytes)
                {
                    strbd.Append(i.ToString("x2"));
                }
                hashedPassword = strbd.ToString();
            }
            return hashedPassword;
        }
    }
}
