using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System;

using VacationSystem.Models;
using VacationSystem.ViewModels;
using VacationSystem.Classes;
using VacationSystem.Classes.Database;
using VacationSystem.Classes.Helpers;
using VacationSystem.Classes.Data;

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

            // добавить сотруднику дни основного оплачиваемого отпуска,
            // если они ещё не были добавлены ранее
            VacationDayHelper.AddMainVacationDays(id);

            // получить список отпусков сотрудника
            List<VacationViewModel> vacations = VacationHelper.MakeVacationsList(id);
            
            if (vacations == null)
            {
                TempData["Error"] = "Не удалось загрузить данные об отпусках";
                return RedirectToAction("Index", "Home");
            }

            if (vacations.Count == 0)
                TempData["Message"] = "Не найдены отпуска";

            return View(vacations);
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
            string lawResult = VacationChecker.CheckLawRules(vacation, null);
            if (lawResult != "success")
            {
                TempData["Error"] = lawResult;
                return View();
            }

            // сохранение в БД
            if (!VacationDataHandler.AddWishedVacation(id, vacation))
                TempData["Error"] = "Не удалось сохранить выбранный период отпуска";
            else
                TempData["Success"] = "Выбранный период отпуска был успешно сохранен!";

            return RedirectToAction("Index");
        }
        
        /// <summary>
        /// Посчитать количество оставшихся у сотрудника отпускных дней
        /// после вычета выбранных им периодов для отпуска
        /// </summary>
        /// <param name="collection">Массив с данными о выбранных периодах отпусках</param>
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

                vacationDays += (int)((endDate - startDate).TotalDays) + 1;
            }

            return availableDays - vacationDays;
        }

        /// <summary>
        /// Отображение календаря отпусков для подразделения
        /// </summary>
        /// <param name="id">Идентификатор подразделения</param>
        public IActionResult Department(string id, int year)
        {


            return View();
        }
    }
}