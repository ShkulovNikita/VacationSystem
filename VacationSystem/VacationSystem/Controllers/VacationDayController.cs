using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;

using VacationSystem.Models;
using VacationSystem.Classes;
using VacationSystem.Classes.Helpers;
using VacationSystem.ViewModels.ListItems;

namespace VacationSystem.Controllers
{
    /// <summary>
    /// Контроллер, отвечающий за назначение дней отпусков сотрудникам
    /// </summary>
    public class VacationDayController : Controller
    {
        /// <summary>
        /// Страница для назначения дней отпусков сотрудникам,
        /// подчиненным авторизованному руководителю
        /// </summary>
        public IActionResult Index()
        {
            // получить идентификатор руководителя
            string headId = HttpContext.Session.GetString("id");
            if (headId == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
                return RedirectToAction("Index");
            }

            // получить подразделения руководителя
            List<Department> departments = Connector.GetSubordinateDepartments(headId);
            if (departments == null)
            {
                TempData["Error"] = "Не удалось загрузить данные о подразделениях";
                return RedirectToAction("Index");
            }

            // список всех подразделений в формате ViewModel
            List<DepListItem> allDeps = DepartmentHelper.GetDepartmentsList(departments);

            // получить сотрудников подразделений руководителя
            List<EmpListItem> allEmps = EmployeeHelper.GetEmployeesList(allDeps);

            // сохранить списки в сессию
            SessionHelper.SetObjectAsJson(HttpContext.Session, "all_employees", allEmps);
            SessionHelper.SetObjectAsJson(HttpContext.Session, "all_departments", allDeps);

            // получить уже назначенные дни отпуска сотрудников


            // собрать все во ViewModel

            return View();
        }

        /// <summary>
        /// Сохранение назначенных дней отпуска в БД
        /// </summary>
        /// <param name="department">Выбранное подразделение руководителя</param>
        /// <param name="employees">Сотрудники, которым добавляются дни отпуска</param>
        /// <param name="type">Выбранный тип отпуска</param>
        /// <param name="number">Количество дней отпуска</param>
        /// <param name="notes">Пояснения</param>
        public IActionResult SetDays(string department, string[] employees, int type, int number, string notes)
        {
            return View();
        }


    }
}
