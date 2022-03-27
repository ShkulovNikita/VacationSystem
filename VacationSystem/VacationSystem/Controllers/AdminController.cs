using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using VacationSystem.Models;
using VacationSystem.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Http;
using VacationSystem.Classes;
using VacationSystem.Classes.Helpers;

namespace VacationSystem.Controllers
{
    public class AdminController : Controller
    {
        /// <summary>
        /// Проверка, разрешен ли пользователю 
        /// доступ к панели администратора
        /// </summary>
        /// <returns>false - запрещен, true - разрешен</returns>
        private bool CheckAdminPermission()
        {
            // проверка авторизации пользователя
            if (HttpContext.Session.GetString("user_type") == null)
            {
                TempData["Error"] = "Авторизуйтесь в системе";
                return false;
            }
            // проверка принадлежности к группе администратора
            if (HttpContext.Session.GetString("user_type") != "administrator")
            {
                TempData["Error"] = "У вас нет прав к этой странице";
                return false;
            }
            // проверка правильности идентификатора администратора
            object user = LoginHandler.GetUser(HttpContext.Session.GetString("id"));
            if (user == null)
            {
                HttpContext.Session.Clear();
                TempData["Error"] = "Неверные данные учетной записи";
                return false;
            }
            if (user.GetType() == typeof(Administrator))
            {
                HttpContext.Session.Clear();
                TempData["Error"] = "У вас нет прав к этой странице";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Главная страница панели администратора
        /// </summary>
        public IActionResult Index()
        {
            // проверка авторизации
            if (!CheckAdminPermission())
                return RedirectToAction("Index", "Login");

            return View();
        }

        /// <summary>
        /// Отображение списка подразделений ТПУ
        /// </summary>
        public IActionResult Departments(string query)
        {
            // проверка авторизации
            if (!CheckAdminPermission())
                return RedirectToAction("Index", "Login");

            List<Department> departments = Connector.GetDepartments()
                                                    .OrderBy(d => d.Name)
                                                    .ToList();

            // если непустой поисковой запрос - произвести фильтрацию
            if (query != null)
                departments = (from dep in departments
                              where dep.Name.ToLower().Contains(query.ToLower())
                              select dep).ToList();

            if (departments == null)
            {
                TempData["Error"] = "Не удалось получить данные о подразделениях";
                return View();
            }

            if (departments.Count == 0)
                TempData["Message"] = "Подразделения не найдены";

            return View(departments);
        }

        /// <summary>
        /// Отображение информации об одном подразделении
        /// </summary>
        /// <param name="id">Идентификатор подразделения</param>
        public IActionResult Department(string id)
        {
            // проверка авторизации
            if (!CheckAdminPermission())
                return RedirectToAction("Index", "Login");

            // получение информации о подразделении из БД
            Department dep = Connector.GetDepartment(id);

            if (dep == null)
            {
                ViewBag.Error = "Не удалось получить данные о подразделении";
                return View();
            }

            // получение подразделения в формате ViewModel
            DepartmentViewModel department = DepartmentHelper.ConvertDepartmentToViewModel(dep);

            return View(department);
        }

        /// <summary>
        /// Отображение списка сотрудников ТПУ (всех или подразделения)
        /// </summary>
        /// <param name="id">Идентификатор подразделения (не обязателен)</param>
        /// <param name="query">Поисковый запрос</param>
        public IActionResult Employees(string id, string query)
        {
            // проверка авторизации
            if (!CheckAdminPermission())
                return RedirectToAction("Index", "Login");

            // не задан идентификатор - отображаются все сотрудники ТПУ
            if (id == null)
            {
                // все подразделения
                List<Department> deps = Connector.GetDepartments();

                if (deps == null)
                {
                    ViewBag.Error = "Не удалось получить данные о подразделениях";
                    return View();
                }

                if (deps.Count == 0)
                {
                    ViewBag.Error = "Не удалось получить данные о подразделениях";
                    return View();
                }

                // получить всех сотрудников
                List<Employee> employees = EmployeeHelper.GetEmployees(deps);
                if (employees == null)
                {
                    ViewBag.Error = "Не удалось получить данные о сотрудниках";
                    return View();
                }

                // отфильтровать сотрудников по поисковому запросу
                if (query != null)
                    employees = EmployeeHelper.SearchEmployees(employees, query);

                // создание модели представления
                EmployeesViewModel emps = EmployeeHelper.ConvertEmployeesToViewModel(employees);

                return View(emps);
            }
            // указан идентификатор - отображаются сотрудники указанного подразделения
            else
            {
                // подразделение, для которого нужно получить сотрудников
                Department dep = Connector.GetDepartment(id);

                // проверка существования подразделения
                if (dep == null)
                {
                    ViewBag.Error = "Подразделение не найдено";
                    return View();
                }

                // получить сотрудников одного подразделения
                List<Employee> employees = Connector.GetEmployeesOfDepartment(id);
                if (employees == null)
                {
                    ViewBag.Error = "Не удалось получить данные о сотрудниках";
                    return View();
                }

                if (query != null)
                    employees = EmployeeHelper.SearchEmployees(employees, query);

                // преобразовать список сотрудников в объект ViewModel
                EmployeesViewModel emps = EmployeeHelper.ConvertEmployeesToViewModel(employees, dep);

                return View(emps);
            }
        }

        /// <summary>
        /// Отображение информации об одном сотруднике
        /// </summary>
        /// <param name="id">Идентификатор сотрудника</param>
        public IActionResult Employee(string id)
        {
            // проверка авторизации
            if (!CheckAdminPermission())
                return RedirectToAction("Index", "Login");

            // попробовать найти сотрудника с указанным идентификатором
            Employee emp = Connector.GetEmployee(id);

            if (emp == null)
            {
                ViewBag.Error = "Не удалось получить данные о сотруднике";
                return View();
            }

            // объект модели представления с данными о сотруднике
            EmployeeViewModel employee = EmployeeHelper.ConvertEmployeeToViewModel(emp);

            return View(employee);
        }
    }
}