using Microsoft.EntityFrameworkCore;
using Service.DAL;
using Service.DAL.Storage;
using Service.Services.Interfaces;
using Service.Services.Realizations;

namespace Service;
public static class Initializer
{
    public static void InitializeRepositories(this IServiceCollection services)
    {
        services.AddScoped<IAccountService, AccountService>();
        // Добавьте другие сервисы здесь
    }

    public static void InitializeDbContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));
    }
}