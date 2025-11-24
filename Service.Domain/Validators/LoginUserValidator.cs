using FluentValidation;
using Service.Domain.ViewModels.LoginAndRegistration;

namespace Service.Domain.Validators
{
    public class LoginUserValidator : AbstractValidator<LoginViewModel>
    {
        public LoginUserValidator()
        {
            RuleFor(x => x.Login)
                .NotEmpty().WithMessage("Введите почту")
                .EmailAddress().WithMessage("Некорректный адрес электронной почты");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Введите пароль")
                .MinimumLength(6).WithMessage("Пароль должен быть больше 6 символов");
        }
    }
}