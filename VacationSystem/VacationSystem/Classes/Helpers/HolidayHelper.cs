using System;
using System.Linq;
using System.Collections.Generic;
using VacationSystem.Classes;
using VacationSystem.Models;

namespace VacationSystem.Classes.Helpers
{
    /// <summary>
    /// Статический класс с методами для нахождения праздничных дней в производственном календаре
    /// </summary>
    static public class HolidayHelper
    {
        /// <summary>
        /// Найти среди указанных дат только те, которые не являются выходными
        /// </summary>
        /// <param name="startDate">Начальная граница периода</param>
        /// <param name="endDate">Конечная граница периода</param>
        /// <returns>Список дат, являющихся праздничными днями</returns>
        static public List<DateTime> FindHolidays(DateTime startDate, DateTime endDate)
        {
            Holiday calendar = Connector.GetCalendar(startDate.Year);
            if (calendar == null)
                return null;

            // отфильтровать по границам
            calendar.Dates = calendar.Dates
                .Where(d => d >= startDate && d <= endDate)
                .OrderBy(d => d)
                .ToList();

            List<DateTime> result = new List<DateTime>();

            foreach (var date in calendar.Dates)
                if ((date.DayOfWeek != DayOfWeek.Saturday) && (date.DayOfWeek != DayOfWeek.Sunday))
                    result.Add(date);

            return result;
        }

        /// <summary>
        /// Рассчитать, сколько есть праздничных дней между указанными датами
        /// </summary>
        /// <param name="startDate">Начальная граница периода</param>
        /// <param name="endDate">Конечная граница периода</param>
        /// <returns>Количество праздничных дней</returns>
        static public int CountHolidays(DateTime startDate, DateTime endDate)
        {
            List<DateTime> holidays = FindHolidays(startDate, endDate);
            if (holidays == null)
                return 0;
            return holidays.Count();
        }

        /// <summary>
        /// Получить тип дня согласно производственному календарю
        /// </summary>
        /// <param name="date">Проверяемая дата</param>
        /// <returns>
        /// b - будний день;
        /// h - праздничный;
        /// v - выходной.
        /// </returns>
        static public string GetDayType(DateTime date)
        {
            DateTime day = Connector.GetCalendar(date.Year).Dates.FirstOrDefault(d => d == date);
            if (day == DateTime.MinValue)
                return "b";
            else
            {
                if ((day.DayOfWeek != DayOfWeek.Saturday) && (day.DayOfWeek != DayOfWeek.Sunday))
                    return "h";
                else
                    return "v";
            }
        }
    }
}
