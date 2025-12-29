using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerce.Data;

namespace ECommerce.Web.Components
{
    public class CartCountViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public CartCountViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Session'dan kullanıcı ID'si al
            var userId = HttpContext.Session.GetInt32("UserId");
            
            if (userId == null)
            {
                // Kullanıcı giriş yapmamışsa 0 döndür
                return View(0);
            }

            // Kullanıcının sepetindeki toplam ürün sayısı
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId.Value);

            var itemCount = cart?.CartItems?.Sum(ci => ci.Quantity) ?? 0;

            return View(itemCount);
        }
    }
}