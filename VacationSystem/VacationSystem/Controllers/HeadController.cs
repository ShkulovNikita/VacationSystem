using Microsoft.AspNetCore.Mvc;
using VacationSystem.Models;
using VacationSystem.ViewModels;
using VacationSystem.Classes;
using VacationSystem.Classes.Database;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Diagnostics;

namespace VacationSystem.Controllers
{
    public class HeadController : Controller
    {
        /// <summary>
        /// Заглавная страница руководителя подразделений
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Просмотр списка подразделений данного руководителя
        /// </summary>
        public IActionResult Departments()
        {
            string id = HttpContext.Session.GetString("id");

            if (id == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
                return RedirectToAction("Index", "Head");
            }

            // получение списка подразделений
            List<Department> departments = Connector.GetSubordinateDepartments(id)
                .OrderBy(d => d.Name)
                .ToList();

            if (departments != null)
                return View(departments);
            else
            {
                ViewBag.Error = "Не удалось получить данные о подразделениях";
                return View();
            }
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
                return RedirectToAction("Index", "Head");
            }

            // информация о подразделении 
            Department dep = Connector.GetDepartment(id);

            if (dep == null)
            {
                TempData["Error"] = "Не удалось получить данные о подразделении";
                return View();
            }

            // получение старшего подразделения
            Department headDep = Connector.GetHeadDepartment(dep.HeadDepartment);

            // получение руководителя подразделения
            Employee headEmp = Connector.GetHeadOfDepartment(dep.Head);

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
        /// Просмотр информации о выбранных стилях руководства для подразделений
        /// </summary>
        public IActionResult Styles()
        {
            string id = HttpContext.Session.GetString("id");

            if (id == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
                return RedirectToAction("Index", "Head");
            }

            // список стилей руководства
            List<HeadStyle> styles = DataHandler.GetHeadStyles(id);

            if (styles == null)
            {
                TempData["Error"] = "Не удалось загрузить данные";
                return RedirectToAction("Index", "Head");
            }

            // подразделения руководителя
            List<Department> deps = Connector.GetSubordinateDepartments(id);

            if (deps == null)
            {
                TempData["Error"] = "Не удалось загрузить данные";
                return RedirectToAction("Index", "Head");
            }

            // стили руководства в подразделениях
            List<HeadStyleViewModel> depStyles = new List<HeadStyleViewModel>();

            // найти стили руководства для всех подразделений руководителя
            foreach(Department dep in deps)
            {
                // найти среди стилей руководителя тот,
                // который применен к данному подразделению
                HeadStyle styleDep = styles
                    .Where(s => s.DepartmentId == dep.Id)
                    .OrderByDescending(s => s.Date)
                    .FirstOrDefault();

                // ничего не найдено - стиль по умолчанию
                if (styleDep == null)
                {
                    ManagementStyle defaultStyle = DataHandler.GetManagementStyle(3);
                    if (defaultStyle == null)
                    {
                        TempData["Error"] = "Произошла ошибка получения данных о стилях руководства";
                        return RedirectToAction("Index", "Head");
                    }

                    depStyles.Add(new HeadStyleViewModel
                    {
                        Id = depStyles.Count,
                        Style = defaultStyle,
                        Department = dep
                    });
                }
                else
                {
                    depStyles.Add(new HeadStyleViewModel
                    {
                        Id = depStyles.Count,
                        Style = styleDep.ManagementStyle,
                        Department = dep
                    });
                }
            }

            return View(depStyles);
        }

        /// <summary>
        /// Изменение стиля руководства для выбранного подразделения
        /// </summary>
        /// <param name="id">Идентификатор подразделения</param>
        [HttpGet]
        public IActionResult EditStyle(string id)
        {
            // идентификатор авторизованного руководителя
            string headId = HttpContext.Session.GetString("id");

            if (headId == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
                return RedirectToAction("Styles");
            }

            // подразделение, к которому будет добавлен стиль
            Department curDep = Connector.GetDepartment(id);
            if (curDep == null)
            {
                TempData["Error"] = "Не удалось загрузить данные о подразделении";
                return RedirectToAction("Styles");
            }

            // текущий стиль руководства
            HeadStyle currentStyle = DataHandler.GetHeadStyle(headId, id);

            int curStyleId = 3;
            if (currentStyle != null)
                curStyleId = currentStyle.ManagementStyle.Id;

            // получить все стили руководства
            List<ManagementStyle> styles = DataHandler.GetManagementStyles();

            if (styles == null)
            {
                TempData["Error"] = "Не удалось загрузить стили руководства";
                return RedirectToAction("Styles");
            }

            EditStyleViewModel viewModel = new EditStyleViewModel
            {
                Styles = styles,
                Department = curDep,
                CurrentStyle = curStyleId
            };

            return View(viewModel);
        }

