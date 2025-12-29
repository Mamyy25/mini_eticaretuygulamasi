using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerce.Data;
using ECommerce.Models;
using ECommerce.Web.Helpers;

namespace ECommerce.Web.Controllers
{
	public class CategoryController : Controller
	{
		private readonly ApplicationDbContext _context;

		public CategoryController(ApplicationDbContext context)
		{
			_context = context;
		}

		// GET: Category/Index - SADECE ADMIN
		public async Task<IActionResult> Index(bool showDeleted = false)
		{
			if (!AuthHelper.IsAdmin(HttpContext))
			{
				TempData["Error"] = "Bu iþlem için yönetici yetkisi gereklidir!";
				return RedirectToAction("Index", "Home");
			}

			var categories = await _context.Categories
				.Where(c => showDeleted ? c.IsDeleted : !c.IsDeleted)
				.OrderBy(c => c.Name)
				.ToListAsync();

			ViewBag.ShowDeleted = showDeleted;
			return View(categories);
		}

		// GET: Category/Create - SADECE ADMIN
		public IActionResult Create()
		{
			if (!AuthHelper.IsAdmin(HttpContext))
			{
				TempData["Error"] = "Bu iþlem için yönetici yetkisi gereklidir!";
				return RedirectToAction("Index", "Home");
			}

			return View();
		}

		// POST: Category/Create - SADECE ADMIN
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(Category category)
		{
			if (!AuthHelper.IsAdmin(HttpContext))
			{
				TempData["Error"] = "Bu iþlem için yönetici yetkisi gereklidir!";
				return RedirectToAction("Index", "Home");
			}

			if (ModelState.IsValid)
			{
				// Ayný isimde kategori var mý kontrol et
				var existingCategory = await _context.Categories
					.FirstOrDefaultAsync(c => c.Name == category.Name && !c.IsDeleted);

				if (existingCategory != null)
				{
					ModelState.AddModelError("Name", "Bu isimde bir kategori zaten mevcut!");
					return View(category);
				}

				category.CreatedAt = DateTime.Now;
				_context.Add(category);
				await _context.SaveChangesAsync();
				TempData["Success"] = "Kategori baþarýyla eklendi!";
				return RedirectToAction(nameof(Index));
			}

			return View(category);
		}

		// GET: Category/Edit/5 - SADECE ADMIN
		public async Task<IActionResult> Edit(int? id)
		{
			if (!AuthHelper.IsAdmin(HttpContext))
			{
				TempData["Error"] = "Bu iþlem için yönetici yetkisi gereklidir!";
				return RedirectToAction("Index", "Home");
			}

			if (id == null)
				return NotFound();

			var category = await _context.Categories.FindAsync(id);

			if (category == null || category.IsDeleted)
				return NotFound();

			return View(category);
		}

		// POST: Category/Edit/5 - SADECE ADMIN
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, Category category)
		{
			if (!AuthHelper.IsAdmin(HttpContext))
			{
				TempData["Error"] = "Bu iþlem için yönetici yetkisi gereklidir!";
				return RedirectToAction("Index", "Home");
			}

			if (id != category.Id)
				return NotFound();

			if (ModelState.IsValid)
			{
				// Ayný isimde baþka kategori var mý kontrol et
				var existingCategory = await _context.Categories
					.FirstOrDefaultAsync(c => c.Name == category.Name && c.Id != id && !c.IsDeleted);

				if (existingCategory != null)
				{
					ModelState.AddModelError("Name", "Bu isimde bir kategori zaten mevcut!");
					return View(category);
				}

				try
				{
					category.UpdatedAt = DateTime.Now;
					_context.Update(category);
					await _context.SaveChangesAsync();
					TempData["Success"] = "Kategori baþarýyla güncellendi!";
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!CategoryExists(category.Id))
						return NotFound();
					else
						throw;
				}
				return RedirectToAction(nameof(Index));
			}

