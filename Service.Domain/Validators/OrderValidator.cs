using FluentValidation;
using Service.Domain.Models;

namespace Service.Domain.Validators
{
    public class OrderValidator : AbstractValidator<Order>
    {
        public OrderValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("Пользователь не определен");

            RuleFor(x => x.InventoryId)
                .NotEmpty().WithMessage("Инвентарь должен быть выбран");
        }
    }
}