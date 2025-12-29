using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerce.Data;

namespace ECommerce.Web.Components
{
    public class CategoryNavViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public CategoryNavViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Veritabanýndan aktif kategorileri al (maksimum 5 tane)
            var categories = await _context.Categories
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.Name)
                .Take(5)
                .ToListAsync();

            return View(categories);
        }
    }
}