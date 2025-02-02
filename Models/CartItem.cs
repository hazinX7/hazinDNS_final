namespace hazinDNS_v2.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public string? CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public Product Product { get; set; } = null!;
    }
} 