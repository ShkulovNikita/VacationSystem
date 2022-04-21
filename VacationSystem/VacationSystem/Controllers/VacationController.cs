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
    public class VacationController : Controller
    {
        /// <summary>
        /// Отображение списка отпусков пользователя
        /// </summary>
        public IActionResult Index()
        {
            // получить идентификатор пользователя
            string id = HttpContext.Session.GetString("id");
            if (id == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        /// <summary>
        /// Отображение страницы с выбором периодов отпусков
        /// </summary>
        [HttpGet]
        public IActionResult AddVacation()
        {
            // получить идентификатор пользователя
            string id = HttpContext.Session.GetString("id");
            if (id == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
                return RedirectToAction("Index");
            }

            // получить количество доступных отпускных дней пользователя
            List<VacationDay> days = VacationDayDataHandler.GetAvailableVacationDays(id);
            int availableDays = VacationDayHelper.CountAvailableDays(days);

            HttpContext.Session.SetInt32("available_days", availableDays);

            return View();
        }

        /// <summary>
        /// Сохранение в БД выбранных сотрудником периодов отпуска
        /// </summary>
        /// <param name="startDates">Начальные даты отпуска</param>
        /// <param name="endDates">Конечные даты отпуска</param>
        [HttpPost]
        public IActionResult AddVacation(DateTime[] startDates, DateTime[] endDates)
        {
            // получить идентификатор пользователя
            string id = HttpContext.Session.GetString("id");
            if (id == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
                return RedirectToAction("Index");
            }

            // создать объект с выбранным отпуском сотрудника
            ChosenVacation vacation = VacationHelper.MakeVacation(startDates, endDates);

            // проверить корректность выбранных периодов
            string checkResult = VacationChecker.CheckVacationPeriods(vacation);

            // проверка не пройдена
            if (checkResult != "success")
            {
                TempData["Error"] = checkResult;
                return View();
            }

            // проверка на соответствие ТК РФ
            // TODO

            // сохранение в БД


            return RedirectToAction("Index");
        }

        [HttpPost]
        public int CalculateDays(List<PeriodViewModel> collection)
        {
            // получить доступное сотруднику количество отпускных дней
            int availableDays = (int)HttpContext.Session.GetInt32("available_days");

            // рассчитать разницу в днях между выбранными периодами дней
            int vacationDays = 0;
            for (int i = 0; i < collection.Count - 2; i = i + 2)
            {
                DateTime startDate = Convert.ToDateTime(collection[i].value);
                DateTime endDate = Convert.ToDateTime(collection[i + 1].value);

                vacationDays += (int)((endDate - startDate).TotalDays);
            }

            return availableDays - vacationDays;
        }
    }
}
