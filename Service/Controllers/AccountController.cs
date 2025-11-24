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

        // GET методы оставляем без изменений, они просто редиректят
        [HttpGet]
        public IActionResult Register() => RedirectToAction("Index", "Home");

        [HttpGet]
        public IActionResult Login() => RedirectToAction("Index", "Home");

        [HttpPost]
        public async Task<JsonResult> Register([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Вызываем обновленный метод Register, который возвращает ClaimsIdentity
                var response = await _accountService.Register(model.Username, model.Password, model.Email);

                if (response.StatusCode == RoleStatusCode.OK)
                {
                    // Устанавливаем куки аутентификации
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(response.Data));

                    return Json(new { success = true, message = "Регистрация успешна!" });
                }
                return Json(new { success = false, errors = new[] { response.Description } });
            }

            // Обработка ошибок валидации
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage).ToArray();
            return Json(new { success = false, errors = errors });
        }

        [HttpPost]
        public async Task<JsonResult> Login([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var response = await _accountService.Login(model.Login, model.Password);

                if (response.StatusCode == RoleStatusCode.OK)
                {
                    // Устанавливаем куки аутентификации
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(response.Data));

                    return Json(new { success = true, message = "Вход выполнен успешно!" });
                }
                return Json(new { success = false, errors = new[] { response.Description } });
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage).ToArray();
            return Json(new { success = false, errors = errors });
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Удаляем куки аутентификации
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}