        /// <summary>
        /// Передача выбранного стиля руководства для выбранного подразделения
        /// </summary>
        [HttpPost]
        public IActionResult EditStyle(string department, int style, int currentstyle)
        {
            // идентификатор авторизованного руководителя
            string headId = HttpContext.Session.GetString("id");

            if (headId == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
                return RedirectToAction("Styles", "Head");
            }

            if (style == currentstyle)
            {
                TempData["Error"] = "Данный стиль уже применен";
                return RedirectToAction("Styles", "Head");
            }

            // попытка сохранения стиля в БД
            if (DataHandler.AddHeadStyle(headId, department, style))
                TempData["Success"] = "Стиль руководства был успешно применен!";
            else
                TempData["Error"] = "Не удалось применить стиль руководства";

            return RedirectToAction("Styles", "Head");
        }

        /// <summary>
        /// Просмотр списка заместителей текущего руководителя
        /// </summary>
        /// <param name="department">Подразделение руководителя</param>
        /// <param name="query">Поисковый запрос</param>
        public IActionResult Deputies(string department, string query)
        {
            string id = HttpContext.Session.GetString("id");

            if (id == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
                return RedirectToAction("Index");
            }

            // список подразделений руководителя
            List<Department> departments = Connector.GetSubordinateDepartments(id).ToList();

            if (departments == null)
            {
                TempData["Error"] = "Не удалось загрузить список подразделений";
                return RedirectToAction("Index");
            }

            // сохранить список всех подразделений во ViewBag
            ViewBag.Departments = departments.OrderBy(d => d.Name).ToList();

            // отфильтровать подразделения по запросу
            if (query != null)
                departments = departments.Where(d => d.Id == query).ToList();

            // получить список заместителей данного руководителя в указанном подразделении
            List<Deputy> deputies = new List<Deputy>();

            if (department != null)
                deputies = DataHandler.GetDeputies(id, department);
            else
                deputies = DataHandler.GetDeputies(id);

            if (deputies == null)
            {
                TempData["Error"] = "Не удалось загрузить данные о заместителях";
                return RedirectToAction("Index");
            }

            // получить заместителей только в тех подразделениях, где руководитель
            // управляет в данный момент
            deputies = deputies
                .Where(deputy => departments.Any(depart => deputy.DepartmentId == depart.Id))
                .ToList();

            // не найдены заместители
            if (deputies.Count == 0)
            {
                TempData["Message"] = "Нет назначенных заместителей";
                return View(new List<DeputyViewModel>());
            }

            // список заместителей в формате ViewModel
            List<DeputyViewModel> deputiesList = new List<DeputyViewModel>();

            foreach(Deputy dep in deputies)
            {
                // данные о заместителе как о сотруднике
                Employee depEmp = Connector.GetEmployee(dep.DeputyEmployeeId);

                if (depEmp != null)
                {
                    // список подразделений, на которые назначен данный заместитель
                    List<Department> depsOfDeputy = new List<Department>();
                    depsOfDeputy = departments
                        .Where(depart => deputies.Any(deputy => deputy.DepartmentId == depart.Id 
                                                        && deputy.DeputyEmployeeId == depEmp.Id))
                        .ToList();

                    if (depsOfDeputy.Count == 0)
                        continue;

                    // сохранить информацию о заместителе во ViewModel
                    deputiesList.Add(new DeputyViewModel
                    {
                        Id = depEmp.Id,
                        FirstName = depEmp.FirstName,
                        MiddleName = depEmp.MiddleName,
                        LastName = depEmp.LastName,
                        Departments = depsOfDeputy
                    });
                }
            }

            return View(deputiesList.OrderBy(d => d.LastName)
                                    .ThenBy(d => d.FirstName)
                                    .ThenBy(d => d.MiddleName)
                                    .ToList());
        }

