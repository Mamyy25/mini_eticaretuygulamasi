using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerce.Data;
using ECommerce.Models;

namespace ECommerce.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Sepeti Görüntüle
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                TempData["Error"] = "Sepeti görüntülemek için giriş yapmalısınız!";
                return RedirectToAction("Login", "Account");
            }

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                        .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(c => c.UserId == userId.Value);

            if (cart == null || !cart.CartItems.Any())
            {
                return View(new Cart { CartItems = new List<CartItem>() });
            }

            return View(cart);
        }

        // Sepete Ürün Ekle
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                TempData["Error"] = "Sepete ürün eklemek için giriş yapmalısınız!";
                return RedirectToAction("Login", "Account");
            }

            // Ürün kontrolü
            var product = await _context.Products.FindAsync(productId);
            if (product == null || product.IsDeleted || !product.IsActive)
            {
                TempData["Error"] = "Ürün bulunamadı!";
                return RedirectToAction("Index", "Product");
            }

            // Stok kontrolü
            if (product.Stock < quantity)
            {
                TempData["Error"] = "Yetersiz stok!";
                return RedirectToAction("Details", "Product", new { id = productId });
            }

            // Sepeti bul veya oluştur
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId.Value);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId.Value,
                    CreatedAt = DateTime.Now
                };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            // Ürün sepette var mı?
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);

            if (cartItem != null)
            {
                // Var ise miktarı artır
                cartItem.Quantity += quantity;

                // Stok kontrolü
                if (cartItem.Quantity > product.Stock)
                {
                    TempData["Error"] = "Yetersiz stok! Maksimum " + product.Stock + " adet ekleyebilirsiniz.";
                    return RedirectToAction("Details", "Product", new { id = productId });
                }
            }
            else
            {
                // Yok ise yeni ekle
                cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = quantity,
                    AddedAt = DateTime.Now
                };
                _context.CartItems.Add(cartItem);
            }

            cart.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Ürün sepete eklendi!";
            return RedirectToAction("Index");
        }

        // Sepet Miktarını Güncelle
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return Json(new { success = false, message = "Giriş yapmalısınız!" });
            }

            var cartItem = await _context.CartItems
                .Include(ci => ci.Cart)
                .Include(ci => ci.Product)
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.Cart.UserId == userId.Value);

            if (cartItem == null)
            {
                return Json(new { success = false, message = "Ürün bulunamadı!" });
            }

            if (quantity < 1)
            {
                return Json(new { success = false, message = "Miktar en az 1 olmalıdır!" });
            }

            if (quantity > cartItem.Product.Stock)
            {
                return Json(new { success = false, message = "Yetersiz stok! Maksimum " + cartItem.Product.Stock + " adet." });
            }

            cartItem.Quantity = quantity;
            cartItem.Cart.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            var subTotal = cartItem.Quantity * cartItem.Product.Price;
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId.Value);

            return Json(new
            {
                success = true,
                subTotal = subTotal.ToString("N2"),
                totalAmount = cart.TotalAmount.ToString("N2"),
                totalItems = cart.TotalItems
            });
        }

        // Sepetten Ürün Sil
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                TempData["Error"] = "Giriş yapmalısınız!";
                return RedirectToAction("Login", "Account");
            }

            var cartItem = await _context.CartItems
                .Include(ci => ci.Cart)
                .FirstOrDefaultAsync(ci => ci.Id == cartItemId && ci.Cart.UserId == userId.Value);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                cartItem.Cart.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Ürün sepetten çıkarıldı!";
            }

            return RedirectToAction("Index");
        }

        // Sepeti Temizle
        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                TempData["Error"] = "Giriş yapmalısınız!";
                return RedirectToAction("Login", "Account");
            }

            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId.Value);

            if (cart != null && cart.CartItems.Any())
            {
                _context.CartItems.RemoveRange(cart.CartItems);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Sepet temizlendi!";
            }

            return RedirectToAction("Index");
        }
    }
}