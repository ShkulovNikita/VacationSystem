using System;
using System.Diagnostics;
using System.Collections.Generic;
using VacationSystem.Models;
using VacationSystem.Classes.Database;
using VacationSystem.Classes.Helpers;

namespace VacationSystem.Classes
{
    /// <summary>
    /// Класс с методами, отвечающими за ежедневное обновление данных в системе
    /// </summary>
    static public class Updater
    {
        static public void Update()
        {
            try
            {
                // текущая дата
                DateTime today = DateTime.Now;

                // проверить, была ли уже сегодня проведена проверка
                bool wasUpdated = UpdateDataHandler.HasDate(today);

                if (!wasUpdated)
                {
                    // получить список утвержденных отпусков сотрудников
                    List<SetVacation> vacations = VacationDataHandler.GetActiveSetVacations(today.Year);

                    if (vacations != null)
                        // найти среди них тех, которые скоро начинаются или заканчиваются
                        SendNotifications(today, vacations, 7);
                }

                UpdateDataHandler.AddUpdate(today);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Проверка близости начала или окончания отпусков с отправкой соответствующего уведомления
        /// </summary>
        /// <param name="date">Дата, для которой проводится сравнение</param>
        /// <param name="vacations">Список всех текущих утвержденных отпусков</param>
        /// <param name="number">Количество дней, определяющее близость события</param>
        static private void SendNotifications(DateTime date, List<SetVacation> vacations, int number)
        {
            try
            {
                foreach(SetVacation vacation in vacations)
                {
                    // скоро сотрудник уйдет в отпуск
                    if ((vacation.StartDate - date).Days == number)
                        NotificationHelper.EmployeeLeave(vacation.EmployeeId, vacation.Id, number);
                    // скоро выйдет из отпуска
                    if ((vacation.EndDate - date).Days == number)
                        NotificationHelper.EmployeeReturn(vacation.EmployeeId, vacation.Id, number);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
