using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;

using VacationSystem.Models;
using VacationSystem.ViewModels;
using VacationSystem.Classes;
using VacationSystem.Classes.Database;
using VacationSystem.Classes.Helpers;

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
                return RedirectToAction("Index", "Head");
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
                return RedirectToAction("Index", "Head");
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

        /* ---------------- */
        /* Стили управления */
        /* ---------------- */

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
            depStyles = StylesHelper.FindHeadStyles(deps, styles);

            if (depStyles.Count == 0)
            {
                TempData["Error"] = "Не удалось загрузить стили руководства";
                return RedirectToAction("Index", "Head");
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

        /* ------------------------ */
        /* Заместители руководителя */
        /* ------------------------ */

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
            List<Deputy> deputies = DeputyHelper.GetDeputies(id, department, departments);

            // произошла ошибка
            if (deputies == null)
            {
                TempData["Error"] = "Не удалось загрузить данные о заместителях";
                return RedirectToAction("Index");
            }

            // не найдены заместители
            if (deputies.Count == 0)
            {
                TempData["Message"] = "Нет назначенных заместителей";
                return View(new List<DeputyViewModel>());
            }

            // список заместителей в формате ViewModel
            List<DeputyViewModel> deputiesList = DeputyHelper.ConvertDeputiesToViewModel(deputies, departments);

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

            // список всех подразделений в формате ViewModel
            List<DeputyDepViewModel> allDeps = DeputyHelper.GetDepartmentsList(departments);

            // список всех сотрудников
            List<DeputyEmpViewModel> allEmps = DeputyHelper.GetEmployeesList(id, allDeps);

            if ((allDeps.Count == 0) || (allEmps.Count == 0))
            {
                TempData["Error"] = "Не удалось загрузить список подразделений";
                return RedirectToAction("Deputies");
            }

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

        /// <summary>
        /// Вывод частичного представления со списком сотрудников из указанного подразделения
        /// </summary>
        /// <param name="id">Идентификатор подразделения</param>
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
        /// <param name="Employee">Идентификатор сотрудника-заместителя</param>
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

        /// <summary>
        /// Удаление заместителя
        /// </summary>
        /// <param name="deputyId">Идентификатор сотрудника-заместителя</param>
        public IActionResult DeleteDeputy(string deputyId)
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
            if (Connector.GetSubordinateEmployees(headId).FirstOrDefault(e => e.Id == deputyId) == null)
            {
                TempData["Error"] = "Выбран некорректный сотрудник";
                return RedirectToAction("Deputies");
            }

            // попытка удаления заместителя
            if (DataHandler.DeleteDeputy(headId, deputyId))
                TempData["Success"] = "Заместитель был успешно удален";
            else
                TempData["Error"] = "Не удалось удалить заместителя";

            return RedirectToAction("Deputies");
        }

        /* ---------------------- */
        /* Подчиненные сотрудники */
        /* ---------------------- */

        /// <summary>
        /// Просмотр списка подчиненных сотрудников
        /// </summary>
        /// <param name="depId">Идентификатор подразделения</param>
        public IActionResult Employees(string depId, string query)
        {
            // идентификатор авторизованного руководителя
            string headId = HttpContext.Session.GetString("id");
            if (headId == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
                return RedirectToAction("Index");
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

                // отфильтровать сотрудников по поисковому запросу
                if (query != null)
                    subEmps = EmployeeHelper.SearchEmployees(subEmps, query);

                // создание модели представления
                EmployeesViewModel emps = EmployeeHelper.ConvertEmployeesToViewModel(subEmps);

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

                if (query != null)
                    subEmps = EmployeeHelper.SearchEmployees(subEmps, query);

                // преобразовать список сотрудников в объект ViewModel
                EmployeesViewModel emps = EmployeeHelper.ConvertEmployeesToViewModel(subEmps, dep);

                return View(emps);
            }
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
                return RedirectToAction("Index");
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

        /* ------------------------------ */
        /* Группы подчиненных сотрудников */
        /* ------------------------------ */

        /// <summary>
        /// Просмотр списка групп сотрудников, созданных руководителем
        /// </summary>
        /// <param name="department">Подразделение руководителя</param>
        /// <param name="query">Поисковый запрос</param>
        public IActionResult Groups(string department, string query)
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

            // получить список групп, созданных данным руководителем
            List<Group> groups = GroupHelper.GetGroups(id, department, departments);

            if (groups == null)
            {
                TempData["Error"] = "Не удалось загрузить список групп";
                return RedirectToAction("Index");
            }

            // не найдены группы
            if (groups.Count == 0)
            {
                TempData["Message"] = "Нет созданных групп";
                return View(new List<GroupViewModel>());
            }

            // список групп в формате ViewModel
            List<GroupViewModel> groupsList = GroupHelper.ConvertGroupsToViewModel(groups);

            return View(groupsList);
        }

        /// <summary>
        /// Просмотр группы сотрудников
        /// </summary>
        /// <param name="id">Идентификатор группы</param>
        public IActionResult Group(int id)
        {
            string headId = HttpContext.Session.GetString("id");
            if (headId == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
                return RedirectToAction("Index");
            }

            // получить данные о группе из БД
            Group group = DataHandler.GetGroup(id);
            if (group == null)
            {
                TempData["Error"] = "Не удалось получить данные о группе";
                return RedirectToAction("Groups");
            }

            // преобразовать данные во ViewModel
            GroupViewModel groupVm = GroupHelper.ConvertGroupToViewModel(group);
            if (groupVm == null)
            {
                TempData["Error"] = "Не удалось получить данные о группе";
                return RedirectToAction("Groups");
            }

            return View(groupVm);
        }

        /// <summary>
        /// Добавление новой группы сотрудников
        /// </summary>
        public IActionResult AddGroup()
        {
            string headId = HttpContext.Session.GetString("id");
            if (headId == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
                return RedirectToAction("Index");
            }

            // получить подразделения текущего руководителя
            List<Department> departments = Connector.GetSubordinateDepartments(headId);
            if (departments == null)
            {
                TempData["Error"] = "Не удалось загрузить данные о подразделениях";
                return RedirectToAction("Groups");
            }

            ViewBag.Departments = departments;



        }
    }
}