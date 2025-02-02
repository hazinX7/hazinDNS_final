using Microsoft.EntityFrameworkCore;
using hazinDNS_v2.Models;

namespace hazinDNS_v2.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Wishlist> Wishlist { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.HasOne(d => d.Product)
                    .WithMany()
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.CartId)
                    .IsRequired();
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.DeliveryAddress)
                    .IsRequired();

                entity.Property(e => e.TotalAmount)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.OrderDate)
                    .HasColumnType("datetime2");
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.Product)
                    .WithMany()
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(18,2)");

                entity.Property(e => e.Quantity)
                    .IsRequired();
            });
        }
    }
} 