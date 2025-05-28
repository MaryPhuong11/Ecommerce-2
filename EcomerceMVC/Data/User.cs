using System;
using System.Collections.Generic;

namespace EcomerceMVC.Data;

public partial class User
{
    public int MaKh { get; set; }

    public string? GgId { get; set; }

    public string? TaiKhoan { get; set; }

    public string? MatKhau { get; set; }

    public string? HoTen { get; set; }

    public string? Sdt { get; set; }

    public string? Email { get; set; }

    public string? DiaChi { get; set; }

    public string? GioiTinh { get; set; }

    public string? VaiTro { get; set; }

    public virtual ICollection<GioHang> GioHangs { get; set; } = new List<GioHang>();

    public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
}
