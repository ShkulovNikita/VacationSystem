using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using VacationSystem.Models;
using VacationSystem.ViewModels;
using VacationSystem.Classes;

namespace VacationSystem.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Отображение списка подразделений ТПУ
        /// </summary>
        public IActionResult Departments()
        {
            List<Department> departments = DataHandler.GetDepartments();
            if (departments != null)
                return View(departments);
            else
            {
                ViewBag.Error = "Не удалось получить данные о подразделениях";
                return View();
            }
        }

        /// <summary>
        /// Отображение информации об одном подразделении
        /// </summary>
        /// <param name="id">Идентификатор подразделения</param>
        public IActionResult Department(string id)
        {
            // получение информации о подразделении из БД
            Department dep = DataHandler.GetFullDepartmentById(id);

            if (dep == null)
            {
                ViewBag.Error = "Не удалось получить данные о подразделении";
                return View();
            }
            else
            {
                // получение старшего подразделения
                Department headDep = dep.HeadDepartment;

                // получение руководителя подразделения
                Employee headEmp = dep.HeadEmployee;

                // передать данные о подразделении во ViewModel
                DepartmentViewModel department = new DepartmentViewModel()
                {
                    Id = dep.Id,
                    Name = dep.Name,
                    ChildDepartments = dep.ChildDepartments
                };

                if (headDep != null)
                    department.HeadDepartment = headDep;
                if (headEmp != null)
                    department.Head = headEmp;

                return View(department);
            }
        }

        /// <summary>
        /// Отображение списка сотрудников ТПУ (всех или подразделения)
        /// </summary>
        /// <param name="id">Идентификатор подразделения (не обязателен)</param>
        public IActionResult Employees(string id)
        {
            // не задан идентификатор - отображаются все сотрудники ТПУ
            if (id == null)
            {
                // получить всех сотрудников
                List<Employee> employees = DataHandler.GetEmployees();

                if (employees != null)
                {
                    // создание модели представления
                    EmployeesViewModel emps = new EmployeesViewModel
                    {
                        Employees = employees
                    };
                    return View(emps);
                }
                else
                {
                    ViewBag.Error = "Не удалось получить данные о сотрудниках";
                    return View();
                }
            }
            // указан идентификатор - отображаются сотрудники указанного подразделения
            else
            {
                // подразделение, для которого нужно получить сотрудников
                Department dep = DataHandler.GetDepartmentById(id);

                // проверка существования подразделения
                if (dep == null)
                {
                    ViewBag.Error = "Подразделение не найдено";
                    return View();
                }
                else
                {
                    // получить сотрудников одного подразделения
                    List<Employee> employees = DataHandler.GetEmployees(id);

                    if (employees != null)
                    {
                        EmployeesViewModel emps = new EmployeesViewModel
                        {
                            Employees = employees,
                            Department = dep
                        };
                        return View(emps);
                    }
                    else
                    {
                        ViewBag.Error = "Не удалось получить данные о сотрудниках";
                        return View();
                    }
                }
            }
        }

        /// <summary>
        /// Отображение информации об одном сотруднике
        /// </summary>
        /// <param name="id">Идентификатор сотрудника</param>
        public IActionResult Employee(string id)
        {
            Employee emp = DataHandler.GetEmployeeById(id);

            if (emp == null)
            {
                ViewBag.Error = "Не удалось получить данные о сотруднике";
                return View();
            }
            else
                return View(emp);
        }
    }
}