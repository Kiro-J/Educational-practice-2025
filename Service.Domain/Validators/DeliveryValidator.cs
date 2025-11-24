using FluentValidation;
using Service.Domain.Models;
using System;

namespace Service.Domain.Validators
{
    public class DeliveryValidator : AbstractValidator<Delivery>
    {
        public DeliveryValidator()
        {
            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Адрес доставки обязателен")
                .MinimumLength(5).WithMessage("Укажите корректный адрес");

            RuleFor(x => x.ScheduledDate)
                .GreaterThan(DateTime.Now).WithMessage("Дата доставки должна быть в будущем");

            RuleFor(x => x.OrderId)
                .NotEmpty().WithMessage("Не указан заказ для доставки");
        }
    }
}