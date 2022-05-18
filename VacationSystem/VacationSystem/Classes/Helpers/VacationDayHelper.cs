using System;
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
        /// <param name="year">Год, для которого просматриваются отпускные дни</param>
        /// <returns>Модель представления с информацией о днях отпуска сотрудника</returns>
        static private VacationDaysViewModel MakeViewModel(EmpListItem emp, int year)
        {
            // все отпускные дни сотрудника
            List<VacationDay> vacationDays = VacationDayDataHandler.GetVacationDays(emp.EmpId);

            // отобрать только актуальные дни
            vacationDays = GetCurrentDays(vacationDays, year);

            VacationDaysViewModel daysVm = new VacationDaysViewModel();

            // общее количество выпускных дней
            daysVm.TotalDays = CountTotalDays(vacationDays);

            // общее количество доступных отпускных дней
            daysVm.AvailableDays = CountAvailableDays(vacationDays);

            // получить распределение дней по их типам
            daysVm.SetDays = GetDaysInfo(vacationDays, year);

            // год, на который выданы отпускные дни
            daysVm.Year = year;

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
        /// <returns>Список актуальных дней отпуска</returns>
        static private List<VacationDay> GetCurrentDays(List<VacationDay> days, int year)
        {
            List<VacationDay> result = days.Where(d =>
                d.Year == year
                || ((d.Year == year - 1) && (d.TakenDays < d.NumberOfDays)) )
                .ToList();

            return result;
        }

        /// <summary>
        /// Посчитать количество отпускных дней, которые ещё не были включены ни в один отпуск
        /// </summary>
        /// <param name="days">Список всех дней, назначенных сотруднику</param>
        /// <param name="year">Год, на который назначены дни отпуска</param>
        /// <returns>Количество отпускных дней, которые не были включены в какой-либо утвержденный
        /// либо запланированный отпуск</returns>
        static public int CountFreeDays(List<VacationDay> days, int year)
        {
            int count = 0;
            foreach (VacationDay day in days)
            {
                if ((day.Year == year) || (day.Year == year - 1))
                    count += day.NumberOfDays - day.TakenDays;
            }

            return count;
        }

        /// <summary>
        /// Посчитать количество отпускных дней, которые ещё не были включены ни в один отпуск
        /// </summary>
        /// <param name="empId">Идентификатор сотрудника</param>
        /// <param name="year">Год, на который назначены дни отпуска</param>
        /// <returns>Количество отпускных дней, которые не были включены в какой-либо утвержденный
        /// либо запланированный отпуск</returns>
        static public int CountFreeDays(string empId, int year)
        {
            // получить список отпускных дней данного сотрудника
            List<VacationDay> days = VacationDayDataHandler
                .GetVacationDays(empId)
                .Where(d => d.Year == year || d.Year == year - 1)
                .ToList();

            if (days != null)
            {
                int count = CountFreeDays(days, year);
                return count;
            }
            else
                return -1;
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
                result = result + (day.NumberOfDays - day.TakenDays);

            return result;
        }

        /// <summary>
        /// Посчитать дни, занятие указанным периодом отпуска
        /// </summary>
        /// <param name="vacationId">Идентификатор отпуска</param>
        /// <param name="type">false: запланированный, true: утвержденный</param>
        /// <returns>Количество занятых отпуском отпускных дней</returns>
        static public int CountTakenDays(int vacationId, bool type)
        {
            int result = 0;

            if (type)
            {
                SetVacation vacation = VacationDataHandler.GetSetVacation(vacationId);
                if (vacation == null)
                    return 0;

                int holidays = HolidayHelper.CountHolidays(vacation.StartDate, vacation.EndDate);

                result += (vacation.EndDate - vacation.StartDate).Days + 1 - holidays;
            }
            else
            {
                WishedVacationPeriod vacation = VacationDataHandler.GetWishedVacation(vacationId);
                if (vacation == null)
                    return 0;

                foreach (VacationPart part in vacation.VacationParts)
                {
                    int holidays = HolidayHelper.CountHolidays(part.StartDate, part.EndDate);
                    result += (part.EndDate - part.StartDate).Days + 1 - holidays;
                }
            }

            return result;
        }

        /// <summary>
        /// Получить распределение отпускных дней 
        /// </summary>
        /// <param name="days">Список отпускных дней, назначенных сотруднику</param>
        /// <param name="year">Год, на который нужно просмотреть отпускные дни</param>
        /// <returns>Словарь "тип отпуска - количество назначенных дней"</returns>
        static private Dictionary<string, int> GetDaysInfo(List<VacationDay> days, int year)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();

            days = days.OrderByDescending(d => d.Year).ToList();
            foreach (VacationDay day in days)
            {
                // проверка, принадлежат ли дни отпуска прошлому году
                if (day.Year == year - 1)
                {
                    string previousYearText = "Оставшееся за прошлый год";
                    if (result.ContainsKey(previousYearText))
                        result[previousYearText] += day.NumberOfDays - day.TakenDays;
                    else
                        result.Add(previousYearText, day.NumberOfDays - day.TakenDays);
                }
                // дни принадлежат текущему году
                else
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
            VacationDay mainVacationDaysCurYear = VacationDayDataHandler.GetEmployeeVacationDays(empId, 1, DateTime.Now.Year);
            VacationDay mainVacationDaysNextYear = VacationDayDataHandler.GetEmployeeVacationDays(empId, 1, DateTime.Now.AddYears(1).Year);

            bool resultCurYear;
            // если такие дни отпуска уже есть, то ничего не делать
            if (mainVacationDaysCurYear != null)
                resultCurYear = true;
            else
                resultCurYear = VacationDayDataHandler.SetVacationDays(empId, 1,
                "Автоматическое добавление дней основного ежегодного отпуска",
                28, DateTime.Now.Year);

            bool resultNextYear;
            if (mainVacationDaysNextYear != null)
                resultNextYear = true;
            else
                resultNextYear = VacationDayDataHandler.SetVacationDays(empId, 1,
                "Автоматическое добавление дней основного ежегодного отпуска",
                28, DateTime.Now.AddYears(1).Year);

            return (resultCurYear && resultNextYear);
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
