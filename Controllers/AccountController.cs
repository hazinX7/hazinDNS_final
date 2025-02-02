using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using hazinDNS_v2.Data;
using hazinDNS_v2.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace hazinDNS_v2.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ApplicationDbContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
        }
        

        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Получаем username вместо userId
            var username = User.Identity?.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                return NotFound();
            }

            // Проверяем текущий пароль
            if (user.Password != model.CurrentPassword)
            {
                ModelState.AddModelError("CurrentPassword", "Неверный текущий пароль");
                return View(model);
            }

            // Обновляем пароль
            user.Password = model.NewPassword;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Пароль успешно изменен";
            return RedirectToAction("Index", "Profile");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Проверяем, существует ли пользователь с таким email
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (existingUser != null)
            {
                return BadRequest(new { message = "Пользователь с таким email уже существует" });
            }

            // Создаем нового пользователя
            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                Password = model.Password // В реальном приложении пароль должен быть захэширован
            };

            try 
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("Пользователь создан успешно.");
                return StatusCode(201, new { success = true, message = "Регистрация успешна" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при регистрации пользователя");
                return StatusCode(500, new { message = "Ошибка при регистрации пользователя" });
            }
        }
    }
} 