using EcomerceMVC.Data;
using EcomerceMVC.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EcomerceMVC.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly DbecomerceContext db;

        public CartController(DbecomerceContext context)
        {
            db = context;
        }

        // Helper method to get MaKh from UserId claim
        private int? GetCustomerIdFromClaim()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int maKh))
            {
                return null;
            }
            return maKh;
        }

        public async Task<IActionResult> Index()
        {
            var maKh = GetCustomerIdFromClaim();
            if (!maKh.HasValue)
            {
                TempData["ErrorMessage"] = "Không thể xác định thông tin khách hàng.";
                //
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login", "Account");
            }

            var cartItems = await db.GioHangs
                .Where(c => c.MaKh == maKh.Value)
                .Include(c => c.MaHhNavigation)
                .Select(c => new CartItem
                {
                    MaHh = c.MaHh,
                    TenHh = c.MaHhNavigation.TenHh,
                    DonGia = (double)c.MaHhNavigation.DonGia,
                    Hinh = c.MaHhNavigation.Hinh ?? string.Empty,
                    SoLuong = c.SoLuong
                })
                .ToListAsync();

            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int id, int quantity = 1)
        {
            var maKh = GetCustomerIdFromClaim();
            if (!maKh.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để thêm sản phẩm vào giỏ hàng.";
                //
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login", "Account");
            }

            var item = db.GioHangs.SingleOrDefault(p => p.MaHh == id && p.MaKh == maKh.Value);
            int sl = 0;
            if (item != null)
            {
                sl = item.SoLuong;
            }

            // Kiểm tra số lượng hợp lệ
            if (quantity <= 0)
            {
                TempData["Message"] = "Số lượng phải lớn hơn 0";
                return RedirectToAction("Index");
            }

            var hangHoa = await db.HangHoas.FindAsync(id);
            if (hangHoa == null)
            {
                TempData["Message"] = $"Không tìm thấy hàng hóa có mã {id}";
                return Redirect("/404");
            }

            if (quantity + sl > hangHoa.SoLuong)
            {
                TempData["Message"] = $"Bạn đã có {sl} sản phẩm trong giỏ hàng. Không thể thêm số lượng đã chọn vào giỏ hàng vì vượt quá giới hạn.";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            // Kiểm tra sản phẩm đã có trong giỏ hàng chưa
            var cartItem = await db.GioHangs
                .FirstOrDefaultAsync(c => c.MaKh == maKh.Value && c.MaHh == id);

            if (cartItem == null)
            {
                // Thêm mới vào giỏ hàng
                var newCartItem = new GioHang
                {
                    MaKh = maKh.Value,
                    MaHh = id,
                    SoLuong = quantity
                };
                db.GioHangs.Add(newCartItem);
            }
            else
            {
                // Cập nhật số lượng nếu đã có
                cartItem.SoLuong += quantity;
                db.GioHangs.Update(cartItem);
            }

            try
            {
                await db.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã thêm sản phẩm vào giỏ hàng";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi thêm vào giỏ hàng: " + ex.Message;
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCartAsync(int id)
        {
            var maKh = GetCustomerIdFromClaim();
            if (!maKh.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để xóa sản phẩm khỏi giỏ hàng.";
                return RedirectToAction("Login", "Account");
            }

            var item = db.GioHangs.SingleOrDefault(p => p.MaHh == id && p.MaKh == maKh.Value);
            if (item != null)
            {
                db.GioHangs.Remove(item);
                await db.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> PlusAsync(int id)
        {
            var maKh = GetCustomerIdFromClaim();
            if (!maKh.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để cập nhật giỏ hàng.";
                //
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login", "Account");
            }

            var item = db.GioHangs.SingleOrDefault(p => p.MaHh == id && p.MaKh == maKh.Value);
            if (item != null)
            {
                var hangHoa = await db.HangHoas.FindAsync(id);
                if (hangHoa == null || item.SoLuong + 1 > hangHoa.SoLuong)
                {
                    TempData["Message"] = "Không thể tăng số lượng vì vượt quá tồn kho.";
                    return RedirectToAction("Index");
                }

                item.SoLuong += 1;
                db.GioHangs.Update(item);
                await db.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> MinusAsync(int id)
        {
            var maKh = GetCustomerIdFromClaim();
            if (!maKh.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để cập nhật giỏ hàng.";
                //
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Login", "Account");
            }

            var item = db.GioHangs.SingleOrDefault(p => p.MaHh == id && p.MaKh == maKh.Value);
            if (item != null)
            {
                if (item.SoLuong == 1)
                {
                    db.GioHangs.Remove(item);
                }
                else
                {
                    item.SoLuong -= 1;
                    db.GioHangs.Update(item);
                }
                await db.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
    }
}