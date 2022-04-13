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
    public class GroupsController : Controller
    {
        /// <summary>
        /// Просмотр списка групп сотрудников, созданных руководителем
        /// </summary>
        /// <param name="department">Подразделение руководителя</param>
        /// <param name="query">Поисковый запрос</param>
        public IActionResult Index(string department, string query)
        {
            string id = HttpContext.Session.GetString("id");
            if (id == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
                return RedirectToAction("Index", "Head");
            }

            // список подразделений руководителя
            List<Department> departments = Connector.GetSubordinateDepartments(id).ToList();
            if (departments == null)
            {
                TempData["Error"] = "Не удалось загрузить список подразделений";
                return RedirectToAction("Index", "Head");
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
                return RedirectToAction("Index", "Head");
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
            Group group = GroupDataHandler.GetGroup(id);
            if (group == null)
            {
                TempData["Error"] = "Не удалось получить данные о группе";
                return RedirectToAction("Index");
            }

            // преобразовать данные во ViewModel
            GroupViewModel groupVm = GroupHelper.ConvertGroupToViewModel(group);
            if (groupVm == null)
            {
                TempData["Error"] = "Не удалось получить данные о группе";
                return RedirectToAction("Index");
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
                return RedirectToAction("Index");
            }

            // список всех подразделений в формате ViewModel
            List<DepListItem> allDeps = DepartmentHelper.GetDepartmentsList(departments);

            // список всех сотрудников
            List<EmpListItem> allEmps = EmployeeHelper.GetEmployeesList(allDeps);

            if ((allDeps.Count == 0) || (allEmps.Count == 0))
            {
                TempData["Error"] = "Не удалось загрузить список подразделений";
                return RedirectToAction("Index");
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
        public ActionResult GetEmployeeItems(string id)
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
                return RedirectToAction("Index");
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
                return RedirectToAction("Index");
            }

            if (GroupDataHandler.AddGroup(emps, headId, department, name, description))
                TempData["Success"] = "Группа успешно сохранена!";
            else
                TempData["Error"] = "Не удалось сохранить группу";

            ClearListSessionData();
            return RedirectToAction("Index");
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
                return RedirectToAction("Index");
            }

            // проверка, является ли текущий руководитель создателем группы
            if (GroupDataHandler.GetGroup(groupId).HeadEmployeeId != headId)
            {
                TempData["Error"] = "Нет прав для удаления данной группы";
                ClearListSessionData();
                return RedirectToAction("Index");
            }

            // попытка удаления группы
            if (GroupDataHandler.DeleteGroup(groupId))
                TempData["Success"] = "Группа была успешно удалена";
            else
                TempData["Error"] = "Не удалось удалить группу";

            return RedirectToAction("Index");
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
            Group group = GroupDataHandler.GetGroup(groupId);
            if (group == null)
            {
                TempData["Error"] = "Не удалось получить информацию о группе";
                return RedirectToAction("Index");
            }

            // конвертировать группу в модель представления
            GroupViewModel groupVm = GroupHelper.ConvertGroupToViewModel(group);
            if (groupVm == null)
            {
                TempData["Error"] = "Не удалось получить информацию о группе";
                return RedirectToAction("Index");
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
                return RedirectToAction("Index");
            }

            // получить редактируемую группу
            Group group = GroupDataHandler.GetGroup(groupId);
            if (group == null)
            {
                TempData["Error"] = "Не удалось получить данные о группе";
                return RedirectToAction("Index");
            }

            // преобразовать во ViewModel
            GroupViewModel groupVm = GroupHelper.ConvertGroupToViewModel(group);
            if (groupVm == null)
            {
                TempData["Error"] = "Не удалось получить данные о группе";
                return RedirectToAction("Index");
            }

            // получить всех сотрудников подразделения группы
            List<Employee> empsOfDep = Connector.GetEmployeesOfDepartment(group.DepartmentId);
            if (empsOfDep == null)
            {
                TempData["Error"] = "Не удалось получить список сотрудников";
                return RedirectToAction("Index");
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
                return RedirectToAction("Index");
            }

            // получить список сотрудников группы
            List<Employee> empsOfGroup = new List<Employee>();
            foreach (string empId in employees)
            {
                Employee emp = Connector.GetEmployee(empId);
                if (emp != null)
                    empsOfGroup.Add(emp);
            }

            if (GroupDataHandler.EditGroup(groupId, name, description, empsOfGroup))
                TempData["Success"] = "Изменения в группе успешно сохранены!";
            else
                TempData["Error"] = "Не удалось сохранить изменения в группе";

            return RedirectToAction("Index");
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
    }
}
