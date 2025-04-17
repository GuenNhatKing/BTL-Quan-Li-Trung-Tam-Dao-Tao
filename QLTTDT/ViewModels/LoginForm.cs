using System.ComponentModel.DataAnnotations;

namespace QLTTDT.ViewModels
{
    public class LoginForm
    {
        [Required(ErrorMessage = "Hãy nhập tên đăng nhập.")]
        public string Username { get; set; } = null!;
        [Required(ErrorMessage = "Hãy nhập mật khẩu.")]
        public string Password { get; set; } = null!;
        public bool RememberMe { get; set; }
    }
}
