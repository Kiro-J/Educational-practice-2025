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
    public IActionResult Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Логика регистрации
            return RedirectToAction("Login");
        }

        // В случае ошибки возвращаем форму с ошибками
        return View(model);
    
    }

    // Вход
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Логика входа
            return RedirectToAction("Index", "Home");
        }
        return View(model);
    }
}
