using Microsoft.AspNetCore.Mvc;
using EcomerceMVC.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using EcomerceMVC.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using System.Security.Claims;
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
        public IActionResult Register() => View();
        public IActionResult Login() => View();

        

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
            // Xóa cookie xác thực, bao gồm tất cả claims (Name, Role, UserId) được lưu trong cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Xóa tất cả cookie khác để đảm bảo không còn dữ liệu người dùng
            foreach (var cookie in HttpContext.Request.Cookies.Keys)
            {
                HttpContext.Response.Cookies.Delete(cookie);
            }

            // Xóa dữ liệu phiên (nếu có sử dụng session) để đảm bảo không còn thông tin người dùng
           // HttpContext.Session.Clear();

            // Ngăn trình duyệt lưu trữ cache để tránh truy cập dữ liệu cũ
            Response.Headers["Cache-Control"] = "no-cache, no-store, must-revalidate";
            Response.Headers["Pragma"] = "no-cache";
            Response.Headers["Expires"] = "0";

            // Chuyển hướng về trang chủ
            return RedirectToAction("Index", "Home");
        }

        // ==== [GOOGLE LOGIN] ====
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string? returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, provider);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null)
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded || authenticateResult.Principal == null)
            {
                TempData["ErrorMessage"] = "Không thể đăng nhập bằng Google.";
                return RedirectToAction("Login");
            }

            var claims = authenticateResult.Principal.Claims.ToList();

            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var providerKey = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(providerKey) || string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Không thể lấy thông tin từ tài khoản Google.";
                return RedirectToAction("Login");
            }

            // Kiểm tra tài khoản đã tồn tại chưa
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.GgId == providerKey);
            User user;
            if (existingUser == null)
            {
                user = new User
                {
                    GgId = providerKey,
                    Email = email,
                    HoTen = name ?? "Unknown",
                    VaiTro = "user"
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            else
            {
                user = existingUser;
            }

            // Tạo claims cho đăng nhập
            var userClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, user.VaiTro ?? "user"),
                    new Claim("UserId", user.MaKh.ToString())
                };

            var identity = new ClaimsIdentity(userClaims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            // Đăng nhập
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            //await HttpContext.SignInAsync(
            //  CookieAuthenticationDefaults.AuthenticationScheme,
            //  new ClaimsPrincipal(claimsIdentity));

            return Redirect(string.IsNullOrEmpty(returnUrl) ? "/" : returnUrl);
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
