using System;
using System.Collections.Generic;

namespace EcomerceMVC.Data;

public partial class HoaDon
{
    public int MaHd { get; set; }

    public int MaKh { get; set; }

    public DateOnly NgayDat { get; set; }

    public DateOnly? NgayGiao { get; set; }

    public string? HoTen { get; set; }

    public string? DiaChi { get; set; }

    public string? CachThanhToan { get; set; }

    public string? Sdt { get; set; }

    public string? TrangThai { get; set; }

    public virtual ICollection<ChiTietHd> ChiTietHds { get; set; } = new List<ChiTietHd>();

    public virtual User MaKhNavigation { get; set; } = null!;
}
