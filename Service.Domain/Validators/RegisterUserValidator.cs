using FluentValidation;
using Service.Domain.ViewModels.LoginAndRegistration;

namespace Service.Domain.Validators
{
    public class RegisterUserValidator : AbstractValidator<RegisterViewModel>
    {
        public RegisterUserValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Укажите имя")
                .Length(3, 20).WithMessage("Имя должно иметь длину от 3 до 20 символов");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Укажите почту")
                .EmailAddress().WithMessage("Некорректный адрес электронной почты");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Укажите пароль")
                .MinimumLength(6).WithMessage("Пароль должен иметь длину больше 6 символов");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Подтвердите пароль")
                .Equal(x => x.Password).WithMessage("Пароли не совпадают");
        }
    }
}