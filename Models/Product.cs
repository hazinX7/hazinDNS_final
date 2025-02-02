using System.ComponentModel.DataAnnotations;

namespace hazinDNS_v2.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public bool InStock { get; set; }
    }
} 