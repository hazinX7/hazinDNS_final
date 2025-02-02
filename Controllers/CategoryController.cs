using Microsoft.AspNetCore.Mvc;
using hazinDNS_v2.Data;
using Microsoft.EntityFrameworkCore;

namespace hazinDNS_v2.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Products
                .Select(p => p.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            return View(categories);
        }

        public async Task<IActionResult> Products(string category)
        {
            var products = await _context.Products
                .Where(p => p.Category == category)
                .ToListAsync();

            ViewBag.CategoryName = category;
            return View(products);
        }
    }
} 