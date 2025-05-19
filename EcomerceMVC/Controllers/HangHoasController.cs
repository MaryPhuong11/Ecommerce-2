using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EcomerceMVC.Data;
using EcomerceMVC.ViewModels;
using Microsoft.Extensions.Hosting;

namespace EcomerceMVC.Controllers
{
    public class HangHoasController : Controller
    {
        private readonly DbecomerceContext _context;
        private readonly IWebHostEnvironment _env;

        public HangHoasController(DbecomerceContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;

        }

        // GET: HangHoas
        // GET: HangHoa
        public IActionResult Index(string? searchTerm, int? categoryId)
        {
            var model = new ProductViewModel
            {
                SearchTerm = searchTerm,
                SelectedCategoryId = categoryId,
                Categories = _context.Loais.ToList()
            };

            var query = _context.HangHoas
                .Include(h => h.MaLoaiNavigation)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(h => h.TenHh.Contains(searchTerm));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(h => h.MaLoai == categoryId);
            }

            model.Products = query.ToList();

            return View(model);
        }

        // GET: HangHoas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hangHoa = await _context.HangHoas
                .Include(h => h.MaLoaiNavigation)
                .FirstOrDefaultAsync(m => m.MaHh == id);
            if (hangHoa == null)
            {
                return NotFound();
            }

            return View(hangHoa);
        }

        // GET
        public IActionResult Create()
        {
            var vm = new HangHoaCreateViewModel
            {
                LoaiList = _context.Loais
                    .Select(l => new SelectListItem
                    {
                        Value = l.Maloai.ToString(),
                        Text = l.TenLoai
                    }).ToList()
            };
            return View(vm);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(HangHoaCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.LoaiList = _context.Loais
                    .Select(l => new SelectListItem
                    {
                        Value = l.Maloai.ToString(),
                        Text = l.TenLoai
                    }).ToList();
                return View(model);
            }

            string fileName = string.Empty;
            if (model.Hinh != null)
            {
                // Upload file
                fileName = Path.GetFileName(model.Hinh.FileName);
                string path = Path.Combine(_env.WebRootPath, "images", fileName);
                using var stream = new FileStream(path, FileMode.Create);
                model.Hinh.CopyTo(stream);
            }

            var hangHoa = new HangHoa
            {
                TenHh = model.TenHh,
                MaLoai = model.MaLoai,
                SoLuong = model.SoLuong,
                DonGia = model.DonGia,
                MoTa = model.MoTa,
                Hinh = fileName
            };

            _context.HangHoas.Add(hangHoa);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

             var hangHoa = await _context.HangHoas
            .Include(h => h.MaLoaiNavigation)
            .FirstOrDefaultAsync(h => h.MaHh == id);

            if (hangHoa == null)
            {
                return NotFound();
            }


            var model = new HangHoaEditViewModel
            {
                MaHh = hangHoa.MaHh,
                TenHh = hangHoa.TenHh,
                MaLoai = hangHoa.MaLoai,
                SoLuong = hangHoa.SoLuong,
                DonGia = hangHoa.DonGia,
                MoTa = hangHoa.MoTa,
                HinhHienTai = hangHoa.Hinh,
                LoaiList = _context.Loais
                    .Select(l => new SelectListItem
                    {
                        Value = l.Maloai.ToString(),
                        Text = l.TenLoai
                    }).ToList()
            };

            return View(model);
        }

        // POST: HangHoas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(HangHoaEditViewModel model)
        {
            //if (!ModelState.IsValid)
            //{
            //    model.LoaiList = _context.Loais
            //        .Select(l => new SelectListItem
            //        {
            //            Value = l.Maloai.ToString(),
            //            Text = l.TenLoai
            //        }).ToList();
            //    return View(model);

            //}

            
           

            var hangHoa = await _context.HangHoas.FindAsync(model.MaHh);
            if (hangHoa == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin sản phẩm
            hangHoa.TenHh = model.TenHh;
            hangHoa.MaLoai = model.MaLoai;
            hangHoa.SoLuong = model.SoLuong;
            hangHoa.DonGia = model.DonGia;
            hangHoa.MoTa = model.MoTa;

            // Xử lý hình ảnh
            if (model.Hinh != null)
            {
                // Lưu hình ảnh mới
                string fileName = Path.GetFileName(model.Hinh.FileName);
                string path = Path.Combine(_env.WebRootPath, "images", fileName);
                using var stream = new FileStream(path, FileMode.Create);
                await model.Hinh.CopyToAsync(stream);
                hangHoa.Hinh = fileName;
            }
            // Nếu không có hình ảnh mới, giữ nguyên hình ảnh hiện tại

            try
            {
                _context.Update(hangHoa);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HangHoaExists(model.MaHh))
                {
                    return NotFound();
                }
                throw;
            }

            return RedirectToAction(nameof(Index));
        }



        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hangHoa = await _context.HangHoas.FindAsync(id);
            if (hangHoa == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại." });
            }
            try
            {
                if (!string.IsNullOrEmpty(hangHoa.Hinh))
                {
                    var imagePath = Path.Combine(_env.WebRootPath, "images", hangHoa.Hinh);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }
                _context.HangHoas.Remove(hangHoa);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Xóa sản phẩm thành công." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi khi xóa sản phẩm: {ex.Message}" });
            }
        }

        private bool HangHoaExists(int id)
        {
            return _context.HangHoas.Any(e => e.MaHh == id);
        }
    }
}
