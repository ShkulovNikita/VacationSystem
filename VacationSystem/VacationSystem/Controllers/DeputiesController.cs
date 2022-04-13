using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

using VacationSystem.Models;
using VacationSystem.ViewModels;
using VacationSystem.Classes;
using VacationSystem.Classes.Database;
using VacationSystem.Classes.Helpers;
using VacationSystem.ViewModels.ListItems;
using System;
using System.Diagnostics;

namespace VacationSystem.Controllers
{
    public class DeputiesController : Controller
    {
        /// <summary>
        /// Просмотр списка заместителей текущего руководителя
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

            // получить список заместителей данного руководителя в указанном подразделении
            List<Deputy> deputies = DeputyHelper.GetDeputies(id, department, departments);

            // произошла ошибка
            if (deputies == null)
            {
                TempData["Error"] = "Не удалось загрузить данные о заместителях";
                return RedirectToAction("Index", "Head");
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
                return RedirectToAction("Index");
            }

            // список подразделений руководителя
            List<Department> departments = Connector.GetSubordinateDepartments(id).ToList();

            if (departments == null)
            {
                TempData["Error"] = "Не удалось загрузить список подразделений";
                return RedirectToAction("Index");
            }

            // список всех подразделений в формате ViewModel
            List<DepListItem> allDeps = DepartmentHelper.GetDepartmentsList(departments);

            // список всех сотрудников
            List<EmpListItem> allEmps = DeputyHelper.GetEmployeesList(id, allDeps);

            if ((allDeps.Count == 0) || (allEmps.Count == 0))
            {
                TempData["Error"] = "Не удалось загрузить список подразделений";
                return RedirectToAction("Index");
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
                return RedirectToAction("Index");
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

            if (DeputyDataHandler.AddDeputy(headId, Employee, Department))
                TempData["Success"] = "Заместитель успешно сохранен!";
            else
                TempData["Error"] = "Не удалось сохранить заместителя";

            ClearListSessionData();
            return RedirectToAction("Index");
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
                return RedirectToAction("Index");
            }

            // проверить наличие выбранного сотрудника среди подчиненных руководителя
            if (Connector.GetSubordinateEmployees(headId).FirstOrDefault(e => e.Id == deputyId) == null)
            {
                TempData["Error"] = "Выбран некорректный сотрудник";
                return RedirectToAction("Index");
            }

            // попытка удаления заместителя
            if (DeputyDataHandler.DeleteDeputy(headId, deputyId))
                TempData["Success"] = "Заместитель был успешно удален";
            else
                TempData["Error"] = "Не удалось удалить заместителя";

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
    }
}
