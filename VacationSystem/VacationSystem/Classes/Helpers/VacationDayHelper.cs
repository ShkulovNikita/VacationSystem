﻿using System;
using System.Linq;
using System.Collections.Generic;

using VacationSystem.Models;
using VacationSystem.ViewModels;
using VacationSystem.ViewModels.ListItems;
using VacationSystem.Classes.Database;

namespace VacationSystem.Classes.Helpers
{
    static public class VacationDayHelper
    {
        /// <summary>
        /// Получить модель представления с днями отпуска на основе
        /// списка сотрудников
        /// </summary>
        /// <param name="emps">Список сотрудников</param>
        /// <returns>Список моделей представления с информацией об отпускных днях
        /// заданных сотрудников</returns>
        static public List<VacationDaysViewModel> MakeDaysList(List<EmpListItem> emps)
        {
            List<VacationDaysViewModel> result = new List<VacationDaysViewModel>();

            foreach (EmpListItem emp in emps)
            {
                // получить дни текущего года
                VacationDaysViewModel daysVmCurrentYear = MakeViewModel(emp, DateTime.Now.Year);
                result.Add(daysVmCurrentYear);
                // получить дни следующего года
                VacationDaysViewModel daysVmNextYear = MakeViewModel(emp, DateTime.Now.AddYears(1).Year);
                result.Add(daysVmNextYear);
            }

            return result;
        }

        /// <summary>
        /// Создать модель представления о днях отпуска для указанного сотрудника
        /// </summary>
        /// <param name="emp">Сотрудник</param>
        /// <returns>Модель представления с информацией о днях отпуска сотрудника</returns>
        static private VacationDaysViewModel MakeViewModel(EmpListItem emp, int year)
        {
            // все отпускные дни сотрудника
            List<VacationDay> vacationDays = VacationDayDataHandler.GetVacationDays(emp.EmpId);

            // отобрать только актуальные дни
            vacationDays = GetCurrentDays(vacationDays, year, false);

            VacationDaysViewModel daysVm = new VacationDaysViewModel();

            // общее количество выпускных дней
            daysVm.TotalDays = CountTotalDays(vacationDays);

            // общее количество доступных выпускных дней
            daysVm.AvailableDays = CountAvailableDays(vacationDays);

            // получить распределение дней по их типам
            daysVm.SetDays = GetDaysInfo(vacationDays);

            // указать сотрудника, которому заданы данные отпускные дни
            daysVm.EmployeeId = emp.EmpId;

            return daysVm;
        }

        /// <summary>
        /// Получить информацию только об актуальных дня отпуска:
        /// - неистраченных за прошлый год
        /// - назначенных на текущий год
        /// </summary>
        /// <param name="days">Список всех дней, назначенных сотруднику</param>
        /// <param name="year">Год, на который назначены дни отпуска</param>
        /// <param name="currentYear">true: получить дни для текущего года; false: для следующего</param>
        /// <returns>Список актуальных дней отпуска</returns>
        static private List<VacationDay> GetCurrentDays(List<VacationDay> days, int year, bool currentYear)
        {
            List<VacationDay> result = days.Where(d =>
                d.Year == DateTime.Now.Year 
                || d.UsedDays < d.NumberOfDays
            ).ToList();

            return result;
        }

        /// <summary>
        /// Посчитать общее количество назначенных отпускных дней
        /// </summary>
        /// <param name="days">Список отпускных дней</param>
        /// <returns>Общее количество отпускных дней сотрудники на данный момент</returns>
        static private int CountTotalDays(List<VacationDay> days)
        {
            int result = 0;
            foreach (VacationDay day in days)
                result = result + day.NumberOfDays;

            return result;
        }

        /// <summary>
        /// Посчитать ещё не потраченные отпускные дни
        /// </summary>
        /// <param name="days">Список отпускных дней</param>
        /// <returns>Количество доступных отпускных дней</returns>
        static public int CountAvailableDays(List<VacationDay> days)
        {
            int result = 0;

            foreach (VacationDay day in days)
                result = result + (day.NumberOfDays - day.UsedDays);

            return result;
        }

        /// <summary>
        /// Получить распределение отпускных дней 
        /// </summary>
        /// <param name="days"></param>
        /// <returns>Словарь "тип отпуска - количество назначенных дней"</returns>
        static private Dictionary<string, int> GetDaysInfo(List<VacationDay> days)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();

            foreach (VacationDay day in days)
            {
                // проверка, есть ли уже запись о таком основании отпуска
                if (result.ContainsKey(day.VacationType.Name))
                {
                    // если уже есть - добавить количество полученных по этому
                    // основанию дней
                    result[day.VacationType.Name] += day.NumberOfDays;
                }
                // если нет записи о таком основании отпуска - создать
                else
                    result.Add(day.VacationType.Name, day.NumberOfDays);
            }

            return result;
        }

        /// <summary>
        /// Добавление основного ежегодного отпуска в 28 дней
        /// </summary>
        /// <param name="empId">Идентификатор сотрудника</param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool AddMainVacationDays(string empId)
        {
            // проверка, есть ли у данного сотрудника дни для основного ежегодного отпуска
            VacationDay mainVacationDays = VacationDayDataHandler.GetEmployeeVacationDays(empId, 1, DateTime.Now.Year);

            // если у сотрудника уже есть такие дни отпуска, то ничего не делать
            if (mainVacationDays != null)
                return true;

            // если таких дней ещё нет, то добавить
            if (VacationDayDataHandler.SetVacationDays(empId, 1,
                    "Автоматическое добавление дней основного ежегодного отпуска",
                    28, DateTime.Now.Year))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Добавление основного ежегодного отпуска в 28 дней заданному списку сотрудников
        /// </summary>
        /// <param name="empIds">Идентификаторы сотрудников</param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool AddMainVacationDays(string[] empIds)
        {
            bool result = true;

            foreach (string empId in empIds)
                result &= AddMainVacationDays(empId);

            return result;
        }
    }
}
