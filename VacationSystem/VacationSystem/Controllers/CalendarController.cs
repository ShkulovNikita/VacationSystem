using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System;

using VacationSystem.Models;
using VacationSystem.ViewModels;
using VacationSystem.Classes;
using VacationSystem.Classes.Helpers;

namespace VacationSystem.Controllers
{
    public class CalendarController : Controller
    {
        /// <summary>
        /// Отображение календаря отпусков для подразделения
        /// </summary>
        /// <param name="id">Идентификатор подразделения</param>
        /// <param name="year">Год календаря</param>
        /// <param name="type">Тип отпусков, отображаемый в календаре</param>
        public IActionResult Department(string id, int year, string type)
        {
            // получить идентификатор текущего руководителя
            string headId = HttpContext.Session.GetString("id");
            if (headId == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
                return RedirectToAction("Index");
            }

            Department department = Connector.GetDepartment(id);
            if (department == null)
            {
                TempData["Error"] = "Не удалось загрузить данные о подразделении";
                return RedirectToAction("Index");
            }

            // если год не указан, то текущий
            if (year == 0)
                year = DateTime.Now.Year;

            if (type == null)
                type = "wished";

            // получить список сотрудников подразделения
            List<Employee> employees = Connector.GetEmployeesOfDepartment(id);
            if (employees == null)
            {
                TempData["Error"] = "Не удалось загрузить данные о сотрудниках";
                return RedirectToAction("Index");
            }

            // сформировать на основе списка подчиненных ViewModel с их отпусками
            List<EmpVacationViewModel> calendar = VacationHelper.GetEmployeesVacationsTable(employees, year, type);
            if (calendar == null)
            {
                TempData["Error"] = "Не удалось получить производственный календарь";
                return RedirectToAction("Index");
            }

            // отсортировать по именам сотрудников
            calendar = calendar.OrderBy(c => c.Name).ToList();

            // указать подразделение
            calendar[0].Department = department;

            return View(calendar);
        }

        [HttpPost]
        public int SetVacations(CalendarDateViewModel[] employees)
        {
            return 0;
        }
    }
}
