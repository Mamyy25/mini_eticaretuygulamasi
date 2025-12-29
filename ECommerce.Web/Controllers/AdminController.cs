using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerce.Data;
using ECommerce.Web.Helpers;

namespace ECommerce.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Index - Yönetim Dashboard
        public async Task<IActionResult> Index()
        {
            if (!AuthHelper.IsAdmin(HttpContext))
            {
                TempData["Error"] = "Bu iþlem için yönetici yetkisi gereklidir!";
                return RedirectToAction("Index", "Home");
            }

            // Ýstatistikler
            ViewBag.TotalCategories = await _context.Categories.CountAsync(c => !c.IsDeleted);
            ViewBag.TotalProducts = await _context.Products.CountAsync(p => !p.IsDeleted);
            ViewBag.ActiveProducts = await _context.Products.CountAsync(p => !p.IsDeleted && p.IsActive);
            ViewBag.LowStock = await _context.Products.CountAsync(p => !p.IsDeleted && p.Stock <= 10 && p.Stock > 0);
            ViewBag.OutOfStock = await _context.Products.CountAsync(p => !p.IsDeleted && p.Stock == 0);
            ViewBag.DeletedCategories = await _context.Categories.CountAsync(c => c.IsDeleted);
            ViewBag.DeletedProducts = await _context.Products.CountAsync(p => p.IsDeleted);

            return View();
        }
    }
}