using Microsoft.EntityFrameworkCore;
using hazinDNS_v2.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using hazinDNS_v2.Controllers;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Localization;

namespace hazinDNS_v2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie = new CookieBuilder
                {
                    Name = ".hazinDNS.Session",
                    HttpOnly = true,
                    IsEssential = true,
                    SameSite = SameSiteMode.Lax
                };
            });

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("ru-RU");
                options.SupportedCultures = new List<CultureInfo> { new CultureInfo("ru-RU") };
                options.SupportedUICultures = new List<CultureInfo> { new CultureInfo("ru-RU") };
            });

            var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is not configured");
            var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT issuer is not configured");
            var jwtAudience = builder.Configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT audience is not configured");

            // Add services to the container.
            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                options.Filters.Add(new IgnoreAntiforgeryTokenAttribute());
            });
            builder.Services.AddScoped<CartController>();
            builder.Services.AddScoped<WishlistController>();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddAuthentication("Cookies")
                .AddCookie("Cookies", options =>
                {
                    options.Cookie.Name = ".hazinDNS.Auth";
                    options.LoginPath = "/Home/Login";
                    options.LogoutPath = "/Home/Logout";
                    options.ExpireTimeSpan = TimeSpan.FromDays(1);
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRequestLocalization();

            app.MapControllerRoute(
                name: "profile",
                pattern: "Profile",
                defaults: new { controller = "Account", action = "Profile" });

            app.MapControllerRoute(
                name: "wishlist",
                pattern: "Wishlist",
                defaults: new { controller = "Wishlist", action = "Index" });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "admin",
                pattern: "admin/{action=Index}/{id?}",
                defaults: new { controller = "Admin" });

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try 
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    // Убедимся, что база данных создана и применены все миграции
                    context.Database.EnsureCreated();
                    
                    // Теперь можно обновлять продукты
                    DbInitializer.UpdateProducts(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Произошла ошибка при инициализации базы данных.");
                    throw;
                }
            }

            app.Run();
        }
    }
}
