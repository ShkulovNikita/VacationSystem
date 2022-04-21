using System;
using System.Diagnostics;
using VacationSystem.Classes.Data;

namespace VacationSystem.Classes.Helpers
{
    /// <summary>
    /// Класс с различными методами для работы с отпусками сотрудников
    /// </summary>
    static public class VacationHelper
    {
        /// <summary>
        /// Создать объект отпуска из выбранных периодов внутри этого отпуска
        /// </summary>
        /// <param name="startDates">Начальные даты периодов</param>
        /// <param name="endDates">Конечные даты периодов</param>
        /// <returns>Объект с данными об отпуске</returns>
        static public ChosenVacation MakeVacation(DateTime[] startDates, DateTime[] endDates)
        {
            try
            {
                ChosenVacation choosenVacation = new ChosenVacation();

                choosenVacation.Periods = new ChosenPeriod[startDates.Length];

                for (int i = 0; i < startDates.Length; i++)
                {
                    choosenVacation.Periods[i] = new ChosenPeriod
                    {
                        StartDate = startDates[i],
                        EndDate = endDates[i]
                    };
                }

                return choosenVacation;
            }
            catch (IndexOutOfRangeException ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
