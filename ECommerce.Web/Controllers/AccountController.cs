using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerce.Data;
using ECommerce.Models;
using System.Security.Cryptography;
using System.Text;

namespace ECommerce.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user, string confirmPassword)
        {
            if (user.Password != confirmPassword)
            {
                ModelState.AddModelError("confirmPassword", "Şifreler eşleşmiyor!");
                return View(user);
            }

            // Email kontrolü
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == user.Email);

            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Bu email adresi zaten kullanılıyor!");
                return View(user);
            }

            if (ModelState.IsValid)
            {
                // Şifreyi hashle
                user.Password = HashPassword(user.Password);
                user.CreatedAt = DateTime.Now;
                user.IsActive = true;

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Kayıt başarılı! Giriş yapabilirsiniz.";
                return RedirectToAction("Login");
            }

            return View(user);
        }

        // GET: Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            var hashedPassword = HashPassword(password);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == hashedPassword && u.IsActive);

            if (user != null)
            {
                // Session'a kullanıcı bilgilerini kaydet
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserName", user.FullName);
                HttpContext.Session.SetString("UserEmail", user.Email);

                if (user.IsAdmin)
                {
                    HttpContext.Session.SetString("IsAdmin", "true");
                }

                // Son giriş tarihini güncelle
                user.LastLoginDate = DateTime.Now;
                await _context.SaveChangesAsync();

                TempData["Success"] = $"Hoş geldiniz, {user.FullName}!";
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Email veya şifre hatalı!");
            return View();
        }

        // GET: Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "Çıkış yapıldı.";
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/Profile
        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            return View(user);
        }

        // Şifre hashleme
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}