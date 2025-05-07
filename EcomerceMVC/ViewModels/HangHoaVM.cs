namespace ECommerceMVC.ViewModels
{
    public class HangHoaVM
    {
        public int MaHh { get; set; }  // Sửa từ Math thành MaHh (Mã hàng hóa)
        public string TenHH { get; set; }  // Tên hàng hóa
        public string Hinh { get; set; }  // Đường dẫn hình ảnh
        public double DonGia { get; set; }  // Đơn giá
        public string MoTaNgan { get; set; }  // Mô tả ngắn
        public string TenLoai { get; set; }  // Tên loại hàng hóa
    }

    public class ChiTietHangHoaVM
    {

        public int MaHh { get; set; }
        public string TenHH { get; set; }
        public string Hinh { get; set; }
        public double DonGia { get; set; }
        public string MoTaNgan { get; set; }
        public string TenLoai { get; set; }
        public string ChiTiet { get; set; }
        public int DiemDanhGia { get; set; }
        public int SoluongTon { get; set; }

    }
}