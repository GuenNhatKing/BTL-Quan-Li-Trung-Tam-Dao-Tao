using System.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using QLTTDT.Data;
using QLTTDT.Services;
using QLTTDT.ViewModels;

namespace QLTTDT.Controllers
{
    public class AuthenticationController : Controller
    {
        private QLTTDTDbContext _context;
        public AuthenticationController(QLTTDTDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginForm loginForm, string? ReturnUrl = null)
        {
            if(ModelState.IsValid)
            {
                var login = new Login(_context, loginForm);
                if (await login.IsLoginVaild())
                {
                    var account = _context.TaiKhoans.FirstOrDefault(i => i.TenDangNhap == loginForm.Username);
                    await _context.Entry(account).Reference(i => i.MaVaiTroNavigation).LoadAsync();
                    var role = account.MaVaiTroNavigation.TenVaiTro;
                    await HttpContext.SignInAsync("AuthenticationSchema",
                    AuCookie.CreatePrincipal(account.MaNguoiDung, account.TenDangNhap, role),
                    loginForm.RememberMe? AuCookie.CreateProperties(): null);
                    if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                        return Redirect(ReturnUrl);
                    else 
                        return RedirectToAction("Index", "Home", new { area = "" });
                }
                else
                {
                    ModelState.AddModelError(login.ErrorKey, login.Error);
                }
            }
            return View(loginForm);
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterForm registerForm)
        {
            if (ModelState.IsValid)
            {
                var register = new Register(_context, registerForm);
                Console.WriteLine(register.RegisterForm.HoVaTen);
                if(await register.CreateUser())
                {
                    return RedirectToAction("Index", "Home", new { area = "" });
                }
                else
                {
                    ModelState.AddModelError(register.ErrorKey, register.Error);
                }
            }
            return View(registerForm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AuthenticationSchema");
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        public IActionResult AccessDenied(string ReturnUrl)
        {
            return View();
        }
    }
}
