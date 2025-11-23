using Microsoft.EntityFrameworkCore;
using Service.DAL;
using Service.DAL.Storage;
using Service.Domain.ModelsDb;
using Service.Services.Interfaces;
using Service.Services.Realizations;

namespace Service;
public static class Initializer
{
    public static void InitializeRepositoryServices(this IServiceCollection services)
    {
        services.AddScoped<IBaseStorage<UserDb>, UserStorage>();
    }

    public static void InitializeServices(this IServiceCollection services)
    {
        services.AddScoped<IAccountService, AccountService>();
        services.AddControllersWithViews()
            .AddDataAnnotationsLocalization()
            .AddViewLocalization();
    }
}