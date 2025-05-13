using System;
using System.Collections.Generic;

namespace EcomerceMVC.Data;

public partial class HangHoa
{
    public int MaHh { get; set; }

    public string TenHh { get; set; } = null!;

    public int MaLoai { get; set; }

    public int SoLuong { get; set; }

    public decimal DonGia { get; set; }

    public string? Hinh { get; set; }

    public string? MoTa { get; set; }

    public virtual ICollection<ChiTietHd> ChiTietHds { get; set; } = new List<ChiTietHd>();

    public virtual ICollection<GioHang> GioHangs { get; set; } = new List<GioHang>();

    public virtual Loai MaLoaiNavigation { get; set; } = null!;
}
