using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using hazinDNS_v2.Models;
using hazinDNS_v2.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;

namespace hazinDNS_v2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuthController> _logger;
        private readonly CartController _cartController;

        public AuthController(IConfiguration configuration, ApplicationDbContext context, ILogger<AuthController> logger, CartController cartController)
        {
            _configuration = configuration;
            _context = context;
            _logger = logger;
            _cartController = cartController;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == model.Username);

                if (user == null || user.Password != model.Password)
                {
                    return Unauthorized("Неверное имя пользователя или пароль");
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var identity = new ClaimsIdentity(claims, "Cookies");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("Cookies", principal);

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при входе пользователя");
                return StatusCode(500, "Произошла ошибка при входе");
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            _logger.LogInformation($"Попытка регистрации пользователя: {model.Username}");

            try 
            {
                if (string.IsNullOrWhiteSpace(model.Password))
                {
                    return BadRequest(new { message = "Пароль не может быть пустым" });
                }

                if (model.Password.Length < 6)
                {
                    return BadRequest(new { message = "Пароль должен содержать минимум 6 символов" });
                }

                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == model.Username || u.Email == model.Email);

                if (existingUser != null)
                {
                    _logger.LogWarning($"Пользователь уже существует: {model.Username}");
                    return Conflict(new { 
                        status = 409,
                        message = "Пользователь с таким именем или email уже существует" 
                    });
                }

                var user = new User
                {
                    Username = model.Username,
                    Email = model.Email,
                    Password = model.Password,
                    Role = "User"
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Пользователь успешно зарегистрирован: {model.Username}");
                return StatusCode(StatusCodes.Status201Created, new { message = "Регистрация успешна" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при регистрации пользователя");
                return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            HttpContext.Session.Clear();
            return Ok(new { success = true });
        }

        [HttpGet("checkAuth")]
        public IActionResult CheckAuth()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return Ok();
            }
            return Unauthorized();
        }

        private string GenerateJwtToken(User user)
        {
            var jwtKey = _configuration["Jwt:Key"] ?? 
                throw new InvalidOperationException("JWT key is not configured");
            var jwtIssuer = _configuration["Jwt:Issuer"] ?? 
                throw new InvalidOperationException("JWT issuer is not configured");
            var jwtAudience = _configuration["Jwt:Audience"] ?? 
                throw new InvalidOperationException("JWT audience is not configured");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("UserId", user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginModel
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterModel
    {
        [Required(ErrorMessage = "Имя пользователя обязательно")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обязателен")]
        [MinLength(6, ErrorMessage = "Пароль должен содержать минимум 6 символов")]
        public string Password { get; set; } = string.Empty;
    }
} 