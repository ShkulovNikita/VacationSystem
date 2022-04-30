using System;
using System.Collections.Generic;

namespace VacationSystem.Classes.Helpers
{
    /// <summary>
    /// Класс с методами для работы с датами
    /// </summary>
    static public class DateHelper
    {
        /// <summary>
        /// Получить все дни между двумя указанными датами
        /// </summary>
        /// <param name="start">Начальная граница периода</param>
        /// <param name="end">Конечная граница периода</param>
        /// <returns>Список дат указанного периода</returns>
        static public List<DateTime> GetDateRange(DateTime start, DateTime end)
        {
            List<DateTime> dates = new List<DateTime>();

            for (DateTime dt = start; dt <= end; dt = dt.AddDays(1))
            {
                dates.Add(dt);
            }

            return dates;
        }

        /// <summary>
        /// Получить все дни указанного года
        /// </summary>
        /// <param name="year">Год</param>
        /// <returns>Список дней указанного года</returns>
        static public List<DateTime> GetYearDates(int year)
        {
            DateTime start = new DateTime(year, 1, 1);
            DateTime end = new DateTime(year, 12, 31);

            List<DateTime> result = GetDateRange(start, end);

            return result;
        }

        /// <summary>
        /// Проверка корректности даты при заданном году
        /// </summary>
        /// <param name="date">Дата для проверки</param>
        /// <param name="year">Год</param>
        /// <param name="type">Тип: false - начальная, true - конечная</param>
        /// <returns>Дата в исправленном (при необходимости) виде</returns>
        static public DateTime CheckDate(DateTime? date, int year, bool type)
        {
            // дата не задана
            if (date == null)
            {
                // дата является конечной
                if (type)
                    return new DateTime(year, 12, 31);
                // начальной
                else
                    return new DateTime(year, 1, 1);
            }
            else
            {
                // дата за пределами года
                if ((date < new DateTime(year, 1, 1)) || (date > new DateTime(year, 12, 31)))
                {
                    if (type)
                        return new DateTime(year, 12, 31);
                    else
                        return new DateTime(year, 1, 1);
                }
                else
                    return (DateTime)date;
            }
        }
    }
}
