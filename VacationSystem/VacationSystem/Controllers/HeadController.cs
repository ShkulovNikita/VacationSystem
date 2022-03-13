using Microsoft.AspNetCore.Mvc;
using VacationSystem.Models;
using VacationSystem.ViewModels;
using VacationSystem.Classes;
using VacationSystem.Classes.Database;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Linq;

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
                return RedirectToAction("Styles", "Head");
            }

            // подразделение, к которому будет добавлен стиль
            Department curDep = Connector.GetDepartment(id);
            if (curDep == null)
            {
                TempData["Error"] = "Не удалось загрузить данные о подразделении";
                return RedirectToAction("Styles", "Head");
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
                return RedirectToAction("Styles", "Head");
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
        public IActionResult EditStyle(string department, int style)
        {
            // идентификатор авторизованного руководителя
            string headId = HttpContext.Session.GetString("id");

            if (headId == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
                return View();
            }

            // попытка сохранения стиля в БД
            if (DataHandler.AddHeadStyle(headId, department, style))
                TempData["Success"] = "Стиль руководства был успешно применен!";
            else
                TempData["Error"] = "Не удалось применить стиль руководства";

            return RedirectToAction("Styles", "Head");
        }
    }
}