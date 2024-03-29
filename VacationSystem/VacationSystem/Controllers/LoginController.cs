﻿using System;
using Microsoft.AspNetCore.Mvc;
using VacationSystem.Models;
using Microsoft.AspNetCore.Http;
using VacationSystem.Classes.Database;
using VacationSystem.Classes;

namespace VacationSystem.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string login)
        {
            // получить пользователя по его логину
            Object user = LoginHandler.GetUser(login);

            // нет пользователя с таким логином
            if (user == null)
            {
                TempData["Error"] = "Пользователь с таким логином не найден";
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
                        TempData["Error"] = "Ошибка авторизации";
                        return View();
                    }

                    // очистка сессии
                    HttpContext.Session.Clear();

                    // тип пользователя
                    HttpContext.Session.SetString("user_type", "administrator");
                    // идентификатор пользователя
                    HttpContext.Session.SetString("id", admin.Id);

                    return RedirectToAction("Index", "Admin");
                }
                else if (user.GetType() == typeof(Employee))
                {
                    // получение авторизованного сотрудника
                    Employee employee = Connector.GetEmployee(login);

                    if (employee == null)
                    {
                        TempData["Error"] = "Ошибка авторизации";
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
                    TempData["Error"] = "Ошибка авторизации";
                    return View();
                }
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}