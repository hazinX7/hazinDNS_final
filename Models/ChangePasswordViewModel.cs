using System.ComponentModel.DataAnnotations;

namespace hazinDNS_v2.Models
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Введите текущий пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Текущий пароль")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Введите новый пароль")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль должен быть не менее 6 символов")]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Подтвердите новый пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение нового пароля")]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
    }
} 