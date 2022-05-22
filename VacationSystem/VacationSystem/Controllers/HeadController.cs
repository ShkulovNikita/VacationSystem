using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;

using VacationSystem.Models;
using VacationSystem.ViewModels;
using VacationSystem.Classes;
using VacationSystem.Classes.Helpers;
using VacationSystem.ViewModels.ListItems;

namespace VacationSystem.Controllers
{
    public class HeadController : Controller
    {
        /// <summary>
        /// Заглавная страница руководителя подразделений
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }

        /* -------------------------- */
        /* Подразделения руководителя */
        /* -------------------------- */

        /// <summary>
        /// Просмотр списка подразделений данного руководителя
        /// </summary>
        public IActionResult Departments()
        {
            string id = HttpContext.Session.GetString("id");

            if (id == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
                return RedirectToAction("Profile", "Home");
            }

            // получение списка подразделений
            List<Department> departments = Connector.GetSubordinateDepartments(id)
                .OrderBy(d => d.Name)
                .ToList();

            if (departments == null)
            {
                ViewBag.Error = "Не удалось получить данные о подразделениях";
                return View();
            }

            if (departments.Count == 0)
                TempData["Message"] = "Подразделения не найдены";

            return View(departments);
        }

        /// <summary>
        /// Просмотр информации о подчиненном подразделении
        /// </summary>
        /// <param name="id">Идентификатор подразделения</param>
        public IActionResult Department(string id)
        {
            string headId = HttpContext.Session.GetString("id");

            if (headId == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
                return RedirectToAction("Profile", "Home");
            }

            // информация о подразделении 
            Department dep = Connector.GetDepartment(id);

            if (dep == null)
            {
                TempData["Error"] = "Не удалось получить данные о подразделении";
                return View();
            }

            // получение подразделения в формате ViewModel
            DepartmentViewModel department = DepartmentHelper.ConvertDepartmentToViewModel(dep);

            return View(department);
        }

        /// <summary>
        /// Вывод частичного представления со списком сотрудников из указанного подразделения
        /// </summary>
        /// <param name="id">Идентификатор подразделения</param>
        public ActionResult GetItems(string id)
        {
            // получить из сессии всех сотрудников
            List<EmpListItem> allEmps = SessionHelper.GetObjectFromJson<List<EmpListItem>>(HttpContext.Session, "all_employees");
            return PartialView(allEmps.Where(e => e.DepartmentId == id).ToList());
        }

        /// <summary>
        /// Очистить данные в сессии о всех подчиненных сотрудниках и подразделениях
        /// </summary>
        private void ClearListSessionData()
        {
            try
            {
                HttpContext.Session.Remove("all_employees");
                HttpContext.Session.Remove("all_departments");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /* ---------------------- */
        /* Подчиненные сотрудники */
        /* ---------------------- */

        /// <summary>
        /// Просмотр списка подчиненных сотрудников
        /// </summary>
        /// <param name="depId">Идентификатор подразделения</param>
        public IActionResult Employees(string depId, string query, string positionId)
        {
            // идентификатор авторизованного руководителя
            string headId = HttpContext.Session.GetString("id");
            if (headId == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
                return RedirectToAction("Profile", "Home");
            }

            // не задан идентификатор - отображаются все подчиненные сотрудники
            if (depId == null)
            {
                List<Employee> subEmps = Connector.GetSubordinateEmployees(headId);
                if (subEmps == null)
                {
                    TempData["Error"] = "Не удалось загрузить список сотрудников";
                    return View();
                }

                List<Employee> allEmps = subEmps;

                // отфильтровать сотрудников по поисковому запросу
                if (query != null)
                    subEmps = EmployeeHelper.SearchEmployees(subEmps, query);

                // по должности
                if ((positionId != "-1") && (positionId != null))
                    subEmps = FilterByPosition(subEmps, null, positionId);

                // создание модели представления
                EmployeesViewModel emps = EmployeeHelper.ConvertEmployeesToViewModel(subEmps);
                List<Position> positions = PositionHelper.GetEmployeesPositions(allEmps);
                if (positions != null)
                    emps.Positions = positions;
                emps.ChosenPosition = positionId;

                return View(emps);
            }
            // указан идентификатор - отображаются сотрудники указанного подразделения
            else
            {
                // подразделение, для которого нужно получить сотрудников
                Department dep = Connector.GetDepartment(depId);

                // проверка существования подразделения
                if (dep == null)
                {
                    TempData["Error"] = "Подразделение не найдено";
                    return View();
                }

                // получить сотрудников одного подразделения
                List<Employee> subEmps = Connector.GetEmployeesOfDepartment(depId);
                if (subEmps == null)
                {
                    TempData["Error"] = "Не удалось загрузить список сотрудников";
                    return View();
                }

                List<Employee> allEmps = subEmps;

                if (query != null)
                    subEmps = EmployeeHelper.SearchEmployees(subEmps, query);

                if ((positionId != "-1") && (positionId != null))
                    subEmps = FilterByPosition(subEmps, depId, positionId);

                // преобразовать список сотрудников в объект ViewModel
                EmployeesViewModel emps = EmployeeHelper.ConvertEmployeesToViewModel(subEmps, dep);
                List<Position> positions = PositionHelper.GetEmployeesPositions(allEmps, depId);
                if (positions != null)
                    emps.Positions = positions;
                emps.ChosenPosition = positionId;

                return View(emps);
            }
        }

        /// <summary>
        /// Отфильтровать сотрудников по их должностям
        /// </summary>
        private List<Employee> FilterByPosition(List<Employee> employees, string depId, string positionId)
        {
            List<Employee> result = new List<Employee>();

            foreach (Employee emp in employees)
            {
                List<PositionInDepartment> positions;
                if (depId == null)
                    // получить должности сотрудника во всех подразделениях
                    positions = Connector.GetEmployeePositions(emp.Id).Where(p => p.Position == positionId).ToList();
                else
                    positions = Connector.GetPositionsInDepartment(depId, emp.Id).Where(p => p.Position == positionId).ToList();

                if (positions.Count > 0)
                    result.Add(emp);
            }

            return result;
        }

        /// <summary>
        /// Отображение информации об одном сотруднике
        /// </summary>
        /// <param name="id">Идентификатор сотрудника</param>
        public IActionResult Employee(string id)
        {
            // идентификатор авторизованного руководителя
            string headId = HttpContext.Session.GetString("id");
            if (headId == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
                return RedirectToAction("Profile", "Home");
            }

            // попробовать найти сотрудника с указанным идентификатором
            Employee emp = Connector.GetEmployee(id);
            if (emp == null)
            {
                TempData["Error"] = "Не удалось получить данные о сотруднике";
                return View();
            }

            // объект модели представления с данными о сотруднике
            EmployeeViewModel employee = EmployeeHelper.ConvertEmployeeToViewModel(emp);

            return View(employee);
        }
    }
}