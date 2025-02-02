using System.ComponentModel.DataAnnotations;

public class SBPBalanceModel
{
    [Required(ErrorMessage = "Номер телефона обязателен")]
    [RegularExpression(@"^\+7\d{10}$", ErrorMessage = "Неверный формат номера телефона")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Сумма пополнения обязательна")]
    [Range(1, 750000, ErrorMessage = "Сумма должна быть от 1 до 750 000 рублей")]
    public decimal Amount { get; set; }
} 