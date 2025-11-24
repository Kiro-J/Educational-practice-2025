using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Service.Domain.Response;
using Service.Domain.ViewModels.LoginAndRegistration;
using Service.Services.Interfaces;
using System.Security.Claims;

namespace Service.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Register() => RedirectToAction("Index", "Home");

        [HttpGet]
        public IActionResult Login() => RedirectToAction("Index", "Home");

        [HttpPost]
        public async Task<JsonResult> Register([FromBody] RegisterViewModel model)
        {
            // Передаем модель целиком в сервис
            var response = await _accountService.Register(model);

            if (response.StatusCode == RoleStatusCode.OK)
            {
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(response.Data));

                return Json(new { success = true, message = "Регистрация успешна!" });
            }

            // Возвращаем ошибку валидации или логики, пришедшую из сервиса
            return Json(new { success = false, errors = new[] { response.Description } });
        }

        [HttpPost]
        public async Task<JsonResult> Login([FromBody] LoginViewModel model)
        {
            // Передаем модель целиком в сервис
            var response = await _accountService.Login(model);

            if (response.StatusCode == RoleStatusCode.OK)
            {
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(response.Data));

                return Json(new { success = true, message = "Вход выполнен успешно!" });
            }

            return Json(new { success = false, errors = new[] { response.Description } });
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Удаляем куки аутентификации
            await Microsoft.AspNetCore.Authentication.AuthenticationHttpContextExtensions.SignOutAsync(HttpContext, Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}