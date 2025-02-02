using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using hazinDNS_v2.Models;
using Microsoft.AspNetCore.Authorization;
using hazinDNS_v2.Data;
using Microsoft.EntityFrameworkCore;

namespace hazinDNS_v2.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return RedirectToAction(nameof(Index));
            }

            var products = await _context.Products
                .Where(p => p.Name.Contains(query) || 
                           p.Description.Contains(query) ||
                           p.Category.Contains(query))
                .ToListAsync();

            ViewBag.SearchTerm = query;
            return View("Index", products);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.ToListAsync();
            return View(products);
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult UserGuide()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
