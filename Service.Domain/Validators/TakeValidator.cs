using FluentValidation;
using Service.Domain.Models;

namespace Service.Domain.Validators
{
    public class TakeValidator : AbstractValidator<Take>
    {
        public TakeValidator()
        {
            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Количество должно быть больше 0");

            RuleFor(x => x.InventoryId)
                .NotEmpty().WithMessage("Необходимо выбрать инвентарь");

            RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage("Привязка к заказу обязательна");
        }
    }
}