using System.ComponentModel.DataAnnotations;

namespace Service.Domain.ViewModels.LoginAndRegistration
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email или Логин обязателен")]
        [Display(Name = "Email или Логин")]
        public string Login { get; set; } // Убран атрибут [EmailAddress], чтобы работал вход по логину

        [Required(ErrorMessage = "Пароль обязателен")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
    }
}