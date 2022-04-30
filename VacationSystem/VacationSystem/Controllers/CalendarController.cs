using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System;

using VacationSystem.Models;
using VacationSystem.ViewModels;
using VacationSystem.Classes;
using VacationSystem.Classes.Helpers;
using VacationSystem.Classes.Database;

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
        public IActionResult Department(string id, int year, DateTime? startDate, DateTime? endDate, string type)
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

            if (startDate == null)
                startDate = new DateTime(year, 1, 1);

            if (endDate == null)
                endDate = new DateTime(year, 12, 31);

            // получить список сотрудников подразделения
            List<Employee> employees = Connector.GetEmployeesOfDepartment(id);
            if (employees == null)
            {
                TempData["Error"] = "Не удалось загрузить данные о сотрудниках";
                return RedirectToAction("Index");
            }

            // сформировать на основе списка подчиненных ViewModel с их отпусками
            List<EmpVacationViewModel> calendarVacations = VacationHelper.GetEmployeesVacationsTable(employees, year, type);
            if (calendarVacations == null)
            {
                TempData["Error"] = "Не удалось получить производственный календарь";
                return RedirectToAction("Index");
            }

            // отсортировать по именам сотрудников
            calendarVacations = calendarVacations.OrderBy(c => c.Name).ToList();

            // создать модель представления
            CalendarViewModel calendar = new CalendarViewModel
            {
                Department = department,
                Year = year,
                Type = type,
                StartDate = (DateTime)startDate,
                EndDate = (DateTime)endDate,
                Vacations = calendarVacations
            };

            return View(calendar);
        }

        /// <summary>
        /// Утвердить отпуска сотрудников
        /// </summary>
        /// <param name="id">Идентификатор подразделения</param>
        /// <param name="year">Год, на который назначаются отпуска</param>
        [HttpPost]
        public IActionResult SetVacation(string id, int year)
        {
            // получить всех сотрудников подразделения
            List<Employee> employees = Connector.GetEmployeesOfDepartment(id);

            // получить все желаемые отпуска сотрудников
            foreach (Employee emp in employees)
            {
                // получение периода с наивысшим приоритетом
                WishedVacationPeriod vacation = VacationDataHandler
                    .GetWishedVacations(emp.Id, year)
                    .FirstOrDefault(wv => wv.Priority == 1);
                // сохранение периода в объекте сотрудника
                emp.WishedVacationPeriods.Add(vacation);
            }

            // проверки на соответствие правилам
            

            // если прошли - сохранение в БД
            VacationHelper.SetVacations(employees, year);

            return RedirectToAction("Department");
        }
    }
}
