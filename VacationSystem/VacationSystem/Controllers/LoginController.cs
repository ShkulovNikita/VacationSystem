using Microsoft.AspNetCore.Mvc;

namespace VacationSystem.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public string Index(string login)
        {
            return $"Авторизация пользователя: {login}";
        }
    }
}