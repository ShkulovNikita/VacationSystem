using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System;

using VacationSystem.Models;
using VacationSystem.ViewModels;
using VacationSystem.Classes;
using VacationSystem.Classes.Rules;
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
        /// <param name="startDate">Начальная дата календаря</param>
        /// <param name="endDate">Конечная дата календаря</param>
        [HttpGet]
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

            // проверить и исправить значения конечных дат периода
            DateTime stDate = DateHelper.CheckDate(startDate, year, false);
            DateTime enDate = DateHelper.CheckDate(endDate, year, true);

            // получить список сотрудников подразделения
            List<Employee> employees = Connector.GetEmployeesOfDepartment(id);
            if (employees == null)
            {
                TempData["Error"] = "Не удалось загрузить данные о сотрудниках";
                return RedirectToAction("Index");
            }

            // сформировать на основе списка подчиненных ViewModel с их отпусками
            List<EmpVacationViewModel> calendarVacations = VacationHelper.GetEmployeesVacationsTable(employees, type, stDate, enDate);
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
                StartDate = stDate,
                EndDate = enDate,
                CurrentType = type,
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
        public JsonResult SetVacation(string id, int year)
        {
            // получить идентификатор текущего руководителя
            string headId = HttpContext.Session.GetString("id");
            if (headId == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
            }

            // получить всех сотрудников подразделения
            List<Employee> employees = Connector.GetEmployeesOfDepartment(id);

            // получить все желаемые отпуска сотрудников
            employees = GetEmployeesWithVacations(employees, year);

            // если прошли - сохранение в БД
            if (VacationHelper.SetVacations(employees, year))
            {
                TempData["Success"] = "Отпуска были успешно утверждены";
                NotificationHelper.SetVacations(headId, employees);
            }
            else
                TempData["Error"] = "Не удалось утвердить отпуска";

            return Json(new { redirectToUrl = Url.Action("Department", "Calendar", new { id, year }) });
        }

        /// <summary>
        /// Создать вспомогательный список сотрудников для проверки выполнения правил, 
        /// где запланированные и утвержденные отпуска помещены в один список
        /// </summary>
        /// <param name="employees">Список сотрудников</param>
        /// <returns>Список сотрудников с сохраненными данными об их отпусках</returns>
        private List<Employee> MakeTempEmployees(List<Employee> employees, int year)
        {
            List<Employee> result = new List<Employee>();

            foreach (Employee emp in employees)
            {
                Employee tempEmp = new Employee
                {
                    Id = emp.Id,
                    FirstName = emp.FirstName,
                    MiddleName = emp.MiddleName,
                    LastName = emp.LastName,
                    Time = emp.Time,
                    BirthDate = emp.BirthDate,
                    StartDate = emp.StartDate,
                    WishedVacationPeriods = emp.WishedVacationPeriods,
                    SetVacations = emp.SetVacations
                };

                result.Add(tempEmp);
            }

            int counter = -1;
            foreach (Employee emp in result)
            {
                counter = counter - 1;
                // преобразовать утвержденные отпуска в желаемые
                List<VacationPart> parts = new List<VacationPart>();

                // нет запланированных отпусков, но будут добавлены утвержденные
                if ((emp.WishedVacationPeriods.Count == 0) && (emp.SetVacations.Count > 0))
                    emp.WishedVacationPeriods.Add(new WishedVacationPeriod
                    {
                        Id = counter,
                        Priority = 1,
                        Date = DateTime.Now,
                        Year = year,
                        EmployeeId = emp.Id
                    });

                foreach (SetVacation vacation in emp.SetVacations)
                {
                    VacationPart part = new VacationPart
                    {
                        Id = parts.Count * (-1),
                        StartDate = vacation.StartDate,
                        EndDate = vacation.EndDate,
                        Part = 0,
                        WishedVacationPeriodId = emp.WishedVacationPeriods[0].Id,
                        WishedVacationPeriod = emp.WishedVacationPeriods[0]
                    };

                    parts.Add(part);
                }

                // когда закончено заполнение всех частей отпуска - сохранить у сотрудника
                if (parts.Count > 0)
                    emp.WishedVacationPeriods[0].VacationParts.AddRange(parts);
            }

            return result;
        }

        /// <summary>
        /// Получить запланированные отпуска для сотрудников
        /// </summary>
        /// <param name="employees">Список сотрудников</param>
        /// <returns>Список сотрудников с сохраненными данными о желаемых отпусках</returns>
        private List<Employee> GetEmployeesWithVacations(List<Employee> employees, int year)
        {
            foreach (Employee emp in employees)
            {
                // получение периода с наивысшим приоритетом
                WishedVacationPeriod vacation = VacationDataHandler
                    .GetWishedVacations(emp.Id, year)
                    .FirstOrDefault(wv => wv.Priority == 1);
                // сохранение периода в объекте сотрудника
                if (vacation != null)
                    emp.WishedVacationPeriods.Add(vacation);
                else
                    emp.WishedVacationPeriods = new List<WishedVacationPeriod>();
            }

            return employees;
        }

        /// <summary>
        /// Получить утвержденные отпуска для сотрудников
        /// </summary>
        /// <param name="employees">Список сотрудников</param>
        /// <param name="year">Год</param>
        /// <returns>Список сотрудников с сохраненными данными об утвержденных отпусках</returns>
        private List<Employee> GetEmployeesWithSetVacations(List<Employee> employees, int year)
        {
            foreach (Employee emp in employees)
            {
                // получение всех утвержденных отпусков
                List<SetVacation> vacations = VacationDataHandler
                    .GetSetVacations(emp.Id, year);
                // сохранение в объекте сотрудника
                if (vacations != null)
                    emp.SetVacations.AddRange(vacations);
                else
                    emp.SetVacations = new List<SetVacation>();
            }

            return employees;
        }

        /// <summary>
        /// Проверка утверждаемых отпусков на соответствие правилам
        /// </summary>
        /// <param name="depId">Идентификатор подразделения</param>
        [HttpPost]
        public JsonResult CheckVacations(string depId, int year)
        {
            // получить идентификатор руководителя
            string headId = HttpContext.Session.GetString("id");
            if (headId == null)
                TempData["Error"] = "Не удалось загрузить данные руководителя";

            // получить всех сотрудников подразделения
            List<Employee> employees = Connector.GetEmployeesOfDepartment(depId);

            // получить все желаемые отпуска сотрудников
            employees = GetEmployeesWithVacations(employees, year);
            // получить также все утвержденные отпуска сотрудников
            employees = GetEmployeesWithSetVacations(employees, year);

            // создать вспомогательный список сотрудников с обеими списками
            List<Employee> tempEmployees = MakeTempEmployees(employees, year);

            // проверки на соответствие правилам
            List<RuleWarning> warnings = EmployeeRulesChecker.CheckEmployeeRules(tempEmployees, headId, depId);
            warnings.AddRange(PositionRulesChecker.CheckPositionRules(tempEmployees, headId, depId));
            warnings.AddRange(GroupRulesChecker.CheckGroupRules(tempEmployees, headId, depId));

            var result = Json(new { warnings });

            return result;
        }

        /// <summary>
        /// Отладочный метод для очистки утвержденных отпусков
        /// </summary>
        /// <param name="id">Идентификатор подразделения</param>
        /// <param name="year">Год, на который назначаются отпуска</param>
        [HttpPost]
        public JsonResult ClearSetVacations(string id, int year)
        {
            bool result = DatabaseHandler.ClearSetVacations();

            if (result)
                TempData["Success"] = "Отпуска были успешно удалены";
            else
                TempData["Error"] = "Не удалось удалить отпуска";

            return Json(new { redirectToUrl = Url.Action("Department", "Calendar", new { id, year }) });
        }
    }
}