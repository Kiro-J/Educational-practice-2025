using Microsoft.AspNetCore.Mvc;
using Service.Domain.ViewModels.LoginAndRegistration;
using System.Diagnostics;
using System.Reflection;

namespace Service.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult SiteInformation() => View();
        public IActionResult Functions() => View();
        public IActionResult Contacts() => View();

        [HttpPost]
        public ActionResult Login([FromBody] LoginViewModel model)
        {
            return Ok(model);
        }
    }
}
