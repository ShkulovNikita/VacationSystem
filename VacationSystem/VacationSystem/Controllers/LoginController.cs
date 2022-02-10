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

                    // сохранение данных в сессии

                    // тип пользователя
                    HttpContext.Session.SetString("user_type", "administrator");
                    // идентификатор пользователя
                    HttpContext.Session.SetString("login", admin.Id);

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

                    // загрузка данных из API
                    EmployeeParsed emp = Connector.GetEmployee(employee.Id);

                    if (emp == null)
                    {
                        ViewBag.Error = "Ошибка авторизации";
                        return View();
                    }

                    // очистка сессии
                    HttpContext.Session.Clear();

                    // сохранение данных в сессии

                    // ФИО пользователя
                    HttpContext.Session.SetString("first_name", emp.FirstName);
                    HttpContext.Session.SetString("middle_name", emp.MiddleName);
                    HttpContext.Session.SetString("last_name", emp.LastName);

                    // тип пользователя
                    HttpContext.Session.SetString("user_type", "employee");

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