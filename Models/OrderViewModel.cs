namespace hazinDNS_v2.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public string UserEmail { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public List<OrderItemViewModel> Items { get; set; }
    }

    public class OrderItemViewModel
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
} 