			return View(category);
		}

		// GET: Category/Delete/5 - SADECE ADMIN
		public async Task<IActionResult> Delete(int? id)
		{
			if (!AuthHelper.IsAdmin(HttpContext))
			{
				TempData["Error"] = "Bu iþlem için yönetici yetkisi gereklidir!";
				return RedirectToAction("Index", "Home");
			}

			if (id == null)
				return NotFound();

			var category = await _context.Categories
				.Include(c => c.Products)
				.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

			if (category == null)
				return NotFound();

			// Kategoriye ait ürün sayýsýný göster
			ViewBag.ProductCount = category.Products.Count(p => !p.IsDeleted);

			return View(category);
		}

		// POST: Category/Delete/5 - SADECE ADMIN (SOFT DELETE)
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			if (!AuthHelper.IsAdmin(HttpContext))
			{
				TempData["Error"] = "Bu iþlem için yönetici yetkisi gereklidir!";
				return RedirectToAction("Index", "Home");
			}

			var category = await _context.Categories
				.Include(c => c.Products)
				.FirstOrDefaultAsync(c => c.Id == id);

			if (category != null)
			{
				// Kategoriye ait ürünleri kontrol et
				var activeProducts = category.Products.Count(p => !p.IsDeleted);

				if (activeProducts > 0)
				{
					TempData["Error"] = $"Bu kategoriye ait {activeProducts} adet ürün bulunmaktadýr. Önce ürünleri silmelisiniz!";
					return RedirectToAction(nameof(Index));
				}

				category.IsDeleted = true;
				category.UpdatedAt = DateTime.Now;
				_context.Update(category);
				await _context.SaveChangesAsync();
				TempData["Success"] = "Kategori baþarýyla silindi!";
			}

			return RedirectToAction(nameof(Index));
		}

		// GET: Category/PermanentDelete/5 - KALICI SÝLME ONAY SAYFASI
		public async Task<IActionResult> PermanentDelete(int? id)
		{
			if (!AuthHelper.IsAdmin(HttpContext))
			{
				TempData["Error"] = "Bu iþlem için yönetici yetkisi gereklidir!";
				return RedirectToAction("Index", "Home");
			}

			if (id == null)
				return NotFound();

			var category = await _context.Categories
				.Include(c => c.Products)
				.FirstOrDefaultAsync(c => c.Id == id);

			if (category == null)
				return NotFound();

			ViewBag.ProductCount = category.Products.Count;
			ViewBag.IsDeleted = category.IsDeleted;

			return View(category);
		}

		// POST: Category/PermanentDelete/5 - VERÝTABANINDAN KALICI SÝL
		[HttpPost, ActionName("PermanentDelete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> PermanentDeleteConfirmed(int id)
		{
			if (!AuthHelper.IsAdmin(HttpContext))
			{
				TempData["Error"] = "Bu iþlem için yönetici yetkisi gereklidir!";
				return RedirectToAction("Index", "Home");
			}

			var category = await _context.Categories
				.Include(c => c.Products)
				.FirstOrDefaultAsync(c => c.Id == id);

			if (category != null)
			{
				// Kategoriye ait ürünleri kontrol et
				var totalProducts = category.Products.Count;

				if (totalProducts > 0)
				{
					TempData["Error"] = $"Bu kategoriye ait {totalProducts} adet ürün bulunmaktadýr. Önce ürünleri kalýcý olarak silmelisiniz!";
					return RedirectToAction(nameof(Index));
				}

				// Veritabanýndan tamamen sil
				_context.Categories.Remove(category);
				await _context.SaveChangesAsync();
				TempData["Success"] = "Kategori kalýcý olarak silindi!";
			}

			return RedirectToAction(nameof(Index));
		}

		// POST: Category/Restore/5 - SÝLÝNEN KATEGORÝYÝ GERÝ GETÝR
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Restore(int id)
		{
			if (!AuthHelper.IsAdmin(HttpContext))
			{
				TempData["Error"] = "Bu iþlem için yönetici yetkisi gereklidir!";
				return RedirectToAction("Index", "Home");
			}

			var category = await _context.Categories.FindAsync(id);

			if (category != null && category.IsDeleted)
			{
				category.IsDeleted = false;
				category.UpdatedAt = DateTime.Now;
				_context.Update(category);
				await _context.SaveChangesAsync();
				TempData["Success"] = "Kategori geri getirildi!";
			}

			return RedirectToAction(nameof(Index));
		}

		private bool CategoryExists(int id)
		{
			return _context.Categories.Any(e => e.Id == id && !e.IsDeleted);
		}
	}
}