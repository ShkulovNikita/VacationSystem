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
        public IActionResult Index(string empId)
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
            if (empId != null)
                VacationDayHelper.AddMainVacationDays(empId);

            // получить список отпусков сотрудника
            List<VacationViewModel> vacations;
            if ((empId == null) || (empId == id))
                vacations = VacationHelper.MakeVacationsList(id);
            else
            {
                if (EmployeeHelper.IsHead(id, empId))
                    vacations = VacationHelper.MakeVacationsList(empId);
                else
                {
                    TempData["Error"] = "Нет прав для доступа к запрашиваемой странице";
                    return RedirectToAction("Index");
                }
            }
            
            if (vacations == null)
            {
                TempData["Error"] = "Не удалось загрузить данные об отпусках";
                return RedirectToAction("Index", "Home");
            }

            if (vacations.Count == 0)
                TempData["Message"] = "Не найдены отпуска";

            // сотрудник, отпуска которого отображаются
            Employee emp;

            if (empId == null)
                emp = Connector.GetEmployee(id);
            else
                emp = Connector.GetEmployee(empId);

            if (emp == null)
            {
                TempData["Error"] = "Не удалось загрузить данные о сотруднике";
                return RedirectToAction("Index", "Home");
            }

            // создать модель представления
            VacationIndexViewModel vacationVm = new VacationIndexViewModel
            {
                Employee = emp,
                Vacations = vacations
            };

            return View(vacationVm);
        }

        /// <summary>
        /// Отображение страницы с выбором периодов отпусков
        /// </summary>
        [HttpGet]
        public IActionResult AddVacation(string empId)
        {
            // получить идентификатор пользователя
            string id = HttpContext.Session.GetString("id");
            if (id == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
                return RedirectToAction("Index");
            }

            List<VacationDay> days;

            if ((empId == null) || (empId == id))
                days = VacationDayDataHandler.GetAvailableVacationDays(id);
            else
                // если задан идентификатор сотрудника, то отпуск, вероятно, добавляется руководителем
                if (EmployeeHelper.IsHead(id, empId))
                    days = VacationDayDataHandler.GetAvailableVacationDays(empId);
                else
                {
                    TempData["Error"] = "Нет прав для доступа к запрашиваемой странице";
                    return RedirectToAction("Index");
                }

            int availableDays = VacationDayHelper.CountAvailableDays(days);

            HttpContext.Session.SetInt32("available_days", availableDays);

            // сотрудник добавляемого отпуска
            Employee emp;
            if (empId == null)
                emp = Connector.GetEmployee(id);
            else
                emp = Connector.GetEmployee(empId);

            if (emp == null)
            {
                TempData["Error"] = "Не удалось получить данные о сотруднике";
                return RedirectToAction("Index");
            }

            return View(emp);
        }

        /// <summary>
        /// Сохранение в БД выбранных сотрудником периодов отпуска
        /// </summary>
        /// <param name="startDates">Начальные даты отпуска</param>
        /// <param name="endDates">Конечные даты отпуска</param>
        [HttpPost]
        public IActionResult AddVacation(string empId, DateTime[] startDates, DateTime[] endDates)
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
            if ((empId == null) || (empId == id))
            {
                if (!VacationDataHandler.AddWishedVacation(id, vacation))
                    TempData["Error"] = "Не удалось сохранить выбранный период отпуска";
                else
                    TempData["Success"] = "Выбранный период отпуска был успешно сохранен!";
            }
            else
            {
                // добавление отпуска не самим сотрудником, а его руководителем
                if (EmployeeHelper.IsHead(id, empId))
                {
                    if (!VacationDataHandler.AddWishedVacation(empId, vacation))
                        TempData["Error"] = "Не удалось сохранить выбранный период отпуска";
                    else
                        TempData["Success"] = "Выбранный период отпуска был успешно сохранен!";
                }
                else
                {
                    TempData["Error"] = "Нет прав для доступа к запрашиваемой странице";
                    return RedirectToAction("Index");
                }

            }

            return RedirectToAction("Index", new { empId });
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
        /// Отображение страницы для редактирования запланированного отпуска указанного сотрудника
        /// </summary>
        /// <param name="empId">Идентификатор сотрудника</param>
        /// <param name="vacationId">Идентификатор отпуска</param>
        [HttpGet]
        public IActionResult EditWishedVacation(string empId, int vacationId)
        {
            // получить количество доступных отпускных дней пользователя
            List<VacationDay> days = VacationDayDataHandler.GetAvailableVacationDays(empId);
            int availableDays = VacationDayHelper.CountAvailableDays(days);

            HttpContext.Session.SetInt32("available_days", availableDays);

            // получить периоды указанного отпуска сотрудника
            List<VacationDatesViewModel> dates = VacationHelper.GetWishedVacationPeriods(vacationId);

            if (dates == null)
            {
                TempData["Error"] = "Не удалось загрузить данные об отпусках";
                return RedirectToAction("Index");
            }

            EditVacationViewModel vm = new EditVacationViewModel
            {
                Id = vacationId,
                Dates = dates
            };

            return View(vm);
        }

        /// <summary>
        /// Сохранение в БД выбранных периодов отпуска
        /// </summary>
        /// <param name="vacationId">Идентификатор отпуска</param>
        /// <param name="startDates">Начальные даты отпуска</param>
        /// <param name="endDates">Конечные даты отпуска</param>
        [HttpPost]
        public IActionResult EditWishedVacation(int vacationId, DateTime[] startDates, DateTime[] endDates)
        {
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

            // если все в порядке, то сохранить изменения в БД
            if (!VacationDataHandler.EditWishedVacation(vacationId, vacation))
                TempData["Error"] = "Не удалось сохранить изменения в отпуске";
            else
                TempData["Success"] = "Изменения были успешно сохранены";

            return RedirectToAction("Index");
        }
    }
}