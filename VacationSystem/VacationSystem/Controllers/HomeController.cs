﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using VacationSystem.Models;
using VacationSystem.Classes;

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
            DatabaseHandler.RecreateDB();

            /*Connector.GetPositionsList();
            Connector.GetDepartmentsList();
            Connector.GetEmployeeList("1");
            Connector.GetEmployee("25");
            Connector.GetDepartment("1");
            Connector.GetCalendar();

            DatabaseHandler.LoadData();*/
            
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