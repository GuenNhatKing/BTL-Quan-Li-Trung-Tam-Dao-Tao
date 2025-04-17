using System.ComponentModel.DataAnnotations;

namespace QLTTDT.ViewModels
{
    public class RegisterForm
    {
        [Required(ErrorMessage = "Hãy nhập họ và tên.")]
        public string HoVaTen { get; set; } = null!;
        [Required(ErrorMessage = "Hãy nhập ngày sinh.")]
        public DateOnly NgaySinh { get; set; }
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
        [Required(ErrorMessage = "Hãy nhập số điện thoại.")]
        public string SoDienThoai { get; set; } = null!;
        [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
        [Required(ErrorMessage = "Hãy nhập email.")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage = "Hãy nhập tên đăng nhập.")]
        public string Username { get; set; } = null!;
        [Required(ErrorMessage = "Hãy nhập mật khẩu.")]
        public string Password { get; set; } = null!;
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        [Required(ErrorMessage = "Hãy nhập lại mật khẩu.")]
        public string RePassword { get; set; } = null!;
    }
}
