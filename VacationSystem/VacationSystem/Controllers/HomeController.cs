using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using VacationSystem.Models;
using VacationSystem.Classes.Database;
using System.Linq;
using System.Collections.Generic;
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
            // проверка, куда нужно перенаправить пользователя:
            // если авторизован - в профиль
            // если нет - на страницу авторизации
            if (HttpContext.Session.GetString("id") != null)
                return RedirectToAction("Profile", "Home");
            else
                return RedirectToAction("Index", "Login");
        }

        public class EmpHead
        {
            public EmpHead() { }
            public string Id { get; set; }
            public List<Department> SubDeps { get; set; } = new List<Department>();
        }

        /// <summary>
        /// Главная страница профиля пользователя
        /// </summary>
        public IActionResult Profile()
        {
            if (HttpContext.Session.GetString("user_type") != null)
            {
                // у администратора своя админ-панель
                if (HttpContext.Session.GetString("user_type") == "administrator")
                    return RedirectToAction("Index", "Admin");
                else
                {
                    if (HttpContext.Session.GetString("id") != null)
                    {
                        // проверка, является ли данный пользователь руководителем
                        // какого-нибудь подразделения
                        List<Department> subordinateDeps = Connector.GetSubordinateDepartments(HttpContext.Session.GetString("id"));
                        if (subordinateDeps != null)
                            if (subordinateDeps.Count > 0)
                                TempData["head"] = true;
                            else
                                TempData["head"] = false;
                        else
                            TempData["head"] = false;

                        return View();
                    }
                    else
                    {
                        HttpContext.Session.Clear();
                        return RedirectToAction("Index", "Login");
                    }
                }
            }
            else
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Index", "Login");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
