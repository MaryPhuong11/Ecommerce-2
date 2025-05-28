using EcomerceMVC.Data; // Thay thế bằng namespace của bạn
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

public class CartIconViewComponent : ViewComponent
{
    private readonly DbecomerceContext _db;


    private const int _customerId = 1; // Tạm thời fix cứng, có thể thay bằng User.Identity

    public CartIconViewComponent(DbecomerceContext db)
    {
        _db = db;
    }   

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var count = await _db.GioHangs
            .Where(c => c.MaKh == _customerId)
            .CountAsync();

        return View(count);
    }
}