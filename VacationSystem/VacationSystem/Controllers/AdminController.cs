using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using VacationSystem.Models;
using VacationSystem.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Http;
using VacationSystem.Classes;

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

            // получение старшего подразделения
            Department headDep = Connector.GetHeadDepartment(dep.Id);

            // получение руководителя подразделения
            Employee headEmp = Connector.GetHeadOfDepartment(dep.Id);

            // получение младших подразделений
            List<Department> lowerDeps = Connector.GetLowerDepartments(dep.Id)
                .OrderBy(d => d.Name)
                .ToList();

            // передать данные о подразделении во ViewModel
            DepartmentViewModel department = new DepartmentViewModel()
            {
                Id = dep.Id,
                Name = dep.Name,
                ChildDepartments = lowerDeps
            };

            if (headDep != null)
                department.HeadDepartment = headDep;
            if (headEmp != null)
                department.Head = headEmp;

            return View(department);
        }

        /// <summary>
        /// Отображение списка сотрудников ТПУ (всех или подразделения)
        /// </summary>
        /// <param name="id">Идентификатор подразделения (не обязателен)</param>
        public IActionResult Employees(string id, string query)
        {
            // проверка авторизации
            if (!CheckAdminPermission())
                return RedirectToAction("Index", "Login");

            // не задан идентификатор - отображаются все сотрудники ТПУ
            if (id == null)
            {
                // получить всех сотрудников
                List<Employee> employees = new List<Employee>();

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

                // пройтись по всем подразделениям, чтобы получить их сотрудников
                foreach(Department dep in deps)
                {
                    // сотрудники одного подразделения
                    List<Employee> empsOfDep = Connector.GetEmployeesOfDepartment(dep.Id);

                    if (empsOfDep == null)
                        continue;

                    // те сотрудники, которые ещё не были добавлены в общий список
                    List<Employee> newEmps = empsOfDep
                        .Where(e => !employees.Any(emp => emp.Id == e.Id))
                        .ToList();

                    employees.AddRange(newEmps);
                }

                if (employees != null)
                {
                    if (query != null)
                        employees = SearchEmployees(employees, query);

                    // создание модели представления
                    EmployeesViewModel emps = new EmployeesViewModel();

                    // создать список сотрудников
                    List<EmpDepViewModel> employeesInUni = new List<EmpDepViewModel>();

                    // конвертировать формат БД в формат модели представления
                    foreach (Employee employee in employees)
                        employeesInUni.Add(new EmpDepViewModel(employee));

                    // передать список сотрудников
                    emps.Employees = employeesInUni
                        .OrderBy(e => e.LastName)
                        .ThenBy(e => e.FirstName)
                        .ThenBy(e => e.MiddleName)
                        .ToList();

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
                Department dep = Connector.GetDepartment(id);

                // проверка существования подразделения
                if (dep == null)
                {
                    ViewBag.Error = "Подразделение не найдено";
                    return View();
                }
                else
                {
                    // получить сотрудников одного подразделения
                    List<Employee> employees = Connector.GetEmployeesOfDepartment(id);

                    if (employees != null)
                    {
                        if (query != null)
                            employees = SearchEmployees(employees, query);

                        // список сотрудников в подразделении с их должностями
                        List<EmpDepViewModel> empsInDep = new List<EmpDepViewModel>();

                        // перебрать полученный список сотрудников подразделения
                        foreach (Employee employee in employees)
                        {
                            // передать в модель представления идентификатор и имя сотрудника
                            EmpDepViewModel empInDep = new EmpDepViewModel(employee);

                            // получить должности сотрудника в подразделении
                            List<PositionInDepartment> positions = Connector.GetPositionsInDepartment(id, employee.Id);

                            if (positions == null)
                                continue;

                            List<Position> posOfDemp = new List<Position>();
                            foreach (PositionInDepartment pos in positions)
                            {
                                Position newPos = Connector.GetPosition(pos.Position);
                                if (newPos != null)
                                    posOfDemp.Add(newPos);
                            }

                            if (posOfDemp.Count > 0)
                            {
                                empInDep.Positions = posOfDemp;
                                empsInDep.Add(empInDep);
                            }
                        }

                        EmployeesViewModel emps = new EmployeesViewModel
                        {
                            Department = dep,
                            Employees = empsInDep
                                .OrderBy(e => e.LastName)
                                .ThenBy(e => e.FirstName)
                                .ThenBy(e => e.MiddleName)
                                .ToList()
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
        /// Отфильтровать имеющийся список сотрудников согласно поисковому запросу
        /// </summary>
        /// <param name="employees">Список всех сотрудников</param>
        /// <param name="query">Поисковый запрос</param>
        /// <returns>Список сотрудников, удовлетворяющих запросу</returns>
        private List<Employee> SearchEmployees(List<Employee> employees, string query)
        {
            return (from emp in employees
                    where emp.FirstName.ToLower().Contains(query.ToLower())
                    || emp.MiddleName.ToLower().Contains(query.ToLower())
                    || emp.LastName.ToLower().Contains(query.ToLower())
                    select emp).ToList();
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
                List<Department> subordinateDepartments = Connector.GetSubordinateDepartments(employee.Id);
                if (subordinateDepartments != null)
                    employee.SubordinateDepartments = subordinateDepartments
                        .OrderBy(d => d.Name)
                        .ToList();

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
            // все должности сотрудника в подразделениях
            List<PositionInDepartment> positions = Connector.GetEmployeePositions(id);

            if (positions == null)
                return null;

            if (positions.Count == 0)
                return null;

            // должности по подразделениям
            List<DepPositionsViewModel> posInDeps = new List<DepPositionsViewModel>();

            // пройти по всем должностям сотрудника
            foreach (PositionInDepartment pos in positions)
            {
                // проверить, было ли добавлено подразделение данной должности в список
                // уже есть такое подразделение - добавить к нему
                if (posInDeps.Any(p => p.Department.Id == pos.Department))
                {
                    // получить соответствующую должность из API
                    Position position = Connector.GetPosition(pos.Position);

                    if (position == null)
                        continue;

                    // добавить должность к уже добавленному подразделению
                    posInDeps.Find(p => p.Department.Id == pos.Department)
                        .Positions.Add(position);
                }
                // для такого подразделения ещё не были добавлены должности
                else
                {
                    // создание новой пары подразделение-должность
                    DepPositionsViewModel depPos = new DepPositionsViewModel();

                    // получить из API данные о подразделении и должности
                    Position position = Connector.GetPosition(pos.Position);
                    Department department = Connector.GetDepartment(pos.Department);

                    if ((position == null) || (department == null))
                        return null;

                    // добавить пару подразделение-должность
                    depPos.Department = department;
                    depPos.Positions.Add(position);

                    // сохранить пару в общий список
                    posInDeps.Add(depPos);
                }
            }

            return posInDeps;
        }
    }
}