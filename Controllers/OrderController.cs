using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using hazinDNS_v2.Models;
using hazinDNS_v2.Data;
using System.Security.Claims;

namespace hazinDNS_v2.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrderController> _logger;

        public OrderController(ApplicationDbContext context, ILogger<OrderController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var cartItems = await _context.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.CartId == $"user_{userId}")
                .ToListAsync();

            if (!cartItems.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            var totalAmount = cartItems.Sum(ci => ci.Product.Price * ci.Quantity);
            var user = await _context.Users.FindAsync(int.Parse(userId));

            ViewBag.TotalAmount = totalAmount;
            ViewBag.UserBalance = user?.Balance ?? 0;

            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderModel model)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null) return Unauthorized(new { success = false, message = "Пользователь не авторизован" });

                var user = await _context.Users.FindAsync(int.Parse(userId));
                if (user == null) return NotFound(new { success = false, message = "Пользователь не найден" });

                // Получаем товары из корзины
                var cartItems = await _context.CartItems
                    .Include(ci => ci.Product)
                    .Where(ci => ci.CartId == $"user_{userId}")
                    .ToListAsync();

                if (!cartItems.Any())
                {
                    return BadRequest(new { success = false, message = "Корзина пуста" });
                }

                decimal totalAmount = cartItems.Sum(ci => ci.Product.Price * ci.Quantity);

                // Проверяем баланс пользователя
                if (user.Balance < totalAmount)
                {
                    return BadRequest(new { 
                        success = false, 
                        message = $"Недостаточно средств. Необходимо: {totalAmount:N0} ₽, на балансе: {user.Balance:N0} ₽" 
                    });
                }

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Создаем заказ
                    var order = new Order
                    {
                        UserId = int.Parse(userId),
                        OrderDate = DateTime.UtcNow,
                        Status = "Создан",
                        TotalAmount = totalAmount,
                        DeliveryAddress = model.DeliveryAddress,
                        DeliveryCity = model.DeliveryCity,
                        Comment = model.Comment
                    };

                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();

                    // Создаем элементы заказа
                    foreach (var cartItem in cartItems)
                    {
                        var orderItem = new OrderItem
                        {
                            OrderId = order.Id,
                            ProductId = cartItem.ProductId,
                            Quantity = cartItem.Quantity,
                            Price = cartItem.Product.Price
                        };
                        _context.OrderItems.Add(orderItem);
                    }

                    // Списываем средства с баланса
                    user.Balance -= totalAmount;

                    // Очищаем корзину
                    _context.CartItems.RemoveRange(cartItems);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return StatusCode(StatusCodes.Status201Created, new { 
                        success = true, 
                        message = "Заказ успешно оформлен",
                        orderId = order.Id,
                        orderDetails = new {
                            deliveryAddress = order.DeliveryAddress,
                            deliveryCity = order.DeliveryCity,
                            totalAmount = order.TotalAmount,
                            items = cartItems.Select(ci => new {
                                productName = ci.Product.Name,
                                quantity = ci.Quantity,
                                price = ci.Product.Price,
                                total = ci.Quantity * ci.Product.Price
                            })
                        }
                    });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Ошибка при создании заказа");
                    return StatusCode(500, new { success = false, message = "Ошибка при создании заказа" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Неожиданная ошибка при оформлении заказа");
                return StatusCode(500, new { success = false, message = "Произошла ошибка при оформлении заказа" });
            }
        }

        [Authorize]
        public async Task<IActionResult> History()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == int.Parse(userId))
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }
    }

    public class PlaceOrderModel
    {
        public string DeliveryAddress { get; set; }
        public string DeliveryCity { get; set; }
        public string? Comment { get; set; }
    }
} 