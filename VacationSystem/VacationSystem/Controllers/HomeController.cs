using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using VacationSystem.Models;
using Microsoft.AspNetCore.Http;
using VacationSystem.Classes;
using VacationSystem.ProgramClasses;
using VacationSystem.ParsingClasses;

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
            /*Connector.GetPositionsList();
            Connector.GetDepartmentsList();
            Connector.GetEmployeeList("1");
            Connector.GetEmployee("25");
            Connector.GetDepartment("1");
            Connector.GetCalendar();*/

            /*DatabaseHandler.RecreateDB();
            DatabaseHandler.LoadData();*/

            return View();
        }

        public IActionResult Profile()
        {
            // загрузка информации о пользователе из API
            string userType = HttpContext.Session.GetString("user_type");

            // данные пользователя

            return View();
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