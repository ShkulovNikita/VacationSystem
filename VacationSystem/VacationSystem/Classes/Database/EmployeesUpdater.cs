using System;
using System.Linq;
using System.Collections.Generic;
using VacationSystem.Models;

namespace VacationSystem.Classes.Database
{
    /// <summary>
    /// Класс для загрузки и обновления данных в БД о
    /// сотрудниках, получаемых из API
    /// </summary>
    static public class EmployeesUpdater
    {
        /// <summary>
        /// Загрузить из API данные о сотрудниках
        /// </summary>
        /// <returns>Успешность выполнения операции</returns>
        static public bool LoadEmployees()
        {
            if (DataHandler.GetEmployeesCount() == 0)
                return FillEmployees();
            else
                return UpdateEmployees();
        }

        /// <summary>
        /// Загрузить данные о сотрудниках в БД из API
        /// </summary>
        /// <returns>Успешность выполнения операции</returns>
        static private bool FillEmployees()
        {
            try
            {
                using(ApplicationContext db = new ApplicationContext())
                {
                    // получить список идентификаторов подразделений в БД
                    List<string> depIds = DataHandler.GetDepartments(db).Select(d => d.Id).ToList();
                    if (depIds == null)
                        return false;
                    if (depIds.Count == 0)
                        return false;

                    // получить сотрудников всех подразделений
                    List<Employee> employees = GetEmployees(depIds);
                    if (employees.Count == 0)
                        return false;

                    // добавить сотрудников в БД
                    db.Employees.AddRange(employees);
                    db.SaveChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Получение списка сотрудников из API
        /// </summary>
        /// <param name="depIds">Список идентификаторов подразделений в БД</param>
        /// <returns>Список сотрудников из API</returns>
        static private List<Employee> GetEmployees(List<string> depIds)
        {
            List<Employee> result = new List<Employee>();

            // проход по всем подразделениям
            // с целью получения их сотрудников
            foreach(string depId in depIds)
            {
                // сотрудники подразделения
                List<Employee> employees = ModelConverter.ConvertToEmployees(Connector.GetParsedEmployeeList(depId));
                if (employees != null)
                {
                    // добавить тех сотрудников, которых ещё нет в списке result
                    List<Employee> newEmps = employees
                        .Where(e => !result.Any(emp => e.Id == emp.Id))
                        .ToList();
                    result.AddRange(newEmps);
                }
            }

            return result;
        }

        /// <summary>
        /// Обновить имеющиеся в БД данные о сотрудниках
        /// </summary>
        /// <returns>Успешность выполнения операции</returns>
        static private bool UpdateEmployees()
        {
            return true;
        }
    }
}
