using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EcomerceMVC.ViewModels
{
    public class HangHoaEditViewModel
    {
        public int MaHh { get; set; }

        [Required(ErrorMessage = "Tên hàng hóa là bắt buộc")]
        public string TenHh { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại hàng hóa")]
        public int MaLoai { get; set; }

        [Required(ErrorMessage = "Số lượng là bắt buộc")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0")]
        public int SoLuong { get; set; }

        [Required(ErrorMessage = "Đơn giá là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Đơn giá phải lớn hơn hoặc bằng 0")]
        public decimal DonGia { get; set; }

        public string MoTa { get; set; }

        public string? HinhHienTai { get; set; }

        public IFormFile? Hinh { get; set; }

        public List<SelectListItem> LoaiList { get; set; }
    }
}
