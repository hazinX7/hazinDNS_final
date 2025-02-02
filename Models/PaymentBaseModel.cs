using System.ComponentModel.DataAnnotations;

public abstract class PaymentBaseModel
{
    [Required(ErrorMessage = "Сумма пополнения обязательна")]
    [Range(1, 750000, ErrorMessage = "Сумма должна быть от 1 до 750 000 рублей")]
    public virtual decimal Amount { get; set; }

    [Required]
    public string PaymentType { get; set; } = string.Empty;
} 