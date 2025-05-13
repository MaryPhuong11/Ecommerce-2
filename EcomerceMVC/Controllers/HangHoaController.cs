using EcomerceMVC.Data;
using ECommerceMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceMVC.Controllers
{
    public class HangHoaController : Controller
    {
        private readonly DbecomerceContext db;

        public HangHoaController(DbecomerceContext context)
        {
            db = context;
        }

        public IActionResult Index(int? loai)
        {
            // Lấy danh sách hàng hóa từ database
            var query = db.HangHoas.AsQueryable();

            // Lọc theo loại nếu có tham số loai
            if (loai.HasValue)
            {
                query = query.Where(p => p.MaLoai == loai.Value);
            }

            // Chuyển đổi sang ViewModel
            var result = query.Select(p => new HangHoaVM
            {
                MaHh = p.MaHh,
                TenHH = p.TenHh ?? "Không có tên",
                DonGia = p.DonGia ,
                Hinh = p.Hinh ?? "no-image.jpg",
                MoTaNgan = p.MoTa ?? string.Empty,
                TenLoai = p.MaLoaiNavigation.TenLoai
            }).ToList();

            return View(result);
        }

        public IActionResult Search(string? query)
        {
            // Lấy danh sách hàng hóa từ database dưới dạng truy vấn LINQ
            var hangHoas = db.HangHoas.AsQueryable();

            // Nếu có query tìm kiếm
            if (query != null)
            {
                // Lọc danh sách theo tên hàng hóa chứa chuỗi query
                hangHoas = hangHoas.Where(p => p.TenHh.Contains(query));
            }

            // Chọn và ánh xạ dữ liệu sang ViewModel
            var result = hangHoas.Select(p => new HangHoaVM
            {
                MaHh = p.MaHh,
                TenHH = p.TenHh,
                DonGia = p.DonGia,
                Hinh = p.Hinh ?? "",
                MoTaNgan = p.MoTa ?? "",
                TenLoai = p.MaLoaiNavigation.TenLoai
            });

            // Trả kết quả ra view
            return View(result);
        }

        public IActionResult Detail(int id)
        {
            var data = db.HangHoas
            .Include(p => p.MaLoaiNavigation)
            .SingleOrDefault(p => p.MaHh == id);
            if (data == null)
            {
                TempData["Message"] = $"Không thấy sản phẩm có mã ýid$";
                return Redirect("/404");
            }

            var result = new ChiTietHangHoaVM
            {
                MaHh = data.MaHh,
                TenHH = data.TenHh,
                DonGia = data.DonGia,
                ChiTiet = data.MoTa ?? string.Empty,
                Hinh = data.Hinh ?? string.Empty,
                MoTaNgan = data.MoTa ?? string.Empty,
                TenLoai = data.MaLoaiNavigation.TenLoai,
                SoluongTon = 10,//tinh sau
                DiemDanhGia = 5,//check sau
            };
            return View(result);
        }

    }
}
