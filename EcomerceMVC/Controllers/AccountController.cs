using Microsoft.AspNetCore.Mvc;
using EcomerceMVC.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using EcomerceMVC.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EcomerceMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly DbecomerceContext _context;

        public AccountController(DbecomerceContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login() => View();

        public IActionResult Register() => View();

        // ==== [ĐĂNG NHẬP] ====
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string hashedPassword = HashPassword(model.Password);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.TaiKhoan == model.Username && u.MatKhau == hashedPassword);

            if (user == null)
            {
                ModelState.AddModelError("", "Sai tài khoản hoặc mật khẩu.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.TaiKhoan),
                new Claim(ClaimTypes.Role, user.VaiTro ?? "user"),
                new Claim("UserId", user.MaKh.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

        // ==== [ĐĂNG KÝ] ====
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.TaiKhoan == model.Username);

            if (existingUser != null)
            {
                ModelState.AddModelError("", "Vui lòng đặt tên tài khoản khác.");
                return View(model);
            }

            var hashedPassword = HashPassword(model.Password);

            var user = new User
            {
                TaiKhoan = model.Username,
                MatKhau = hashedPassword,
                VaiTro = "user"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Đăng ký thành công. Vui lòng đăng nhập.";
            return RedirectToAction("Index", "Home");
        }

        // ==== [ĐĂNG XUẤT] ====
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // ==== [GOOGLE LOGIN] ====
        [HttpGet("account/GoogleLogin")]
        public IActionResult ExternalLogin(string provider, string returnUrl = "/")
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, provider);
        }

        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = "/")
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded)
                return RedirectToAction("LoginFailed");

            var claims = result.Principal.Identities
                .FirstOrDefault()?.Claims.Select(claim => new
                {
                    claim.Type,
                    claim.Value
                });

            var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var providerKey = claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            // Kiểm tra xem đã có user với GoogleID này chưa
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.GgId == providerKey);

            if (existingUser == null)
            {
                // Chưa có => tạo mới
                var newUser = new User
                {
                    GgId = providerKey,
                    Email = email,
                    HoTen = name,
                    VaiTro = "user"
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();
            }

            // Sau khi xử lý xong, cho đăng nhập như bình thường
            return Redirect(returnUrl ?? "/");
        }


        public IActionResult LoginFailed()
        {
            return Content("Đăng nhập thất bại.");
        }

        // ==== [HÀM MÃ HÓA MẬT KHẨU MD5] ====
        private string HashPassword(string password)
        {
            using (var md5 = MD5.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = md5.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
