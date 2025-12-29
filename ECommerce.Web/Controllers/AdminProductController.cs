using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ECommerce.Data;
using ECommerce.Models;
using ECommerce.Web.Helpers;

namespace ECommerce.Web.Controllers
{
    public class AdminProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminProductController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: AdminProduct/Index - SADECE ADMIN
        public async Task<IActionResult> Index(bool showDeleted = false)
        {
            if (!AuthHelper.IsAdmin(HttpContext))
            {
                TempData["Error"] = "Bu iþlem için yönetici yetkisi gereklidir!";
                return RedirectToAction("Index", "Home");
            }

            var products = await _context.Products
                .Include(p => p.Category)
                .Where(p => showDeleted ? p.IsDeleted : !p.IsDeleted)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            ViewBag.ShowDeleted = showDeleted;
            return View(products);
        }

        // GET: AdminProduct/Create - SADECE ADMIN
        public IActionResult Create()
        {
            if (!AuthHelper.IsAdmin(HttpContext))
            {
                TempData["Error"] = "Bu iþlem için yönetici yetkisi gereklidir!";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Categories = new SelectList(_context.Categories.Where(c => !c.IsDeleted), "Id", "Name");
            return View();
        }

        // POST: AdminProduct/Create - SADECE ADMIN
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile? imageFile)
        {
            if (!AuthHelper.IsAdmin(HttpContext))
            {
                TempData["Error"] = "Bu iþlem için yönetici yetkisi gereklidir!";
                return RedirectToAction("Index", "Home");
            }

            if (ModelState.IsValid)
            {
                // Resim yükleme
                if (imageFile != null && imageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");

                    // Klasör yoksa oluþtur
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    product.ImageUrl = "/images/products/" + uniqueFileName;
                }
                else
                {
                    product.ImageUrl = "/images/products/no-image.jpg"; // Varsayýlan resim
                }

                product.CreatedAt = DateTime.Now;
                _context.Add(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Ürün baþarýyla eklendi!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new SelectList(_context.Categories.Where(c => !c.IsDeleted), "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: AdminProduct/Edit/5 - SADECE ADMIN
        public async Task<IActionResult> Edit(int? id)
        {
            if (!AuthHelper.IsAdmin(HttpContext))
            {
                TempData["Error"] = "Bu iþlem için yönetici yetkisi gereklidir!";
                return RedirectToAction("Index", "Home");
            }

            if (id == null)
                return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null || product.IsDeleted)
                return NotFound();

            ViewBag.Categories = new SelectList(_context.Categories.Where(c => !c.IsDeleted), "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: AdminProduct/Edit/5 - SADECE ADMIN
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile? imageFile)
        {
            if (!AuthHelper.IsAdmin(HttpContext))
            {
                TempData["Error"] = "Bu iþlem için yönetici yetkisi gereklidir!";
                return RedirectToAction("Index", "Home");
            }

            if (id != product.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Yeni resim yüklendiyse
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        // Eski resmi sil (varsayýlan resim deðilse)
                        if (!string.IsNullOrEmpty(product.ImageUrl) && product.ImageUrl != "/images/products/no-image.jpg")
                        {
                            string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Yeni resmi yükle
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "products");

                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }

                        product.ImageUrl = "/images/products/" + uniqueFileName;
                    }

                    product.UpdatedAt = DateTime.Now;
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Ürün baþarýyla güncellendi!";
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

            ViewBag.Categories = new SelectList(_context.Categories.Where(c => !c.IsDeleted), "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: AdminProduct/Delete/5 - SADECE ADMIN
        public async Task<IActionResult> Delete(int? id)
        {
            if (!AuthHelper.IsAdmin(HttpContext))
            {
                TempData["Error"] = "Bu iþlem için yönetici yetkisi gereklidir!";
                return RedirectToAction("Index", "Home");
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

        // POST: AdminProduct/Delete/5 - SOFT DELETE
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!AuthHelper.IsAdmin(HttpContext))
            {
                TempData["Error"] = "Bu iþlem için yönetici yetkisi gereklidir!";
                return RedirectToAction("Index", "Home");
            }

            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                product.IsDeleted = true;
                product.UpdatedAt = DateTime.Now;
                _context.Update(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Ürün baþarýyla silindi!";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: AdminProduct/PermanentDelete/5
        public async Task<IActionResult> PermanentDelete(int? id)
        {
            if (!AuthHelper.IsAdmin(HttpContext))
            {
                TempData["Error"] = "Bu iþlem için yönetici yetkisi gereklidir!";
                return RedirectToAction("Index", "Home");
            }

            if (id == null)
                return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        // POST: AdminProduct/PermanentDelete/5 - HARD DELETE
        [HttpPost, ActionName("PermanentDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PermanentDeleteConfirmed(int id)
        {
            if (!AuthHelper.IsAdmin(HttpContext))
            {
                TempData["Error"] = "Bu iþlem için yönetici yetkisi gereklidir!";
                return RedirectToAction("Index", "Home");
            }

            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                // Resmi sil (varsayýlan resim deðilse)
                if (!string.IsNullOrEmpty(product.ImageUrl) && product.ImageUrl != "/images/products/no-image.jpg")
                {
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Ürün kalýcý olarak silindi!";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: AdminProduct/Restore/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(int id)
        {
            if (!AuthHelper.IsAdmin(HttpContext))
            {
                TempData["Error"] = "Bu iþlem için yönetici yetkisi gereklidir!";
                return RedirectToAction("Index", "Home");
            }

            var product = await _context.Products.FindAsync(id);

            if (product != null && product.IsDeleted)
            {
                product.IsDeleted = false;
                product.UpdatedAt = DateTime.Now;
                _context.Update(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Ürün geri getirildi!";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id && !e.IsDeleted);
        }
    }
}