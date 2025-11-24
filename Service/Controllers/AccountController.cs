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
            // 1. Вызываем сервис, который отправит почту и вернет код
            var response = await _accountService.Register(model);

            if (response.StatusCode == RoleStatusCode.OK)
            {
                // Успех: возвращаем success = true и данные (включая код) для второго шага
                return Json(new { success = true, message = "Код отправлен на почту", data = response.Data });
            }

            return Json(new { success = false, errors = new[] { response.Description } });
        }

        [HttpPost]
        public async Task<JsonResult> ConfirmEmail([FromBody] ConfirmEmailViewModel model)
        {
            // 2. Проверяем код и создаем пользователя
            var response = await _accountService.ConfirmEmail(model);

            if (response.StatusCode == RoleStatusCode.OK)
            {
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(response.Data));

                return Json(new { success = true, message = "Регистрация завершена!" });
            }

            return Json(new { success = false, errors = new[] { response.Description } });
        }

        [HttpPost]
        public async Task<JsonResult> Login([FromBody] LoginViewModel model)
        {
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
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}