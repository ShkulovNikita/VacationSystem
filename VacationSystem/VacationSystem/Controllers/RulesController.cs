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
    public class RulesController : Controller
    {
        /// <summary>
        /// Просмотр списка установленных правил выбора отпусков
        /// </summary>
        public IActionResult Index(string depId)
        {
            // идентификатор авторизованного руководителя
            string headId = HttpContext.Session.GetString("id");
            if (headId == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
                return RedirectToAction("Index", "Head");
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
                return RedirectToAction("Index", "Head");
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
                return RedirectToAction("Index");
            }

            // список всех подразделений в формате ViewModel
            List<DepListItem> allDeps = DepartmentHelper.GetDepartmentsList(departments);

            // список всех сотрудников
            List<EmpListItem> allEmps = EmployeeHelper.GetEmployeesList(allDeps).OrderBy(e => e.Name).ToList();

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
            List<EmpListItem> empsOfDep = allEmps.Where(e => e.DepartmentId == selectedIndex).OrderBy(e => e.Name).ToList();
            ViewBag.Employees = empsOfDep;

            // получить все виды правил из БД
            List<RuleType> types = DataHandler.GetRuleTypes();
            if (types == null)
            {
                TempData["Error"] = "Не удалось загрузить типы правил";
                return RedirectToAction("Index");
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
        public IActionResult AddEmpRule(string department, int type, string description, string[] employees, DateTime startDate, DateTime endDate)
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

            startDate = DateHelper.TransformEdgeDate(startDate, false);
            endDate = DateHelper.TransformEdgeDate(endDate, true);

            if (EmployeeRuleDataHandler.AddEmployeesRule(description, type, department, headId, emps, startDate, endDate))
                TempData["Success"] = "Правило успешно сохранено!";
            else
                TempData["Error"] = "Не удалось сохранить правило";

            ClearListSessionData();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Удаление правила для сотрудников
        /// </summary>
        /// <param name="ruleId">Идентификатор правила</param>
        public IActionResult DeleteEmpRule(int ruleId)
        {
            if (EmployeeRuleDataHandler.DeleteEmployeesRule(ruleId))
                TempData["Success"] = "Правило было успешно удалено";
            else
                TempData["Error"] = "Не удалось удалить правило";

            return RedirectToAction("Index");
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
                return RedirectToAction("Index");
            }

            // получить редактируемое правило
            EmpRuleViewModel rule = RuleHelper.ConvertEmpRuleToViewModel(ruleId);
            if (rule == null)
            {
                TempData["Error"] = "Не удалось получить данные о правиле";
                return RedirectToAction("Index");
            }

            // все сотрудники подразделения правила
            List<Employee> empsOfDep = Connector.GetEmployeesOfDepartment(rule.Rule.DepartmentId);
            if (empsOfDep == null)
            {
                TempData["Error"] = "Не удалось получить список сотрудников";
                return RedirectToAction("Index");
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
        public IActionResult EditEmpRule(int ruleId, string description, string[] employees, DateTime startDate, DateTime endDate)
        {
            // идентификатор авторизованного руководителя
            string headId = HttpContext.Session.GetString("id");
            if (headId == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
                return RedirectToAction("Index");
            }

            // получить список сотрудников правила
            List<Employee> empsOfRule = new List<Employee>();
            foreach (string empId in employees)
            {
                Employee emp = Connector.GetEmployee(empId);
                if (emp != null)
                    empsOfRule.Add(emp);
            }

            startDate = DateHelper.TransformEdgeDate(startDate, false);
            endDate = DateHelper.TransformEdgeDate(endDate, true);

            if (EmployeeRuleDataHandler.EditEmployeesRule(ruleId, description, empsOfRule, startDate, endDate))
                TempData["Success"] = "Изменения в правиле успешно сохранены!";
            else
                TempData["Error"] = "Не удалось сохранить изменения в правиле";

            return RedirectToAction("Index");
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
        /// Добавление нового правила выбора отпусков для должностей
        /// </summary>
        [HttpGet]
        public IActionResult AddPosRule()
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
            List<DepListItem> allDeps = DepartmentHelper.GetDepartmentsList(departments)
                .OrderBy(d => d.Name)
                .ToList();

            // список всех должностей
            List<PosListItem> allPos = PositionHelper.GetPositionsList(allDeps)
                .OrderBy(p => p.Name)
                .ToList();

            if ((allDeps.Count == 0) || (allPos.Count == 0))
            {
                TempData["Error"] = "Не удалось загрузить список подразделений";
                return RedirectToAction("Index");
            }

            // сохранить списки в сессию
            SessionHelper.SetObjectAsJson(HttpContext.Session, "all_positions", allPos);
            SessionHelper.SetObjectAsJson(HttpContext.Session, "all_departments", allDeps);

            // индекс по умолчанию
            string selectedIndex = allDeps[0].Id;
            ViewBag.Departments = allDeps;

            // должности выбранного по умолчанию подразделения
            List<PosListItem> posOfDep = allPos.Where(e => e.DepartmentId == selectedIndex).ToList();
            ViewBag.Positions = posOfDep;

            return View();
        }

        /// <summary>
        /// Вывод частичного представления со списком должностей из указанного подразделения
        /// </summary>
        /// <param name="id">Идентификатор подразделения</param>
        public ActionResult GetPositionItems(string id)
        {
            // получить из сессии все должности
            List<PosListItem> allPos = SessionHelper.GetObjectFromJson<List<PosListItem>>(HttpContext.Session, "all_positions");
            return PartialView(allPos.Where(e => e.DepartmentId == id).ToList());
        }

        /// <summary>
        /// Сохранение в БД нового правила для должности
        /// </summary>
        /// <param name="description">Описание правила</param>
        /// <param name="department">Идентификатор подразделения</param>
        /// <param name="positions">Идентификатор должности</param>
        /// <param name="number">Количество сотрудников должности, которые должны быть 
        /// одновременно на рабочем месте</param>
        [HttpPost]
        public IActionResult AddPosRule(string description, string department, string positions, int number, DateTime startDate, DateTime endDate)
        {
            string headId = HttpContext.Session.GetString("id");
            if (headId == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
                return RedirectToAction("Index");
            }

            startDate = DateHelper.TransformEdgeDate(startDate, false);
            endDate = DateHelper.TransformEdgeDate(endDate, true);

            if (PositionRuleDataHandler.AddPositionRule(number, description, positions, department, headId, startDate, endDate))
                TempData["Success"] = "Правило было успешно добавлено!";
            else
                TempData["Error"] = "Не удалось добавить правило";

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Просмотр страницы с информацией о правиле для должности
        /// </summary>
        /// <param name="ruleId">Идентификатор правила</param>
        public IActionResult ViewPosRule(int ruleId)
        {
            PosRuleViewModel rule = RuleHelper.ConvertPosRuleToViewModel(ruleId);
            if (rule == null)
            {
                TempData["Error"] = "Не удалось получить данные о правиле";
                return RedirectToAction("Index");
            }

            return View(rule);
        }

        /// <summary>
        /// Удалить правило для должности
        /// </summary>
        /// <param name="ruleId">Идентификатор должности</param>
        public IActionResult DeletePosRule(int ruleId)
        {
            if (PositionRuleDataHandler.DeletePositionRule(ruleId))
                TempData["Success"] = "Правило успешно удалено";
            else
                TempData["Error"] = "Не удалось удалить правило";

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Редактирование правила для должности
        /// </summary>
        /// <param name="ruleId">Идентификатор правила</param>
        [HttpGet]
        public IActionResult EditPosRule(int ruleId)
        {
            // идентификатор авторизованного руководителя
            string headId = HttpContext.Session.GetString("id");
            if (headId == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
                return RedirectToAction("Index");
            }

            // получить редактируемое правило
            PosRuleViewModel rule = RuleHelper.ConvertPosRuleToViewModel(ruleId);
            if (rule == null)
            {
                TempData["Error"] = "Не удалось получить данные о правиле";
                return RedirectToAction("Index");
            }

            return View(rule);
        }

        /// <summary>
        /// Сохранение в БД изменений в правиле для должности
        /// </summary>
        /// <param name="ruleId">Идентификатор правила</param>
        /// <param name="description">Описание правила</param>
        /// <param name="number">Количество сотрудников должности, которые должны быть 
        /// одновременно на рабочем месте</param>
        [HttpPost]
        public IActionResult EditPosRule(int ruleId, string description, int number, DateTime startDate, DateTime endDate)
        {
            startDate = DateHelper.TransformEdgeDate(startDate, false);
            endDate = DateHelper.TransformEdgeDate(endDate, true);

            if (PositionRuleDataHandler.EditPositionRule(ruleId, number, description, startDate, endDate))
                TempData["Success"] = "Изменения успешно сохранены";
            else
                TempData["Error"] = "Не удалось сохранить изменения";

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Добавление нового правила для группы
        /// </summary>
        [HttpGet]
        public IActionResult AddGroupRule()
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
            List<DepListItem> allDeps = DepartmentHelper.GetDepartmentsList(departments)
                .OrderBy(d => d.Name)
                .ToList();

            // список всех групп
            List<GroupListItem> allGroups = GroupHelper.GetGroupsList(allDeps)
                .OrderBy(g => g.Name)
                .ToList();

            if ((allDeps.Count == 0) || (allGroups.Count == 0))
            {
                TempData["Error"] = "Не удалось загрузить список подразделений";
                return RedirectToAction("Index");
            }

            // сохранить списки в сессию
            SessionHelper.SetObjectAsJson(HttpContext.Session, "all_groups", allGroups);
            SessionHelper.SetObjectAsJson(HttpContext.Session, "all_departments", allDeps);

            // индекс по умолчанию
            string selectedIndex = allDeps[0].Id;
            ViewBag.Departments = allDeps;

            // группы выбранного по умолчанию подразделения
            List<GroupListItem> groupsOfDep = allGroups.Where(e => e.DepartmentId == selectedIndex).ToList();
            ViewBag.Groups = groupsOfDep;

            // получить все виды правил из БД
            List<RuleType> types = DataHandler.GetRuleTypes();
            if (types == null)
            {
                TempData["Error"] = "Не удалось загрузить типы правил";
                return RedirectToAction("Index");
            }
            ViewBag.Types = types;

            return View();
        }

        /// <summary>
        /// Вывод частичного представления со списком групп из указанного подразделения
        /// </summary>
        /// <param name="id">Идентификатор подразделения</param>
        public ActionResult GetGroupItems(string id)
        {
            // получить из сессии все группы
            List<GroupListItem> allGroups = SessionHelper.GetObjectFromJson<List<GroupListItem>>(HttpContext.Session, "all_groups");
            return PartialView(allGroups.Where(e => e.DepartmentId == id).ToList());
        }

        /// <summary>
        /// Сохранение в БД нового правила для группы сотрудников
        /// </summary>
        /// <param name="description">Описание правила</param>
        /// <param name="group">Идентификатор группы</param>
        /// <param name="type">Тип правила</param>
        [HttpPost]
        public IActionResult AddGroupRule(string department, string description, int group, int type, DateTime startDate, DateTime endDate)
        {
            string headId = HttpContext.Session.GetString("id");
            if (headId == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
                return RedirectToAction("Index");
            }

            startDate = DateHelper.TransformEdgeDate(startDate, false);
            endDate = DateHelper.TransformEdgeDate(endDate, true);

            if (GroupRuleDataHandler.AddGroupRule(description, type, group, startDate, endDate))
                TempData["Success"] = "Новое правило успешно сохранено";
            else
                TempData["Error"] = "Не удалось сохранить правило";

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Просмотр информации о правиле для группы
        /// </summary>
        /// <param name="ruleId">Идентификатор правила</param>
        public IActionResult ViewGroupRule(int ruleId)
        {
            GroupRuleViewModel rule = RuleHelper.ConvertGroupRuleToViewModel(ruleId);
            if (rule == null)
            {
                TempData["Error"] = "Не удалось получить данные о правиле";
                return RedirectToAction("Index");
            }

            return View(rule);
        }

        /// <summary>
        /// Удаление правила для группы
        /// </summary>
        /// <param name="ruleId">Идентификатор группы</param>
        public IActionResult DeleteGroupRule(int ruleId)
        {
            if (GroupRuleDataHandler.DeleteGroupRule(ruleId))
                TempData["Success"] = "Правило было успешно удалено";
            else
                TempData["Error"] = "Не удалось удалить правило";

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Редактирование правила для группы сотрудников
        /// </summary>
        /// <param name="ruleId">Идентификатор правила</param>
        [HttpGet]
        public IActionResult EditGroupRule(int ruleId)
        {
            GroupRuleViewModel rule = RuleHelper.ConvertGroupRuleToViewModel(ruleId);
            if (rule == null)
            {
                TempData["Error"] = "Не удалось получить данные о правиле";
                return RedirectToAction("Index");
            }

            return View(rule);
        }

        /// <summary>
        /// Сохранение в БД изменений в правиле для группы сотрудников
        /// </summary>
        /// <param name="ruleId">Идентификатор правила</param>
        /// <param name="description">Описание правила</param>
        [HttpPost]
        public IActionResult EditGroupRule(int ruleId, string description, DateTime startDate, DateTime endDate)
        {
            startDate = DateHelper.TransformEdgeDate(startDate, false);
            endDate = DateHelper.TransformEdgeDate(endDate, true);

            if (GroupRuleDataHandler.EditGroupRule(ruleId, description, startDate, endDate))
                TempData["Success"] = "Изменения успешно сохранены";
            else
                TempData["Error"] = "Не удалось сохранить изменения";

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
