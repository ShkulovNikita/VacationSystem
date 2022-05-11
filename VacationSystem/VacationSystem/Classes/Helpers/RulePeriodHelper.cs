using System.Collections.Generic;
using System;

namespace VacationSystem.Classes.Helpers
{
    /// <summary>
    /// Отображение текста описания периода на основе
    /// двух выбранных граничных дат
    /// </summary>
    static public class RulePeriodHelper
    {
        static private Dictionary<int, string> OneMonthNames = new Dictionary<int, string>
        {
            { 1, "Январь" },
            { 2, "Февраль" },
            { 3, "Март" },
            { 4, "Апрель" },
            { 5, "Май" },
            { 6, "Июнь" },
            { 7, "Июль" },
            { 8, "Август" },
            { 9, "Сентябрь" },
            { 10, "Октябрь" },
            { 11, "Ноябрь" },
            { 12, "Декабрь" }
        };

        static private Dictionary<int, string> PeriodMonthNames = new Dictionary<int, string>
        {
            { 1, "Января" },
            { 2, "Февраля" },
            { 3, "Марта" },
            { 4, "Апреля" },
            { 5, "Мая" },
            { 6, "Июня" },
            { 7, "Июля" },
            { 8, "Августа" },
            { 9, "Сентября" },
            { 10, "Октября" },
            { 11, "Ноября" },
            { 12, "Декабря" }
        };

        static public string GetPeriodText(DateTime startDate, DateTime endDate)
        {
            // период - весь год
            if (startDate == new DateTime(startDate.Year, 1, 1) && endDate == new DateTime(endDate.Year, 12, 31))
                return "Весь год";

            // период - один месяц
            if (startDate.Month == endDate.Month && startDate.Day == 1 && endDate.Day == DateTime.DaysInMonth(endDate.Year, endDate.Month))
                return OneMonthNames[startDate.Month];

            // случайный период
            return startDate.Day.ToString() + " " + PeriodMonthNames[startDate.Month] + " - " + endDate.Day.ToString() + " " + PeriodMonthNames[endDate.Month];
        }
    }
}