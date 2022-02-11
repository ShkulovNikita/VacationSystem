using System;
using Microsoft.AspNetCore.Mvc;
using VacationSystem.Classes;
using VacationSystem.Models;
using Microsoft.AspNetCore.Http;
using VacationSystem.ParsingClasses;

namespace VacationSystem.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string login)
        {
            // получить пользователя по его логину
            Object user = DataHandler.GetUserByLogin(login);

            // нет пользователя с таким логином
            if (user == null)
            {
                ViewBag.Error = "Пользователь с таким логином не найден";
                return View();
            }
            else
            {
                if (user.GetType() == typeof(Administrator))
                {
                    // получение авторизованного администратора
                    Administrator admin = DataHandler.GetAdminByLogin(login);

                    if (admin == null)
                    {
                        ViewBag.Error = "Ошибка авторизации";
                        return View();
                    }

                    // очистка сессии
                    HttpContext.Session.Clear();

                    // тип пользователя
                    HttpContext.Session.SetString("user_type", "administrator");
                    // идентификатор пользователя
                    HttpContext.Session.SetString("id", admin.Id);

                    return RedirectToAction("Profile", "Home");
                }
                else if (user.GetType() == typeof(Employee))
                {
                    // получение авторизованного сотрудника
                    Employee employee = DataHandler.GetEmployeeById(login);

                    if (employee == null)
                    {
                        ViewBag.Error = "Ошибка авторизации";
                        return View();
                    }

                    // очистка сессии
                    HttpContext.Session.Clear();

                    // тип пользователя
                    HttpContext.Session.SetString("user_type", "employee");

                    // идентификатор пользователя
                    HttpContext.Session.SetString("id", employee.Id);

                    return RedirectToAction("Profile", "Home");
                }
                else
                {
                    ViewBag.Error = "Ошибка авторизации";
                    return View();
                }
            }
        }

        public void Logout()
        {
            HttpContext.Session.Clear();
        }
    }
}