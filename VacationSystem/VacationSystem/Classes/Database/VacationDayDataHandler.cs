using System;
using System.Diagnostics;
using System.Collections.Generic;
using VacationSystem.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace VacationSystem.Classes.Database
{
    /// <summary>
    /// Класс с методами БД для работы с днями отпуска, назначенными сотрудникам
    /// </summary>
    static public class VacationDayDataHandler
    {
        /// <summary>
        /// Получение всех дней отпуска, назначенных сотруднику
        /// </summary>
        /// <param name="id">Идентификатор сотрудника</param>
        /// <returns>Список дней отпуска сотрудника</returns>
        static public List<VacationDay> GetVacationDays(string id)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.VacationDays
                        .Include(vd => vd.VacationType)
                        .Where(vd => vd.EmployeeId == id)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
