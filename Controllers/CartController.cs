using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using hazinDNS_v2.Models;
using hazinDNS_v2.Data;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace hazinDNS_v2.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class CartController : Controller
    {
        public class AddToCartModel
        {
            public int ProductId { get; set; }
        }

        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CartController> _logger;

        public CartController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, ILogger<CartController> logger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized(new { message = "Необходима авторизация" });
            }

            var cartItems = new List<CartItem>();
            var cartId = $"user_{userId}";
            cartItems = await _context.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.CartId == cartId)
                .ToListAsync();

            return View(cartItems);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("AddToCart")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartModel model)
        {
            try
            {
                _logger.LogInformation($"Received request body: {JsonSerializer.Serialize(model)}");
                _logger.LogInformation($"Adding product to cart. ProductId: {model.ProductId}");
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation($"UserId: {userId}");

                if (userId == null)
                {
                    return Unauthorized(new { success = false, message = "Необходима авторизация" });
                }

                var product = await _context.Products.FindAsync(model.ProductId);
                
                if (product == null)
                {
                    _logger.LogWarning($"Product not found: {model.ProductId}");
                    return Json(new { success = false, message = "Продукт не найден" });
                }

                string cartId = $"user_{userId}";

                var cartItem = await _context.CartItems
                    .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == model.ProductId);

                if (cartItem == null)
                {
                    cartItem = new CartItem
                    {
                        CartId = cartId,
                        ProductId = model.ProductId,
                        Quantity = 1
                    };
                    _context.CartItems.Add(cartItem);
                    _logger.LogInformation("Created new cart item");
                }
                else
                {
                    cartItem.Quantity++;
                    _logger.LogInformation($"Updated quantity for existing cart item: {cartItem.Quantity}");
                }
                await _context.SaveChangesAsync();

                return StatusCode(StatusCodes.Status201Created, new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding product to cart");
                return StatusCode(500, new { success = false, message = "Произошла ошибка при добавлении товара в корзину" });
            }
        }

        [AllowAnonymous]
        [HttpGet("GetCartCount")]
        public async Task<IActionResult> GetCartCount()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Json(new { count = 0 });
            }

            string cartId = $"user_{userId}";
            int count = await _context.CartItems
                .Where(ci => ci.CartId == cartId)
                .SumAsync(ci => ci.Quantity);

            return Json(new { count });
        }

        [AllowAnonymous]
        public async Task MergeCartAsync(string userId)
        {
            try 
            {
                _logger.LogInformation($"Starting cart merge for user {userId}");
                var sessionCartId = $"session_{HttpContext.Session.Id}";
                if (string.IsNullOrEmpty(sessionCartId))
                {
                    _logger.LogInformation("No session cart found");
                    return;
                }

                var userCartId = $"user_{userId}";
                await ClearCartItemsAsync(userCartId);

                var sessionCartItems = await _context.CartItems
                    .Where(ci => ci.CartId == sessionCartId)
                    .ToListAsync();

                foreach (var sessionItem in sessionCartItems)
                {
                    var newCartItem = new CartItem
                    {
                        CartId = userCartId,
                        ProductId = sessionItem.ProductId,
                        Quantity = sessionItem.Quantity
                    };
                    _context.CartItems.Add(newCartItem);
                    _logger.LogInformation($"Added new cart item for product {sessionItem.ProductId}");
                }

                await _context.SaveChangesAsync();
                await ClearCartItemsAsync(sessionCartId);
                HttpContext.Session.Clear();
                _logger.LogInformation("Cart merge completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during cart merge");
                throw;
            }
        }

        [HttpPost]
        [Route("RemoveFromCart")]
        public async Task<IActionResult> RemoveFromCart([FromBody] AddToCartModel model)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Unauthorized(new { success = false, message = "Необходима авторизация" });
                }

                string cartId = $"user_{userId}";
                var cartItem = await _context.CartItems
                    .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.ProductId == model.ProductId);

                if (cartItem != null)
                {
                    _context.CartItems.Remove(cartItem);
                    await _context.SaveChangesAsync();
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing item from cart");
                return StatusCode(500, new { success = false, message = "Произошла ошибка при удалении товара из корзины" });
            }
        }

        [HttpPost]
        [Route("ClearCart")]
        public async Task<IActionResult> ClearCart()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Unauthorized(new { success = false, message = "Необходима авторизация" });
                }

                string cartId = $"user_{userId}";
                var cartItems = await _context.CartItems
                    .Where(ci => ci.CartId == cartId)
                    .ToListAsync();

                _context.CartItems.RemoveRange(cartItems);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cart");
                return StatusCode(500, new { success = false, message = "Произошла ошибка при очистке корзины" });
            }
        }

        public async Task ClearCartItemsAsync(string cartId)
        {
            var items = await _context.CartItems
                .Where(ci => ci.CartId == cartId)
                .ToListAsync();
            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }

        [HttpPost("UpdateQuantity")]
        public async Task<IActionResult> UpdateQuantity([FromBody] UpdateQuantityModel model)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (userId != null)
            {
                var cartItem = await _context.CartItems
                    .FirstOrDefaultAsync(ci => ci.CartId == $"user_{userId}" && ci.ProductId == model.ProductId);

                if (cartItem != null)
                {
                    var newQuantity = cartItem.Quantity + model.Delta;
                    
                    if (newQuantity <= 0)
                    {
                        _context.CartItems.Remove(cartItem);
                    }
                    else
                    {
                        cartItem.Quantity = newQuantity;
                    }
                    await _context.SaveChangesAsync();
                }
            }

            return Json(new { success = true });
        }

        public class UpdateQuantityModel
        {
            public int ProductId { get; set; }
            public int Delta { get; set; }
        }
    }
} 