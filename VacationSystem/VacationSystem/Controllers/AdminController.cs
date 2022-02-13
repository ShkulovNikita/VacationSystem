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
            List<Department> departments = new List<Department>();

            departments = DataHandler.GetDepartments();
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
            Department dep = DataHandler.GetDepartmentById(id);

            if (dep == null)
            {
                ViewBag.Error = "Не удалось получить данные о подразделении";
                return View();
            }
            else
            {
                // получение старшего подразделения
                Department headDep = DataHandler.GetDepartmentById(dep.HeadDepartmentId);
                // получение руководителя подразделения
                Employee headEmp = DataHandler.GetDepartmentHead(dep.Id);

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
    }
}