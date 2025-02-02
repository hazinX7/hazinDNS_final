using System.ComponentModel.DataAnnotations;

namespace hazinDNS_v2.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Новый";
        [Required]
        [StringLength(500)]
        public string DeliveryAddress { get; set; } = string.Empty;
        public string DeliveryCity { get; set; } = string.Empty;
        public string? Comment { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        [Required]
        public decimal Price { get; set; }
    }
} 