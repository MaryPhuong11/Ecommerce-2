using System;
using System.Collections.Generic;

namespace EcomerceMVC.Data;

public partial class Loai
{
    public int Maloai { get; set; }

    public string TenLoai { get; set; } = null!;

    public virtual ICollection<HangHoa> HangHoas { get; set; } = new List<HangHoa>();
}
