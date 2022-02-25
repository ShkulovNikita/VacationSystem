using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using VacationSystem.Models;
using VacationSystem.ViewModels;
using System.Linq;
using VacationSystem.Classes.Database;

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
                    EmployeesViewModel emps = new EmployeesViewModel();

                    // создать список сотрудников
                    List<EmpDepViewModel> employeesInUni = new List<EmpDepViewModel>();
                    // конвертировать формат БД в формат модели представления
                    foreach (Employee employee in employees)
                        employeesInUni.Add(new EmpDepViewModel(employee));
                    // передать список сотрудников
                    emps.Employees = employeesInUni;

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
                        // список сотрудников в подразделении с их должностями
                        List<EmpDepViewModel> empsInDep = new List<EmpDepViewModel>();

                        // перебоать полученный список сотрудников подразделения
                        foreach (Employee employee in employees)
                        {
                            // передать в модель представления идентификатор и имя сотрудника
                            EmpDepViewModel empInDep = new EmpDepViewModel(employee);
                            // получить должности сотрудника в подразделении
                            List<Position> positions = DataHandler.GetPositionsOfEmployee(employee.Id, id);
                            if (positions != null)
                            {
                                empInDep.Positions = positions;
                                empsInDep.Add(empInDep);
                            }    
                        }

                        EmployeesViewModel emps = new EmployeesViewModel
                        {
                            Department = dep,
                            Employees = empsInDep
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
            // попробовать найти сотрудника с указанным идентификатором
            Employee emp = DataHandler.GetEmployeeById(id);

            if (emp == null)
            {
                ViewBag.Error = "Не удалось получить данные о сотруднике";
                return View();
            }
            // сотрудник существует, и его удалось получить
            else
            {
                // объект модели представления с данными о сотруднике
                EmployeeViewModel employee = new EmployeeViewModel
                {
                    Id = emp.Id,
                    FirstName = emp.FirstName,
                    MiddleName = emp.MiddleName,
                    LastName = emp.LastName
                };

                // должности сотрудника в его подразделениях
                List<DepPositionsViewModel> positions = GetPositionsInDepartments(employee.Id);
                if (positions != null)
                    employee.PositionsInDepartments = positions;

                // получить подразделения, которыми управляет данный сотрудник
                List<Department> subordinateDepartments = DataHandler.GetSubordinateDepartments(employee.Id);
                if (subordinateDepartments != null)
                    employee.SubordinateDepartments = subordinateDepartments;

                return View(employee);
            }
        }

        /// <summary>
        /// Получить должности сотрудника во всех его подразделениях
        /// </summary>
        /// <param name="id">Идентификатор сотрудника</param>
        /// <returns>Список, содержащий данные о должностях сотрудника в подразделениях</returns>
        static private List<DepPositionsViewModel> GetPositionsInDepartments(string id)
        {
            // получить подразделения и должности сотрудника
            List<EmployeeInDepartment> departments = DataHandler.GetEmployeeDepartments(id);

            if (departments == null)
                return null;
            else
            {
                // должности по подразделениям
                List<DepPositionsViewModel> positions = new List<DepPositionsViewModel>();

                // список уже обработанных подразделений
                List<string> depIds = new List<string>();

                // перебрать все подразделения
                foreach(EmployeeInDepartment dep in departments)
                {
                    // для этого подразделения уже были найдены должности
                    if (depIds.Contains(dep.Department.Id))
                        // добавить должность к уже добавленному подразделению
                        positions.Find(p => p.Department.Id == dep.Department.Id)
                                                     .Positions.Add(dep.Position);
                    else
                    {
                        // создание новой пары подразделение-должность
                        DepPositionsViewModel depPos = new DepPositionsViewModel
                        {
                            Department = dep.Department
                        };
                        depPos.Positions.Add(dep.Position);

                        // сохранение данных о подразделении в должности
                        positions.Add(depPos);

                        // указать, что для данного подразделения
                        // уже была найдена одна должность
                        depIds.Add(dep.Department.Id);
                    }
                }

                return positions;
            }
        }

        /// <summary>
        /// Загрузить/обновить данные из API
        /// для заполнения таблиц БД
        /// </summary>
        /// <param name="obj">Целевые данные в БД, которые следует обновить/загрузить</param>
        public IActionResult Update(string obj)
        {
            // посещение страницы со списком кнопок:
            // обновлять или загружать ещё ничего не нужно
            if (obj == null)
            {
                // получение количества записей в таблицах БД
                ViewBag.HolCount = DataHandler.GetHolidaysCount();
                ViewBag.EmpCount = DataHandler.GetEmployeesCount();
                ViewBag.DepCount = DataHandler.GetDepartmentsCount();
                ViewBag.EmpInDepCount = DataHandler.GetDepartmentsCount();
                ViewBag.HeadDepsCount = DataHandler.GetHeadDepartmentsCount();
                ViewBag.HeadOfDepsCount = DataHandler.GetHeadsOfDepartmentsCount();

                return View();
            }
            // администратор нажал на одну из кнопок
            else
            {
                return View();
            }
        }
    }
}