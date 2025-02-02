using System.ComponentModel.DataAnnotations;

namespace hazinDNS_v2.Models
{
    public class PlaceOrderModel
    {
        [Required(ErrorMessage = "Адрес доставки обязателен")]
        public string DeliveryAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "Город доставки обязателен")]
        public string DeliveryCity { get; set; } = string.Empty;

        public string? Comment { get; set; }
    }
} 