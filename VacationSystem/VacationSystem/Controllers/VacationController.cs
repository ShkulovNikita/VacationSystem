using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
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
        /// <param name="empId">Идентификатор сотрудника</param>
        /// <param name="type">Тип отображаемых отпусков: запланированные (wished) и утвержденные (set)</param>
        /// <param name="year">Год, на который назначены отпуска</param>
        public IActionResult Index(string empId, string type, int year)
        {
            // получить идентификатор пользователя
            string id = HttpContext.Session.GetString("id");
            if (id == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
                return RedirectToAction("Index", "Home");
            }

            if (empId == null)
                empId = id;

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

            // найти года, в которые у сотрудника есть отпуска
            List<int> years = vacations.Select(v => v.Year).Distinct().ToList();
            int currentYear = DateTime.Now.Year;
            int nextYear = DateTime.Now.AddYears(1).Year;
            if (!years.Contains(currentYear))
                years.Add(currentYear);
            if (!years.Contains(nextYear))
                years.Add(nextYear);
            years = years.OrderBy(el => el).ToList();

            if ((year == 0) && (years.Count > 0))
                year = years[0];
            else if (years.Count == 0)
                year = DateTime.Now.Year;

            // отфильтровать, оставив только те отпуска, которые подходят по году
            vacations = vacations.Where(v => v.EndDate.Year == year).ToList();

            // указан тип отпуска
            if (type != null)
                vacations = vacations.Where(v => v.Type == type).ToList();

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

            bool isHead = false;
            if (empId != null)
                isHead = EmployeeHelper.IsHead(id, empId);

            // посчитать количество дней, которые не были распределены в отпуска
            int freeDays;
            if (empId == null)
                freeDays = VacationDayHelper.CountFreeDays(id, year);
            else
                freeDays = VacationDayHelper.CountFreeDays(empId, year);

            // создать модель представления
            VacationIndexViewModel vacationVm = new VacationIndexViewModel
            {
                Employee = emp,
                Vacations = vacations.OrderBy(v => v.Id).ToList(),
                Years = years,
                AvailableDays = freeDays,
                CurrentType = type,
                CurrentYear = year,
                IsHead = isHead
            };

            return View(vacationVm);
        }

        /// <summary>
        /// Отображение страницы с выбором периодов отпусков
        /// </summary>
        [HttpGet]
        public IActionResult AddVacation(string empId, int year)
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
                days = VacationDayDataHandler.GetAvailableVacationDays(id, year);
            else
                // если задан идентификатор сотрудника, то отпуск, вероятно, добавляется руководителем
                if (EmployeeHelper.IsHead(id, empId))
                    days = VacationDayDataHandler.GetAvailableVacationDays(empId, year);
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
        /// <param name="empId">Идентификатор сотрудника</param>
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

            if (empId == null)
                empId = id;
            Employee emp = Connector.GetEmployee(empId);

            // создать объект с выбранным отпуском сотрудника
            ChosenVacation vacation = VacationHelper.MakeVacation(startDates, endDates);

            if (vacation.Periods == null)
            {
                TempData["Error"] = "Не удалось обработать периоды отпусков";
                return View(emp);
            }
            if (vacation.Periods.Length == 0)
            {
                TempData["Error"] = "Не удалось обработать периоды отпусков";
                return View(emp);
            }

            // добавить туда отпуска, которые уже есть у сотрудника
            vacation.Periods = GetAllVacationPeriods(empId, vacation);

            // проверить корректность выбранных периодов
            string checkResult = VacationChecker.CheckVacationPeriods(vacation);

            // проверка не пройдена
            if (checkResult != "success")
            {
                TempData["Error"] = checkResult;
                return View(emp);
            }

            // проверка на соответствие ТК РФ
            string lawResult = VacationChecker.CheckLawRules(empId, vacation, null);
            if (lawResult != "success")
            {
                TempData["Error"] = lawResult;
                return View(emp);
            }

            if (empId == null)
                empId = id;

            if (!VacationDataHandler.AddWishedVacation(id, vacation))
                TempData["Error"] = "Не удалось сохранить выбранный период отпуска";
            else
            {
                TempData["Success"] = "Выбранный период отпуска был успешно сохранен!";
                NotificationHelper.ChoosingPeriods(empId);
            }

            return RedirectToAction("Index", new { empId });
        }

        /// <summary>
        /// Метод для получения всех периодов отпусков сотрудника: запланированных и желаемых
        /// </summary>
        private ChosenPeriod[] GetAllVacationPeriods(string empId, ChosenVacation vacation)
        {
            List<ChosenPeriod> setPeriods = VacationHelper.GetSetPeriods(empId, vacation.Periods[0].StartDate.Year);
            ChosenPeriod[] periods = new ChosenPeriod[vacation.Periods.Length + setPeriods.Count];
            for (int i = 0; i < vacation.Periods.Length; i++)
                periods[i] = vacation.Periods[i];
            for (int i = vacation.Periods.Length; i < vacation.Periods.Length + setPeriods.Count; i++)
                periods[i] = setPeriods[i - vacation.Periods.Length];
            return periods;
        }

        /// <summary>
        /// Посчитать количество оставшихся у сотрудника отпускных дней
        /// после вычета выбранных им периодов для отпуска
        /// </summary>
        /// <param name="collection">Массив с данными о выбранных периодах отпусках</param>
        [HttpPost]
        public int CalculateDays(List<PeriodViewModel> collection, string empId, int year, int vacationId)
        {
            int availableDays;

            if (vacationId == -1)
            {
                List<VacationDay> days = VacationDayDataHandler.GetAvailableVacationDays(empId, year);
                availableDays = VacationDayHelper.CountAvailableDays(days);
            }
            else
                availableDays = VacationDayHelper.CountTakenDays(vacationId, false);

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
        public IActionResult EditWishedVacation(string empId, int vacationId, int year)
        {
            // получить количество доступных отпускных дней пользователя
            int availableDays = VacationDayHelper.CountTakenDays(vacationId, false);

            HttpContext.Session.SetInt32("available_days", availableDays);

            // получить периоды указанного отпуска сотрудника
            List<VacationDatesViewModel> dates = VacationHelper.GetWishedVacationPeriods(vacationId);

            if (dates == null)
            {
                TempData["Error"] = "Не удалось загрузить данные об отпусках";
                return RedirectToAction("Index");
            }

            // сотрудник, которому задан изменяемый отпуск
            Employee emp = Connector.GetEmployee(empId);
            if (emp == null)
            {
                TempData["Error"] = "Не удалось загрузить данные о сотруднике";
                return RedirectToAction("Index");
            }

            EditVacationViewModel vm = new EditVacationViewModel
            {
                Id = vacationId,
                Dates = dates,
                Employee = emp
            };

            return View(vm);
        }

        private EditVacationViewModel GetEditViewModel(int vacationId, string empId, bool type)
        {
            if (type)
            {
                // получить редактируемый период отпуска
                SetVacation vacation = VacationDataHandler.GetSetVacation(vacationId);

                // сотрудник, которому задан изменяемый отпуск
                Employee emp = Connector.GetEmployee(empId);

                List<VacationDatesViewModel> dates = new List<VacationDatesViewModel>();
                dates.Add(new VacationDatesViewModel
                {
                    StartDate = vacation.StartDate,
                    EndDate = vacation.EndDate
                });

                EditVacationViewModel vm = new EditVacationViewModel
                {
                    Id = vacationId,
                    Dates = dates,
                    Employee = emp
                };

                return vm;
            }
            else
            {
                // получить периоды указанного отпуска сотрудника
                List<VacationDatesViewModel> dates = VacationHelper.GetWishedVacationPeriods(vacationId);

                Employee emp = Connector.GetEmployee(empId);

                EditVacationViewModel vm = new EditVacationViewModel
                {
                    Id = vacationId,
                    Dates = dates,
                    Employee = emp
                };

                return vm;
            }
        }

        /// <summary>
        /// Сохранение в БД выбранных периодов отпуска
        /// </summary>
        /// <param name="vacationId">Идентификатор отпуска</param>
        /// <param name="startDates">Начальные даты отпуска</param>
        /// <param name="endDates">Конечные даты отпуска</param>
        [HttpPost]
        public IActionResult EditWishedVacation(string empId, int vacationId, DateTime[] startDates, DateTime[] endDates)
        {
            // создать объект с выбранным отпуском сотрудника
            ChosenVacation vacation = VacationHelper.MakeVacation(startDates, endDates);

            // проверить корректность выбранных периодов
            string checkResult = VacationChecker.CheckVacationPeriods(vacation);

            // проверка не пройдена
            if (checkResult != "success")
            {
                TempData["Error"] = checkResult;
                return View(GetEditViewModel(vacationId, empId, false));
            }

            // проверка на соответствие ТК РФ
            string lawResult = VacationChecker.CheckLawRules(empId, vacation, null);
            if (lawResult != "success")
            {
                TempData["Error"] = lawResult;
                return View(GetEditViewModel(vacationId, empId, false));
            }

            // если все в порядке, то сохранить изменения в БД
            if (!VacationDataHandler.EditWishedVacation(vacationId, vacation))
                TempData["Error"] = "Не удалось сохранить изменения в отпуске";
            else
                TempData["Success"] = "Изменения были успешно сохранены";

            return RedirectToAction("Index", new { empId });
        }

        /// <summary>
        /// Удаление выбранного запланированного отпуска
        /// </summary>
        /// <param name="vacationId">Идентификатор отпуска в БД</param>
        public IActionResult DeleteWishedVacation(int vacationId)
        {
            WishedVacationPeriod period = VacationDataHandler.GetWishedVacation(vacationId);
            if (period == null)
            {
                TempData["Error"] = "Не удалось загрузить данные об отпуске";
                return RedirectToAction("Index");
            }

            string empId = period.EmployeeId;

            if (VacationDataHandler.DeleteWishedVacation(vacationId))
                TempData["Success"] = "Отпуск был успешно удален";
            else
                TempData["Error"] = "Не удалось удалить отпуск";

            return RedirectToAction("Index", new { empId });
        }

        /// <summary>
        /// Отображение страницы прерывания утвержденного отпуска
        /// </summary>
        /// <param name="vacationId">Идентификатор отпуска</param>
        [HttpGet]
        public IActionResult Interrupt(int vacationId)
        {
            // получить прерываемый отпуск
            SetVacation vacation = VacationDataHandler.GetSetVacation(vacationId);

            if (vacation == null)
            {
                TempData["Error"] = "Не удалось получить данные об отпуске";
                string id = HttpContext.Session.GetString("id");
                return RedirectToAction("Index", new { empId = id });
            }

            return View(vacation);
        }

        /// <summary>
        /// Прерывание утвержденного отпуска
        /// </summary>
        /// <param name="vacationId">Идентификатор отпуска</param>
        /// <param name="interruptionDate">Дата прерывания</param>
        [HttpPost]
        public IActionResult Interrupt(int vacationId, DateTime interruptionDate)
        {
            // получить прерываемый отпуск
            SetVacation vacation = VacationDataHandler.GetSetVacation(vacationId);

            if (vacation == null)
            {
                TempData["Error"] = "Не удалось получить данные об отпуске";
                string id = HttpContext.Session.GetString("id");
                return RedirectToAction("Index", new { empId = id });
            }

            // привести дату прерывания к году отпуска
            DateTime date = new DateTime(vacation.StartDate.Year, interruptionDate.Month, interruptionDate.Day);

            if (date > vacation.EndDate)
            {
                TempData["Error"] = "Выбрана некорректная дата";
                return View(vacation);
            }

            if (VacationDataHandler.InterruptVacation(vacationId, date))
                TempData["Success"] = "Отпуск был успешно прерван";
            else
                TempData["Error"] = "Не удалось прервать отпуск";

            return RedirectToAction("Index", new { empId = vacation.EmployeeId });
        }

        /// <summary>
        /// Отмена отпуска
        /// </summary>
        /// <param name="vacationId">Идентификатор отпуска</param>
        public IActionResult Cancel(int vacationId)
        {
            // получить прерываемый отпуск
            SetVacation vacation = VacationDataHandler.GetSetVacation(vacationId);

            if (vacation == null)
            {
                TempData["Error"] = "Не удалось получить данные об отпуске";
                string id = HttpContext.Session.GetString("id");
                return RedirectToAction("Index", new { empId = id });
            }

            if (VacationDataHandler.CancelVacation(vacationId))
                TempData["Success"] = "Отпуск был успешно отменен";
            else
                TempData["Error"] = "Не удалось отменить отпуск";

            return RedirectToAction("Index", new { empId = vacation.EmployeeId });
        }

        /// <summary>
        /// Редактирование уже утвержденного отпуска
        /// </summary>
        /// <param name="empId">Идентификатор сотрудника</param>
        /// <param name="vacationId">Идентификатор отпуска</param>
        public IActionResult EditSetVacation(string empId, int vacationId)
        {
            string id = HttpContext.Session.GetString("id");
            if (empId == null)
                empId = id;

            // получить редактируемый период отпуска
            SetVacation vacation = VacationDataHandler.GetSetVacation(vacationId);

            if (vacation == null)
            {
                TempData["Error"] = "Не удалось получить данные об отпуске";
                return RedirectToAction("Index", new { empId = empId });
            }

            int availableDays = VacationDayHelper.CountTakenDays(vacationId, true);
            HttpContext.Session.SetInt32("available_days", availableDays);

            // сотрудник, которому задан изменяемый отпуск
            Employee emp = Connector.GetEmployee(empId);
            if (emp == null)
            {
                TempData["Error"] = "Не удалось загрузить данные о сотруднике";
                return RedirectToAction("Index", new { empId });
            }

            List<VacationDatesViewModel> dates = new List<VacationDatesViewModel>();
            dates.Add(new VacationDatesViewModel
            {
                StartDate = vacation.StartDate,
                EndDate = vacation.EndDate
            });

            EditVacationViewModel vm = new EditVacationViewModel
            {
                Id = vacationId,
                Dates = dates,
                Employee = emp
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
        public IActionResult EditSetVacation(string empId, int vacationId, DateTime[] startDates, DateTime[] endDates)
        {
            // получить идентификатор пользователя
            string id = HttpContext.Session.GetString("id");
            if (id == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
                return RedirectToAction("Index");
            }

            if (empId == null)
                empId = id;
            Employee emp = Connector.GetEmployee(empId);

            // создать объект с выбранным отпуском сотрудника
            ChosenVacation vacation = VacationHelper.MakeVacation(startDates, endDates);

            if (vacation.Periods == null)
            {
                TempData["Error"] = "Не удалось обработать периоды отпусков";
                return View(GetEditViewModel(vacationId, empId, true));
            }
            if (vacation.Periods.Length == 0)
            {
                TempData["Error"] = "Не удалось обработать периоды отпусков";
                return View(GetEditViewModel(vacationId, empId, true));
            }

            // добавить туда отпуска, которые уже есть у сотрудника
            vacation.Periods = GetAllVacationPeriods(empId, vacation);

            // исключить оттуда редактируемый
            SetVacation editedVacation = VacationDataHandler.GetSetVacation(vacationId);
            vacation.Periods = vacation.Periods.Where(vp => ((vp.StartDate != editedVacation.StartDate) && (vp.EndDate != editedVacation.EndDate)) ).ToArray();

            // проверить корректность выбранных периодов
            string checkResult = VacationChecker.CheckVacationPeriods(vacation);

            // проверка не пройдена
            if (checkResult != "success")
            {
                TempData["Error"] = checkResult;
                return View(GetEditViewModel(vacationId, empId, true));
            }

            // проверка на соответствие ТК РФ
            string lawResult = VacationChecker.CheckLawRules(empId, vacation, null);
            if (lawResult != "success")
            {
                TempData["Error"] = lawResult;
                return View(GetEditViewModel(vacationId, empId, true));
            }

            // сохранение в БД
            if (!VacationDataHandler.AddWishedVacation(empId, vacation))
            {
                TempData["Error"] = "Не удалось сохранить выбранный период отпуска";
                return RedirectToAction("Index", new { empId });
            }
            else
                TempData["Success"] = "Выбранный период отпуска был успешно сохранен!";

            // удалить исходный утвержденный отпуск
            if (!VacationDataHandler.CancelVacation(vacationId))
                TempData["Error"] = "Произошла ошибка при изменении периода отпуска";

            return RedirectToAction("Index", new { empId });
        }

        /// <summary>
        /// Посчитать количество доступных дней при изменении утвержденного отпуска
        /// </summary>
        /// <param name="collection">Выбранные пользователем периоды</param>
        /// <param name="empId">Идентификатор сотрудника</param>
        /// <param name="year">Год</param>
        /// <param name="vacationId">Идентификатор отпуска</param>
        /// <returns></returns>
        [HttpPost]
        public int CalculateSetDays(List<PeriodViewModel> collection, string empId, int year, int vacationId)
        {
            int availableDays;

            if (vacationId == -1)
            {
                List<VacationDay> days = VacationDayDataHandler.GetAvailableVacationDays(empId, year);
                availableDays = VacationDayHelper.CountAvailableDays(days);
            }
            else
                availableDays = VacationDayHelper.CountTakenDays(vacationId, true);

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
    }
}
