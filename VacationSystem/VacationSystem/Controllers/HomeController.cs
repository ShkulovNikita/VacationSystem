using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using VacationSystem.Classes;
using VacationSystem.Models;
using VacationSystem.Classes.Database;
using VacationSystem.Classes.Helpers;
using VacationSystem.ViewModels;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
            Updater.Update();

            // проверка, куда нужно перенаправить пользователя:
            // если авторизован - в профиль
            // если нет - на страницу авторизации
            if (HttpContext.Session.GetString("id") != null)
                return RedirectToAction("Profile", "Home");
            else
                return RedirectToAction("Index", "Login");
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
                    string userId = HttpContext.Session.GetString("id");
                    if (userId != null)
                    {
                        // проверка, является ли данный пользователь руководителем
                        // какого-нибудь подразделения
                        List<Department> subordinateDeps = Connector.GetSubordinateDepartments(userId);
                        if (subordinateDeps != null)
                            if (subordinateDeps.Count > 0)
                                HttpContext.Session.SetString("head", "true");
                            else
                                HttpContext.Session.SetString("head", "false");
                        else
                            HttpContext.Session.SetString("head", "false");

                        // получение уведомлений для данного пользователя
                        List<Notification> notifications = NotificationDataHandler.GetNotifications(userId);
                        Employee emp = Connector.GetEmployee(userId);

                        HomeViewModel vm = new HomeViewModel
                        {
                            Name = EmployeeHelper.GetFullName(emp),
                            Notifications = notifications.OrderByDescending(n => n.Date).ToList()
                        };

                        return View(vm);
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

        public IActionResult DeleteNotification(int notificationId)
        {
            NotificationDataHandler.DeleteNotification(notificationId);
            return RedirectToAction("Profile");
        }
    }
}