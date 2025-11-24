using FluentValidation;
using Service.Domain.Models;

namespace Service.Domain.Validators
{
    public class InventoryValidator : AbstractValidator<Inventory>
    {
        public InventoryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Название инвентаря обязательно")
                .Length(3, 100).WithMessage("Название должно быть от 3 до 100 символов");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Описание не должно превышать 500 символов");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Цена должна быть больше 0");

            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0).WithMessage("Количество не может быть отрицательным");
        }
    }
}