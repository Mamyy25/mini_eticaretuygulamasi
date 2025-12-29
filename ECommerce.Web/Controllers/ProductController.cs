using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ECommerce.Data;
using ECommerce.Models;
using ECommerce.Web.Helpers;

namespace ECommerce.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Product/Index - Arama ve Filtreleme ile
        public async Task<IActionResult> Index(string searchTerm, string sortBy)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .Where(p => !p.IsDeleted && p.IsActive);

            // Arama
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p =>
                    p.Name.Contains(searchTerm) ||
                    p.Description.Contains(searchTerm) ||
                    p.Category.Name.Contains(searchTerm));
                ViewBag.SearchTerm = searchTerm;
            }

            // Sıralama
            query = sortBy switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "name_asc" => query.OrderBy(p => p.Name),
                "name_desc" => query.OrderByDescending(p => p.Name),
                "newest" => query.OrderByDescending(p => p.CreatedAt),
                _ => query.OrderBy(p => p.Name) // Varsayılan: İsme göre A-Z
            };

            ViewBag.SortBy = sortBy;

            var products = await query.ToListAsync();
            return View(products);
        }

        // GET: Product/Category/1 - Kategoriye göre ürünler
        public async Task<IActionResult> Category(int id, string searchTerm, string sortBy)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                TempData["Error"] = "Kategori bulunamadı!";
                return RedirectToAction("Index");
            }

            var query = _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == id && !p.IsDeleted && p.IsActive);

            // Arama
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p =>
                    p.Name.Contains(searchTerm) ||
                    p.Description.Contains(searchTerm));
                ViewBag.SearchTerm = searchTerm;
            }

            // Sıralama
            query = sortBy switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "name_asc" => query.OrderBy(p => p.Name),
                "name_desc" => query.OrderByDescending(p => p.Name),
                "newest" => query.OrderByDescending(p => p.CreatedAt),
                _ => query.OrderBy(p => p.Name)
            };

            ViewBag.SortBy = sortBy;
            ViewBag.CategoryName = category.Name;
            ViewBag.CategoryId = id;

            var products = await query.ToListAsync();
            return View("Index", products);
        }

        // GET: Product/Details/5 - HERKES GÖREBİLİR
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);

            if (product == null)
                return NotFound();

            return View(product);
        }

        // GET: Product/Create - SADECE ADMIN
        public IActionResult Create()
        {
            if (!AuthHelper.IsAdmin(HttpContext))
            {
                TempData["Error"] = "Bu işlem için yönetici yetkisi gereklidir!";
                return RedirectToAction("Index");
            }

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Product/Create - SADECE ADMIN
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product)
        {
            if (!AuthHelper.IsAdmin(HttpContext))
            {
                TempData["Error"] = "Bu işlem için yönetici yetkisi gereklidir!";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                product.CreatedAt = DateTime.Now;
                _context.Add(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Ürün başarıyla eklendi!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Product/Edit/5 - SADECE ADMIN
        public async Task<IActionResult> Edit(int? id)
        {
            if (!AuthHelper.IsAdmin(HttpContext))
            {
                TempData["Error"] = "Bu işlem için yönetici yetkisi gereklidir!";
                return RedirectToAction("Index");
            }

            if (id == null)
                return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null || product.IsDeleted)
                return NotFound();

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Product/Edit/5 - SADECE ADMIN
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (!AuthHelper.IsAdmin(HttpContext))
            {
                TempData["Error"] = "Bu işlem için yönetici yetkisi gereklidir!";
                return RedirectToAction("Index");
            }

            if (id != product.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    product.UpdatedAt = DateTime.Now;
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Ürün başarıyla güncellendi!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Product/Delete/5 - SADECE ADMIN
        public async Task<IActionResult> Delete(int? id)
        {
            if (!AuthHelper.IsAdmin(HttpContext))
            {
                TempData["Error"] = "Bu işlem için yönetici yetkisi gereklidir!";
                return RedirectToAction("Index");
            }

            if (id == null)
                return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);

            if (product == null)
                return NotFound();

            return View(product);
        }

        // POST: Product/Delete/5 - SADECE ADMIN
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!AuthHelper.IsAdmin(HttpContext))
            {
                TempData["Error"] = "Bu işlem için yönetici yetkisi gereklidir!";
                return RedirectToAction("Index");
            }

            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                product.IsDeleted = true;
                product.UpdatedAt = DateTime.Now;
                _context.Update(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Ürün başarıyla silindi!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id && !e.IsDeleted);
        }
    }
}