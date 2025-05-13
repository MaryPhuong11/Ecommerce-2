using System;
using System.Collections.Generic;

namespace EcomerceMVC.Data;

public partial class GioHang
{
    public int MaGh { get; set; }

    public int MaKh { get; set; }

    public int MaHh { get; set; }

    public int SoLuong { get; set; }

    public virtual HangHoa MaHhNavigation { get; set; } = null!;

    public virtual User MaKhNavigation { get; set; } = null!;
}
