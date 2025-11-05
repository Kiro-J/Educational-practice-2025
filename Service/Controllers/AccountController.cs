using Microsoft.AspNetCore.Mvc;
using Service.Domain.ViewModels.LoginAndRegistration;
using Service.Domain;

public class AccountController : Controller
{
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
                // Логика регистрации
                // await _userService.RegisterAsync(model);

                return Json(new { success = true, message = "Регистрация успешна!" });
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
                // Логика входа
                // var result = await _signInManager.PasswordSignInAsync(...);

                // if (result.Succeeded)
                return Json(new { success = true, message = "Вход выполнен успешно!", redirectUrl = Url.Action("Index", "Home") });
                // else
                // return Json(new { success = false, errors = new[] { "Неверный email или пароль" } });
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
}