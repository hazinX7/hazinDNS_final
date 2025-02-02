using System.ComponentModel.DataAnnotations;

public class CardBalanceModel
{
    [Required(ErrorMessage = "Номер карты обязателен")]
    [RegularExpression(@"^\d{16}$", ErrorMessage = "Неверный формат номера карты")]
    public string CardNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Срок действия карты обязателен")]
    [RegularExpression(@"^(0[1-9]|1[0-2])\/([0-9]{2})$", ErrorMessage = "Неверный формат срока действия")]
    public string ExpiryDate { get; set; } = string.Empty;

    [Required(ErrorMessage = "CVV код обязателен")]
    [RegularExpression(@"^\d{3}$", ErrorMessage = "Неверный формат CVV")]
    public string CVV { get; set; } = string.Empty;

    [Required(ErrorMessage = "Сумма пополнения обязательна")]
    [Range(1, 750000, ErrorMessage = "Сумма должна быть от 1 до 750 000 рублей")]
    public decimal Amount { get; set; }
} 