using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerce.Data;
using ECommerce.Models;

namespace ECommerce.Web.Controllers.API
{
    /// <summary>
    /// Ürün iþlemleri için API endpoint'leri
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProductsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Tüm aktif ürünleri listeler
        /// </summary>
        /// <returns>Ürün listesi</returns>
        /// <response code="200">Ürünler baþarýyla getirildi</response>
        /// <response code="500">Sunucu hatasý</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            try
            {
                var products = await _context.Products
                    .Where(p => !p.IsDeleted && p.IsActive)
                    .Include(p => p.Category)
                    .ToListAsync();

                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ürünler getirilirken bir hata oluþtu", error = ex.Message });
            }
        }

        /// <summary>
        /// Belirli bir ürünü ID'sine göre getirir
        /// </summary>
        /// <param name="id">Ürün ID'si</param>
        /// <returns>Ürün detaylarý</returns>
        /// <response code="200">Ürün baþarýyla bulundu</response>
        /// <response code="404">Ürün bulunamadý</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);

            if (product == null)
            {
                return NotFound(new { message = "Ürün bulunamadý" });
            }

            return Ok(product);
        }

        /// <summary>
        /// Kategoriye göre ürünleri filtreler
        /// </summary>
        /// <param name="categoryId">Kategori ID'si</param>
        /// <returns>Filtrelenmiþ ürün listesi</returns>
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductsByCategory(int categoryId)
        {
            var products = await _context.Products
                .Where(p => p.CategoryId == categoryId && !p.IsDeleted && p.IsActive)
                .Include(p => p.Category)
                .ToListAsync();

            return Ok(products);
        }

        /// <summary>
        /// Ürün ara
        /// </summary>
        /// <param name="searchTerm">Arama terimi</param>
        /// <returns>Arama sonuçlarý</returns>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Product>>> SearchProducts([FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest(new { message = "Arama terimi boþ olamaz" });
            }

            var products = await _context.Products
                .Where(p => !p.IsDeleted && p.IsActive &&
                           (p.Name.Contains(searchTerm) || p.Description.Contains(searchTerm)))
                .Include(p => p.Category)
                .ToListAsync();

            return Ok(products);
        }

        /// <summary>
        /// Yeni ürün oluþtur (Admin)
        /// </summary>
        /// <param name="product">Ürün bilgileri</param>
        /// <returns>Oluþturulan ürün</returns>
        /// <response code="201">Ürün baþarýyla oluþturuldu</response>
        /// <response code="400">Geçersiz veri</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                product.CreatedAt = DateTime.Now;
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Ürün oluþturulurken hata oluþtu", error = ex.Message });
            }
        }

        /// <summary>
        /// Ürün güncelle (Admin)
        /// </summary>
        /// <param name="id">Ürün ID'si</param>
        /// <param name="product">Güncellenmiþ ürün bilgileri</param>
        /// <returns>Baþarý durumu</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
        {
            if (id != product.Id)
            {
                return BadRequest(new { message = "ID uyuþmazlýðý" });
            }

            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null || existingProduct.IsDeleted)
            {
                return NotFound(new { message = "Ürün bulunamadý" });
            }

            try
            {
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.Stock = product.Stock;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.IsActive = product.IsActive;
                existingProduct.ImageUrl = product.ImageUrl;
                existingProduct.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Güncelleme sýrasýnda hata oluþtu", error = ex.Message });
            }
        }

        /// <summary>
        /// Ürün sil (Soft delete)
        /// </summary>
        /// <param name="id">Silinecek ürün ID'si</param>
        /// <returns>Baþarý durumu</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "Ürün bulunamadý" });
            }

            product.IsDeleted = true;
            product.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Stok bilgisi getir
        /// </summary>
        /// <param name="id">Ürün ID'si</param>
        /// <returns>Stok bilgisi</returns>
        [HttpGet("{id}/stock")]
        public async Task<ActionResult> GetStock(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null || product.IsDeleted)
            {
                return NotFound(new { message = "Ürün bulunamadý" });
            }

            return Ok(new
            {
                productId = id,
                productName = product.Name,
                stock = product.Stock,
                isInStock = product.Stock > 0
            });
        }
    }
}