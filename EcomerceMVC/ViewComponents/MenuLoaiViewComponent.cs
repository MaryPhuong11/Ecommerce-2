using EcomerceMVC.Data;
using ECommerceMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceMVC.ViewComponents
{
    public class MenuLoaiViewComponent : ViewComponent
    {
        private readonly DbecomerceContext db;

        public MenuLoaiViewComponent(DbecomerceContext context)
        {
            db = context;
        }

        public IViewComponentResult Invoke()
        {
            var data = db.Loais.Select(lo => new MenuLoaiVM
            {
                MaLoai=lo.Maloai,
                TenLoai = lo.TenLoai,
                SoLuong = lo.HangHoas.Count
            }).OrderBy(p => p.TenLoai);

            return View(data); // View mặc định sẽ tìm file: Views/Shared/Components/MenuLoai/Default.cshtml
        }
    }
}
