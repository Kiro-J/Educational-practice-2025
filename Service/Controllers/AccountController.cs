using Microsoft.AspNetCore.Mvc;
using Service.Domain.Response;
using Service.Domain.ViewModels.LoginAndRegistration;
using Service.Services.Interfaces;

namespace Service.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        // Регистрация
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Register([FromBody] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Используем Username как логин
                    var result = await _accountService.Register(model.Username, model.Password, model.Email);

                    if (result.StatusCode == RoleStatusCode.OK)
                    {
                        return Json(new { success = true, message = "Регистрация успешна!" });
                    }
                    else
                    {
                        return Json(new { success = false, errors = new[] { result.Description } });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, errors = new[] { ex.Message } });
                }
            }

            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToArray();

            return Json(new { success = false, errors = errors });
        }

        // Вход
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> Login([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _accountService.Login(model.Login, model.Password);

                    if (result.StatusCode == RoleStatusCode.OK)
                    {
                        HttpContext.Session.SetString("AuthToken", result.Data);
                        return Json(new { success = true, message = "Вход выполнен успешно!" });
                    }
                    else
                    {
                        return Json(new { success = false, errors = new[] { result.Description } });
                    }
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, errors = new[] { ex.Message } });
                }
            }

            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToArray();

            return Json(new { success = false, errors = errors });
        }

        // Выход
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AuthToken");
            return RedirectToAction("Login", "Account");
        }
    }
}