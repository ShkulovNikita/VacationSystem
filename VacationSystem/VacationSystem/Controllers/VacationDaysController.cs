using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;

using VacationSystem.Models;
using VacationSystem.Classes;
using VacationSystem.Classes.Helpers;
using VacationSystem.Classes.Database;
using VacationSystem.ViewModels.ListItems;

namespace VacationSystem.Controllers
{
    /// <summary>
    /// Контроллер, отвечающий за назначение дней отпусков сотрудникам
    /// </summary>
    public class VacationDaysController : Controller
    {
        /// <summary>
        /// Страница для назначения дней отпусков сотрудникам,
        /// подчиненным авторизованному руководителю
        /// </summary>
        [HttpGet]
        public IActionResult SetDays()
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

            // индекс по умолчанию
            string selectedIndex = allDeps[0].Id;
            ViewBag.Departments = allDeps;

            // сотрудники выбранного по умолчанию подразделения
            List<EmpListItem> empsOfDep = allEmps.Where(e => e.DepartmentId == selectedIndex).ToList();
            ViewBag.Employees = empsOfDep;

            // список типов отпусков
            List<VacationType> types = VacationDataHandler.GetVacationTypes();
            ViewBag.Types = types;

            return View();
        }

        /// <summary>
        /// Вывод частичного представления со списком сотрудников из указанного подразделения
        /// </summary>
        /// <param name="id">Идентификатор подразделения</param>
        public ActionResult GetEmployeeItems(string id)
        {
            // получить из сессии всех сотрудников
            List<EmpListItem> allEmps = SessionHelper.GetObjectFromJson<List<EmpListItem>>(HttpContext.Session, "all_employees");
            return PartialView(allEmps.Where(e => e.DepartmentId == id).ToList());
        }

        /// <summary>
        /// Вывод частичного представления с информацией о выпускных днях выбранного сотрудника
        /// </summary>
        /// <param name="id">Идентификатор сотрудника</param>
        public IActionResult GetDaysInfo(string id)
        {
            Employee emp = Connector.GetEmployee(id);
            return PartialView(emp);
        }

        /// <summary>
        /// Сохранение назначенных дней отпуска в БД
        /// </summary>
        /// <param name="employees">Сотрудники, которым добавляются дни отпуска</param>
        /// <param name="type">Выбранный тип отпуска</param>
        /// <param name="number">Количество дней отпуска</param>
        /// <param name="notes">Пояснения</param>
        /// <param name="year">Год, на который назначаются отпускные дни</param>
        [HttpPost]
        public IActionResult SetDays(string[] employees, int type, int number, string notes, int year)
        {
            if (VacationDataHandler.SetVacationDays(employees, type, notes, number, year))
                TempData["Success"] = "Отпускные дни успешно добавлены";
            else
                TempData["Error"] = "Не удалось добавить отпускные дни";

            return RedirectToAction("SetDays", "VacationDays");
        }
    }
}
