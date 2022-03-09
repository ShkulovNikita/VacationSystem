using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using VacationSystem.Models;

namespace VacationSystem.Classes.Database
{
    /// <summary>
    /// Временный класс для работы с данными, получаемыми из API
    /// </summary>
    static public class ApiDataHandler
    {
        /// <summary>
        /// Добавление в таблицу сотрудников идентификаторов, полученных из API
        /// </summary>
        /// <param name="employees">Сотрудники из API</param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool LoadEmployees(List<Employee> employees)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    // все сотрудники в БД
                    List<Employee> empsInDb = db.Employees.ToList();

                    // сотрудники из API, которых нет в БД
                    List<Employee> newEmps = employees
                        .Where(e => !empsInDb.Any(emp => emp.Id == e.Id))
                        .ToList();

                    if (newEmps.Count > 0)
                    {
                        db.Employees.AddRange(newEmps);
                        db.SaveChanges();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