        /// <summary>
        /// Добавление нового заместителя для руководителя
        /// </summary>
        [HttpGet]
        public IActionResult AddDeputy()
        {
            string id = HttpContext.Session.GetString("id");

            if (id == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
                return RedirectToAction("Deputies");
            }

            // список подразделений руководителя
            List<Department> departments = Connector.GetSubordinateDepartments(id).ToList();

            if (departments == null)
            {
                TempData["Error"] = "Не удалось загрузить список подразделений";
                return RedirectToAction("Deputies");
            }

            // список всех сотрудников
            List<DeputyEmpViewModel> allEmps = new List<DeputyEmpViewModel>();

            // список всех подразделений в формате ViewModel
            List<DeputyDepViewModel> allDeps = new List<DeputyDepViewModel>();

            // получить сотрудников всех подразделений
            foreach (Department dep in departments)
            {
                // добавить подразделение в общий список подразделений
                allDeps.Add(new DeputyDepViewModel
                {
                    Id = dep.Id,
                    Name = dep.Name
                });

                // получить всех сотрудников данного подразделения
                List<Employee> employees = Connector.GetEmployeesOfDepartment(dep.Id);
                if (employees == null)
                    continue;
                
                // добавить сотрудников в списки
                foreach(Employee emp in employees)
                {
                    // проверить наличие такого заместителя в БД
                    if (DataHandler.CheckDeputy(id, emp.Id, dep.Id))
                        continue;

                    // новый сотрудник в формате ViewModel
                    DeputyEmpViewModel newEmp = new DeputyEmpViewModel
                    {
                        EmpId = emp.Id,
                        Name = emp.LastName + " " + emp.FirstName + " " + emp.MiddleName,
                        Department = allDeps.FirstOrDefault(d => d.Id == dep.Id),
                        DepartmentId = dep.Id
                    };

                    // добавить в список сотрудников подразделения
                    allDeps.FirstOrDefault(d => d.Id == dep.Id).Employees.Add(newEmp);

                    // добавить в список всех сотрудников
                    allEmps.Add(newEmp);
                }
            }

            if ((allDeps.Count == 0) || (allEmps.Count == 0))
            {
                TempData["Error"] = "Не удалось загрузить список подразделений";
                return RedirectToAction("Deputies");
            }

            // удалить из списка сотрудников самого руководителя
            allEmps = allEmps.Where(e => e.Id != id).ToList();

            // отсортировать подразделения и сотрудников по алфавиту
            allDeps = allDeps.OrderBy(d => d.Name).ToList();
            allEmps = allEmps.OrderBy(e => e.Name).ToList();

            for (int i = 0; i < allEmps.Count; i++)
                allEmps[i].Id = i.ToString();

            // сохранить списки в сессию
            SessionHelper.SetObjectAsJson(HttpContext.Session, "all_employees", allEmps);
            SessionHelper.SetObjectAsJson(HttpContext.Session, "all_departments", allDeps);

            // вывести списки на страницу

            // индекс по умолчанию
            string selectedIndex = allDeps[0].Id;

            // список подразделений
            SelectList departmentsList = new SelectList(allDeps, "Id", "Name", selectedIndex);
            ViewBag.Deps = departmentsList;
            SelectList employeesList = new SelectList(allEmps.Where(e => e.DepartmentId == selectedIndex), "EmpId", "Name");
            ViewBag.Employees = employeesList;

            return View();
        }

        public ActionResult GetItems(string id)
        {
            // получить из сессии всех сотрудников
            List<DeputyEmpViewModel> allEmps = SessionHelper.GetObjectFromJson<List<DeputyEmpViewModel>>(HttpContext.Session, "all_employees");
            return PartialView(allEmps.Where(e => e.DepartmentId == id).ToList());
        }

        /// <summary>
        /// Сохранение в системе нового заместителя
        /// </summary>
        /// <param name="department">Идентификатор подразделения</param>
        [HttpPost]
        public IActionResult AddDeputy(string Department, string Employee)
        {
            // идентификатор авторизованного руководителя
            string headId = HttpContext.Session.GetString("id");
            if (headId == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
                ClearDeputySessionData();
                return RedirectToAction("Deputies");
            }

            // проверить наличие выбранного сотрудника среди подчиненных руководителя
            if (Connector.GetSubordinateEmployees(headId).FirstOrDefault(e => e.Id == Employee) == null)
            {
                TempData["Error"] = "Выбран некорректный сотрудник";
                return RedirectToAction("AddDeputy");
            }

            // проверить наличие выбранного подразделения среди подчиненных подразделений
            if (Connector.GetSubordinateDepartments(headId).FirstOrDefault(d => d.Id == Department) == null)
            {
                TempData["Error"] = "Выбрано некорректное подразделение";
                return RedirectToAction("AddDeputy");
            }

            if (DataHandler.AddDeputy(headId, Employee, Department))
                TempData["Success"] = "Заместитель успешно сохранен!";
            else
                TempData["Error"] = "Не удалось сохранить заместителя";

            ClearDeputySessionData();
            return RedirectToAction("Deputies");
        }

        /// <summary>
        /// Очистить данные в сессии о всех подчиненных сотрудниках и подразделениях
        /// </summary>
        private void ClearDeputySessionData()
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
    }
}