using System.ComponentModel.DataAnnotations;

public class RegisterModel
{
    [Required(ErrorMessage = "Email обязателен")]
    [EmailAddress(ErrorMessage = "Некорректный формат email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Имя пользователя обязательно")]
    public string Username { get; set; }

    [Required(ErrorMessage = "Пароль обязателен")]
    [MinLength(6, ErrorMessage = "Пароль должен содержать минимум 6 символов")]
    public string Password { get; set; }
} 