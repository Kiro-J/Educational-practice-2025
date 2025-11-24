using System.ComponentModel.DataAnnotations;

namespace Service.Domain.ViewModels.LoginAndRegistration
{
    public class ConfirmEmailViewModel
    {
        [Required(ErrorMessage = "Введите код")]
        public string Code { get; set; }

        public string GeneratedCode { get; set; }

        // Данные пользователя, которые мы прокидываем дальше для сохранения после подтверждения
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }
    }
}