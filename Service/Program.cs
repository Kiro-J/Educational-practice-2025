using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Service;
using Service.DAL;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Service.Services; // Добавлено для доступа к AppMappingProfile

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Для работы IHttpContextAccessor в Layout
builder.Services.AddHttpContextAccessor();

// Add AutoMapper
// ИСПРАВЛЕНИЕ: Указываем typeof(AppMappingProfile), чтобы AutoMapper нашел профили в проекте Service.Services
builder.Services.AddAutoMapper(typeof(AppMappingProfile));

// --- ИСПРАВЛЕНИЕ: Вызов метода из Initializer.cs ---
// Этот метод регистрирует AccountService, UserStorage и валидаторы
builder.Services.InitializeRepositories();
// ---------------------------------------------------

// Добавляем аутентификацию через Cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
        options.AccessDeniedPath = new Microsoft.AspNetCore.Http.PathString("/Account/Login");
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    context.Request.Scheme = "http";
    await next();
});

app.UseStaticFiles();

app.UseRouting();

// ВАЖНО: Session должен быть до Authorization
app.UseSession();

// --- ИСПРАВЛЕНИЕ: Сначала Аутентификация, потом Авторизация ---
app.UseAuthentication();
app.UseAuthorization();
// --------------------------------------------------------------

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();