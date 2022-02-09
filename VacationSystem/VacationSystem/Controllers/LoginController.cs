using System;
using Microsoft.AspNetCore.Mvc;
using VacationSystem.Classes;
using VacationSystem.Models;

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
            // получить пользователя по его логину
            Object user = DataHandler.GetUserByLogin(login);

            // нет пользователя с таким логином
            if (user == null)
                return "Авторизация не удалась";
            else
            {
                if (user.GetType() == typeof(Administrator))
                    return "Авторизован администратор";
                else if (user.GetType() == typeof(Employee))
                    return "Авторизован сотрудник ТПУ";
            }

            return $"Авторизация пользователя: {login}";
        }
    }
}