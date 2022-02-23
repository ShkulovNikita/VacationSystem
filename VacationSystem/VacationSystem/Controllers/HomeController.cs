using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using VacationSystem.Models;
using Microsoft.AspNetCore.Http;
using VacationSystem.Classes;
using VacationSystem.Classes.Database;

namespace VacationSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            //DatabaseHandler.ClearData();
            DatabaseUpdater.UpdateDatabase();

            return View();
        }

        /// <summary>
        /// Профиль авторизованного пользователя
        /// </summary>
        public IActionResult Profile()
        {
            string userType = HttpContext.Session.GetString("user_type");
            string id = HttpContext.Session.GetString("id");

            // в сессии нет значений типа или идентификатора пользователя
            if ((id == null) || (userType == null))
            {
                TempData["Error"] = "Не выполнен вход в систему";
                return RedirectToAction("Index", "Login");
            }

            if (userType == "administrator")
                return View();
            else
            {
                // получение данных из API о пользователе
                Employee empInfo = DataHandler.GetEmployeeById(id);

                if (empInfo == null)
                {
                    TempData["Error"] = "Ошибка загрузки данных пользователя";
                    return RedirectToAction("Index", "Login");
                }
                else
                {
                    // сохранить в сессию данные о пользователе
                    SessionHelper.SetObjectAsJson(HttpContext.Session, "user_info", empInfo);

                    // получить подразделения и должности пользователя
                    
                    // подразделения сотрудника
                    List<Department> departments = new List<Department>();

                    // должности сотрудника
                    List<Position> positions = new List<Position>();

                    // факт руководства подразделением
                    List<bool> head = new List<bool>();

                    // передача полученных данных в представление
                    if ((departments.Count > 0) && (positions.Count > 0) && (head.Count > 0))
                    {
                        ViewBag.Departments = departments;
                        ViewBag.HeadOfDepartment = head;
                        ViewBag.Positions = positions;
                    }
                    else
                    {
                        TempData["Error"] = "Ошибка загрузки данных пользователя";
                        return RedirectToAction("Index", "Login");
                    }
                    
                    return View();
                }
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}