using EcomerceMVC.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcomerceMVC.Controllers
{
    //[Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly DbecomerceContext _context;

        public AdminController(DbecomerceContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["CategoryCount"] = await _context.Loais.CountAsync();
            ViewData["ProductCount"] = await _context.HangHoas.CountAsync();

            var chartData = await _context.Loais
                .Select(l => new { l.TenLoai, Count = l.HangHoas.Count })
                .ToListAsync();
            ViewData["ChartData"] = chartData;

            return View();
        }
    }
}
