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
using VacationSystem.ViewModels.ListItems;

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
            List<DepListItem> allDeps = DepartmentHelper.GetDepartmentsList(departments);

            // список всех сотрудников
            List<EmpListItem> allEmps = DeputyHelper.GetEmployeesList(id, allDeps);

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
            List<EmpListItem> allEmps = SessionHelper.GetObjectFromJson<List<EmpListItem>>(HttpContext.Session, "all_employees");
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
                ClearListSessionData();
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

            ClearListSessionData();
            return RedirectToAction("Deputies");
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
                ClearListSessionData();
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
        [HttpGet]
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

            // список всех подразделений в формате ViewModel
            List<DepListItem> allDeps = DepartmentHelper.GetDepartmentsList(departments);

            // список всех сотрудников
            List<EmpListItem> allEmps = EmployeeHelper.GetEmployeesList(allDeps);

            if ((allDeps.Count == 0) || (allEmps.Count == 0))
            {
                TempData["Error"] = "Не удалось загрузить список подразделений";
                return RedirectToAction("Deputies");
            }

            // сохранить списки в сессию
            SessionHelper.SetObjectAsJson(HttpContext.Session, "all_employees", allEmps);
            SessionHelper.SetObjectAsJson(HttpContext.Session, "all_departments", allDeps);

            // индекс по умолчанию
            string selectedIndex = allDeps[0].Id;

            ViewBag.Departments = allDeps;

            // сотрудники выбранного по умолчанию подразделения
            List<EmpListItem> empsOfDep = allEmps.Where(e => e.DepartmentId == selectedIndex).ToList();
            ViewBag.Employees = empsOfDep;

            return View();
        }

        /// <summary>
        /// Вывод частичного представления со списком сотрудников из указанного подразделения
        /// </summary>
        /// <param name="id">Идентификатор подразделения</param>
        public ActionResult GetGroupItems(string id)
        {
            // получить из сессии всех сотрудников
            List<EmpListItem> allEmps = SessionHelper.GetObjectFromJson<List<EmpListItem>>(HttpContext.Session, "all_employees");
            return PartialView(allEmps.Where(e => e.DepartmentId == id).ToList());
        }

        /// <summary>
        /// Сохранение в БД новой группы
        /// </summary>
        /// <param name="name">Наименование группы</param>
        /// <param name="description">Описание группы</param>
        /// <param name="department">Подразделение сотрудников группы</param>
        /// <param name="Employee">Список идентификаторов выбранных сотрудников</param>
        [HttpPost]
        public IActionResult AddGroup(string name, string description, string department, string[] employees)
        {
            // идентификатор авторизованного руководителя
            string headId = HttpContext.Session.GetString("id");
            if (headId == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
                ClearListSessionData();
                return RedirectToAction("Groups");
            }

            // список сотрудников на основе выбранных идентификаторов
            List<Employee> emps = EmployeeHelper.CheckEmployeesInApi(employees);

            // отсеять тех сотрудников, которые не являются подчиненными для текущего руководителя
            emps = emps
                .Where(employee => Connector.GetSubordinateEmployees(headId)
                .Any(subEmp => employee.Id == subEmp.Id))
                .ToList();

            if(emps.Count == 0)
            {
                TempData["Error"] = "Выбраны некорректные сотрудники";
                return RedirectToAction("Groups");
            }

            if (DataHandler.AddGroup(emps, headId, department, name, description))
                TempData["Success"] = "Группа успешно сохранена!";
            else
                TempData["Error"] = "Не удалось сохранить группу";

            ClearListSessionData();
            return RedirectToAction("Groups");
        }

        /// <summary>
        /// Удаление группы сотрудников
        /// </summary>
        /// <param name="groupId">Идентификатор группы</param>
        public IActionResult DeleteGroup(int groupId)
        {
            // идентификатор авторизованного руководителя
            string headId = HttpContext.Session.GetString("id");
            if (headId == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
                ClearListSessionData();
                return RedirectToAction("Groups");
            }

            // проверка, является ли текущий руководитель создателем группы
            if (DataHandler.GetGroup(groupId).HeadEmployeeId != headId)
            {
                TempData["Error"] = "Нет прав для удаления данной группы";
                ClearListSessionData();
                return RedirectToAction("Groups");
            }

            // попытка удаления группы
            if (DataHandler.DeleteGroup(groupId))
                TempData["Success"] = "Группа была успешно удалена";
            else
                TempData["Error"] = "Не удалось удалить группу";

            return RedirectToAction("Groups");
        }

        /// <summary>
        /// Просмотр информации о группе сотрудников
        /// </summary>
        /// <param name="groupId">Идентификатор группы</param>
        public IActionResult ViewGroup(int groupId)
        {
            string headId = HttpContext.Session.GetString("id");
            if (headId == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
                return RedirectToAction("Index");
            }

            // получить информацию о группе
            Group group = DataHandler.GetGroup(groupId);
            if (group == null)
            {
                TempData["Error"] = "Не удалось получить информацию о группе";
                return RedirectToAction("Groups");
            }

            // конвертировать группу в модель представления
            GroupViewModel groupVm = GroupHelper.ConvertGroupToViewModel(group);
            if (groupVm == null)
            {
                TempData["Error"] = "Не удалось получить информацию о группе";
                return RedirectToAction("Groups");
            }

            return View(groupVm);
        }

        /// <summary>
        /// Редактирование группы сотрудников
        /// </summary>
        /// <param name="groupId">Идентификатор группы</param>
        [HttpGet]
        public IActionResult EditGroup(int groupId)
        {
            // идентификатор авторизованного руководителя
            string headId = HttpContext.Session.GetString("id");
            if (headId == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
                return RedirectToAction("Groups");
            }

            // получить редактируемую группу
            Group group = DataHandler.GetGroup(groupId);
            if (group == null)
            {
                TempData["Error"] = "Не удалось получить данные о группе";
                return RedirectToAction("Groups");
            }

            // преобразовать во ViewModel
            GroupViewModel groupVm = GroupHelper.ConvertGroupToViewModel(group);
            if (groupVm == null)
            {
                TempData["Error"] = "Не удалось получить данные о группе";
                return RedirectToAction("Groups");
            }

            // получить всех сотрудников подразделения группы
            List<Employee> empsOfDep = Connector.GetEmployeesOfDepartment(group.DepartmentId);
            if (empsOfDep == null)
            {
                TempData["Error"] = "Не удалось получить список сотрудников";
                return RedirectToAction("Groups");
            }

            // удалить из их числа сотрудников, уже состоящих в группе
            empsOfDep = empsOfDep
                .Where(empInDep => !groupVm.Employees.Any(empInGroup => empInDep.Id == empInGroup.Id))
                .ToList();
            ViewBag.Employees = empsOfDep
                .OrderBy(e => e.LastName)
                .ThenBy(e => e.FirstName)
                .ThenBy(e => e.MiddleName)
                .ToList();
            
            return View(groupVm);
        }

        /// <summary>
        /// Сохранение изменений в группе в БД
        /// </summary>
        /// <param name="groupId">Идентификатор группы</param>
        /// <param name="name">Наименование группы</param>
        /// <param name="description">Описание группы</param>
        /// <param name="employees">Список сотрудников группы</param>
        [HttpPost]
        public IActionResult EditGroup(int groupId, string name, string description, string[] employees)
        {
            // идентификатор авторизованного руководителя
            string headId = HttpContext.Session.GetString("id");
            if (headId == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
                return RedirectToAction("Groups");
            }

            // получить список сотрудников группы
            List<Employee> empsOfGroup = new List<Employee>();
            foreach (string empId in employees)
            {
                Employee emp = Connector.GetEmployee(empId);
                if (emp != null)
                    empsOfGroup.Add(emp);
            }

            if (DataHandler.EditGroup(groupId, name, description, empsOfGroup))
                TempData["Success"] = "Изменения в группе успешно сохранены!";
            else
                TempData["Error"] = "Не удалось сохранить изменения в группе";

            return RedirectToAction("Groups");
        }

        /* ---------------- */
        /* Правила отпусков */
        /* ---------------- */

        /// <summary>
        /// Просмотр списка установленных правил выбора отпусков
        /// </summary>
        /// <returns></returns>
        public IActionResult Rules(string depId)
        {
            // идентификатор авторизованного руководителя
            string headId = HttpContext.Session.GetString("id");
            if (headId == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
                return RedirectToAction("Groups");
            }

            // список правил текущего руководителя
            List<RuleViewModel> rules = RuleHelper.GetRulesList(headId, depId);

            if (rules.Count == 0)
                TempData["Message"] = "Правила не найдены";

            // список подразделений текущего руководителя
            List<Department> departments = Connector.GetSubordinateDepartments(headId);
            if (departments == null)
            {
                TempData["Error"] = "Не удалось загрузить список подразделений";
                return RedirectToAction("Index");
            }
            ViewBag.departments = departments;

            return View(rules.OrderByDescending(r => r.Date).ToList());
        }

        /// <summary>
        /// Добавление нового правила для сотрудников
        /// </summary>
        [HttpGet]
        public IActionResult AddEmpRule()
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
                return RedirectToAction("Rules");
            }

            // список всех подразделений в формате ViewModel
            List<DepListItem> allDeps = DepartmentHelper.GetDepartmentsList(departments);

            // список всех сотрудников
            List<EmpListItem> allEmps = EmployeeHelper.GetEmployeesList(allDeps);

            if ((allDeps.Count == 0) || (allEmps.Count == 0))
            {
                TempData["Error"] = "Не удалось загрузить список подразделений";
                return RedirectToAction("Rules");
            }

            // сохранить списки в сессию
            SessionHelper.SetObjectAsJson(HttpContext.Session, "all_employees", allEmps);
            SessionHelper.SetObjectAsJson(HttpContext.Session, "all_departments", allDeps);

            // индекс по умолчанию
            string selectedIndex = allDeps[0].Id;

            ViewBag.Departments = allDeps;

            // сотрудники выбранного по умолчанию подразделения
            List<EmpListItem> empsOfDep = allEmps.Where(e => e.DepartmentId == selectedIndex).ToList();
            ViewBag.Employees = empsOfDep;

            // получить все виды правил из БД
            List<RuleType> types = DataHandler.GetRuleTypes();
            if (types == null)
            {
                TempData["Error"] = "Не удалось загрузить типы правил";
                return RedirectToAction("Rules");
            }
            ViewBag.types = types;

            return View();
        }

        /// <summary>
        /// Сохранение нового правила для сотрудников
        /// </summary>
        /// <param name="department">Идентификатор подразделения</param>
        /// <param name="type">Идентификатор типа правила</param>
        /// <param name="description">Описание правила</param>
        /// <param name="employees">Список идентификаторов сотрудников</param>
        [HttpPost]
        public IActionResult AddEmpRule(string department, int type, string description, string[] employees)
        {
            // идентификатор авторизованного руководителя
            string headId = HttpContext.Session.GetString("id");
            if (headId == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
                ClearListSessionData();
                return RedirectToAction("Rules");
            }

            // список сотрудников на основе выбранных идентификаторов
            List<Employee> emps = EmployeeHelper.CheckEmployeesInApi(employees);

            // отсеять тех сотрудников, которые не являются подчиненными для текущего руководителя
            emps = emps
                .Where(employee => Connector.GetSubordinateEmployees(headId)
                .Any(subEmp => employee.Id == subEmp.Id))
                .ToList();

            if (emps.Count == 0)
            {
                TempData["Error"] = "Выбраны некорректные сотрудники";
                return RedirectToAction("Rules");
            }

            if (DataHandler.AddEmployeesRule(description, type, department, headId, emps))
                TempData["Success"] = "Правило успешно сохранено!";
            else
                TempData["Error"] = "Не удалось сохранить правило";

            ClearListSessionData();
            return RedirectToAction("Rules");
        }

        /// <summary>
        /// Удаление правила для сотрудников
        /// </summary>
        /// <param name="ruleId">Идентификатор правила</param>
        public IActionResult DeleteEmpRule(int ruleId)
        {
            if (DataHandler.DeleteEmployeesRule(ruleId))
                TempData["Success"] = "Правило было успешно удалено";
            else
                TempData["Error"] = "Не удалось удалить правило";

            return RedirectToAction("Rules");
        }

        /// <summary>
        /// Просмотр страницы с данными о правиле для сотрудников
        /// </summary>
        /// <param name="ruleId">Идентификатор правила</param>
        public IActionResult ViewEmpRule(int ruleId)
        {
            // получить модель представления с данными о правиле
            EmpRuleViewModel rule = RuleHelper.ConvertEmpRuleToViewModel(ruleId);

            return View(rule);
        }

        /// <summary>
        /// Редактирование правила для сотрудников
        /// </summary>
        /// <param name="ruleId">Идентификатор правила</param>
        [HttpGet]
        public IActionResult EditEmpRule(int ruleId)
        {
            // идентификатор авторизованного руководителя
            string headId = HttpContext.Session.GetString("id");
            if (headId == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
                return RedirectToAction("Rules");
            }

            // получить редактируемое правило
            EmpRuleViewModel rule = RuleHelper.ConvertEmpRuleToViewModel(ruleId);
            if (rule == null)
            {
                TempData["Error"] = "Не удалось получить данные о правиле";
                return RedirectToAction("Rules");
            }

            // все сотрудники подразделения правила
            List<Employee> empsOfDep = Connector.GetEmployeesOfDepartment(rule.Rule.DepartmentId);
            if (empsOfDep == null)
            {
                TempData["Error"] = "Не удалось получить список сотрудников";
                return RedirectToAction("Rules");
            }

            // удалить из их числа сотрудников, уже включенных в правило
            empsOfDep = empsOfDep
                .Where(empInDep => !rule.Employees.Any(empInRule => empInDep.Id == empInRule.Id))
                .ToList();
            ViewBag.Employees = empsOfDep
                .OrderBy(e => e.LastName)
                .ThenBy(e => e.FirstName)
                .ThenBy(e => e.MiddleName)
                .ToList();

            return View(rule);
        }

        /// <summary>
        /// Сохранение в БД изменений правила для сотрудников
        /// </summary>
        /// <param name="ruleId">Идентификатор правила</param>
        /// <param name="description">Описание правила</param>
        /// <param name="employees">Список идентификаторов сотрудников правила</param>
        [HttpPost]
        public IActionResult EditEmpRule(int ruleId, string description, string[] employees)
        {
            // идентификатор авторизованного руководителя
            string headId = HttpContext.Session.GetString("id");
            if (headId == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
                return RedirectToAction("Groups");
            }

            // получить список сотрудников правила
            List<Employee> empsOfRule = new List<Employee>();
            foreach (string empId in employees)
            {
                Employee emp = Connector.GetEmployee(empId);
                if (emp != null)
                    empsOfRule.Add(emp);
            }

            if (DataHandler.EditEmployeesRule(ruleId, description, empsOfRule))
                TempData["Success"] = "Изменения в правиле успешно сохранены!";
            else
                TempData["Error"] = "Не удалось сохранить изменения в правиле";

            return RedirectToAction("Rules");
        }
    }
}