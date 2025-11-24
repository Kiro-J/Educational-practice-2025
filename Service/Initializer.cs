using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Service.DAL;
using Service.DAL.Storage;
using Service.Domain.Models;
using Service.Domain.ModelsDb;
using Service.Domain.Validators;
using Service.Domain.ViewModels.LoginAndRegistration;
using Service.Services.Interfaces;
using Service.Services.Realizations;

namespace Service
{
    public static class Initializer
    {
        public static void InitializeRepositories(this IServiceCollection services)
        {
            // Регистрация хранилищ (Storage)
            services.AddScoped<IBaseStorage<UserDb>, UserStorage>();

            // Регистрация сервисов
            services.AddScoped<IAccountService, AccountService>();

            // Регистрация валидаторов
            services.AddScoped<IValidator<LoginViewModel>, LoginUserValidator>();
            services.AddScoped<IValidator<RegisterViewModel>, RegisterUserValidator>();


            //(Глава 17) ===
            services.AddScoped<IValidator<Inventory>, InventoryValidator>();
            services.AddScoped<IValidator<Delivery>, DeliveryValidator>();
            services.AddScoped<IValidator<Take>, TakeValidator>();
            services.AddScoped<IValidator<Order>, OrderValidator>();
        }
    }
}