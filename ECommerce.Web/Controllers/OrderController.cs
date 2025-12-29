using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerce.Data;
using ECommerce.Models;

namespace ECommerce.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Adım 1: Teslimat Bilgileri
        public async Task<IActionResult> Checkout()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                TempData["Error"] = "Sipariş vermek için giriş yapmalısınız!";
                return RedirectToAction("Login", "Account");
            }

            // Sepeti kontrol et
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId.Value);

            if (cart == null || !cart.CartItems.Any())
            {
                TempData["Error"] = "Sepetiniz boş!";
                return RedirectToAction("Index", "Cart");
            }

            // Kullanıcı bilgilerini getir
            var user = await _context.Users.FindAsync(userId.Value);

            // ViewModel oluştur
            var model = new CheckoutViewModel
            {
                Cart = cart,
                User = user,
                Order = new Order
                {
                    ShippingAddress = user?.Address ?? "",
                    ShippingCity = user?.City ?? "",
                    ShippingZipCode = user?.ZipCode ?? "",
                    ShippingPhone = user?.Phone ?? ""
                }
            };

            return View(model);
        }

        // Adım 2: Sipariş Önizleme
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Review(Order order)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Sepeti getir
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId.Value);

            if (cart == null || !cart.CartItems.Any())
            {
                TempData["Error"] = "Sepetiniz boş!";
                return RedirectToAction("Index", "Cart");
            }

            // Teslimat bilgilerini Session'a kaydet
            HttpContext.Session.SetString("ShippingAddress", order.ShippingAddress);
            HttpContext.Session.SetString("ShippingCity", order.ShippingCity ?? "");
            HttpContext.Session.SetString("ShippingZipCode", order.ShippingZipCode ?? "");
            HttpContext.Session.SetString("ShippingPhone", order.ShippingPhone ?? "");
            HttpContext.Session.SetString("Notes", order.Notes ?? "");

            var model = new OrderReviewViewModel
            {
                Cart = cart,
                ShippingAddress = order.ShippingAddress,
                ShippingCity = order.ShippingCity,
                ShippingZipCode = order.ShippingZipCode,
                ShippingPhone = order.ShippingPhone,
                Notes = order.Notes
            };

            return View(model);
        }

        // Adım 3: Sipariş Onaylama
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Sepeti getir
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                    .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId.Value);

            if (cart == null || !cart.CartItems.Any())
            {
                TempData["Error"] = "Sepetiniz boş!";
                return RedirectToAction("Index", "Cart");
            }

            // Stok kontrolü
            foreach (var item in cart.CartItems)
            {
                if (item.Product.Stock < item.Quantity)
                {
                    TempData["Error"] = $"{item.Product.Name} için yetersiz stok!";
                    return RedirectToAction("Index", "Cart");
                }
            }

            // Order oluştur
            var order = new Order
            {
                UserId = userId.Value,
                OrderDate = DateTime.Now,
                TotalAmount = cart.TotalAmount,
                Status = "Pending",
                ShippingAddress = HttpContext.Session.GetString("ShippingAddress") ?? "",
                ShippingCity = HttpContext.Session.GetString("ShippingCity"),
                ShippingZipCode = HttpContext.Session.GetString("ShippingZipCode"),
                ShippingPhone = HttpContext.Session.GetString("ShippingPhone"),
                Notes = HttpContext.Session.GetString("Notes")
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // OrderItems oluştur ve stok düş
            foreach (var cartItem in cart.CartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.Product.Price,
                    ProductName = cartItem.Product.Name
                };

                _context.OrderItems.Add(orderItem);

                // Stoktan düş
                cartItem.Product.Stock -= cartItem.Quantity;
            }

            // Sepeti temizle
            _context.CartItems.RemoveRange(cart.CartItems);

            await _context.SaveChangesAsync();

            // Session'ı temizle
            HttpContext.Session.Remove("ShippingAddress");
            HttpContext.Session.Remove("ShippingCity");
            HttpContext.Session.Remove("ShippingZipCode");
            HttpContext.Session.Remove("ShippingPhone");
            HttpContext.Session.Remove("Notes");

            TempData["Success"] = "Siparişiniz başarıyla oluşturuldu!";
            return RedirectToAction("Confirmation", new { id = order.Id });
        }

        // Sipariş Onay Sayfası
        public async Task<IActionResult> Confirmation(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId.Value);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // Sipariş Geçmişi
        public async Task<IActionResult> MyOrders()
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                TempData["Error"] = "Giriş yapmalısınız!";
                return RedirectToAction("Login", "Account");
            }

            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId.Value)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        // Sipariş Detayı
        public async Task<IActionResult> Details(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId.Value);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}