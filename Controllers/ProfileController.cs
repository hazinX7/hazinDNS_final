using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using hazinDNS_v2.Data;
using Microsoft.EntityFrameworkCore;

namespace hazinDNS_v2.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("Login", "Home");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id.ToString() == userId);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> ResetBalance()
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == "admin");
            if (user != null)
            {
                user.Balance = 0;
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
    }
} 