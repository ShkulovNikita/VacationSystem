using System.Collections.Generic;
using System.Linq;
using VacationSystem.Models;

namespace VacationSystem.Classes.Helpers
{
    /// <summary>
    /// Класс для различных операций с сотрудниками
    /// </summary>
    static public class EmployeesHelper
    {
        /// <summary>
        /// Отфильтровать имеющийся список сотрудников согласно поисковому запросу
        /// </summary>
        /// <param name="employees">Список всех сотрудников</param>
        /// <param name="query">Поисковый запрос</param>
        /// <returns>Список сотрудников, удовлетворяющих запросу</returns>
        static public List<Employee> SearchEmployees(List<Employee> employees, string query)
        {
            return (from emp in employees
                    where emp.FirstName.ToLower().Contains(query.ToLower())
                    || emp.MiddleName.ToLower().Contains(query.ToLower())
                    || emp.LastName.ToLower().Contains(query.ToLower())
                    select emp).ToList();
        }
    }
}
