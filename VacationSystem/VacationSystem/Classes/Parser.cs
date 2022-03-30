using VacationSystem.Models;
using VacationSystem.ApiClasses;
using System.Collections.Generic;
using System.Text.Json;
using System.Diagnostics;
using System;

namespace VacationSystem.Classes
{
    /// <summary>
    /// Класс, отвечающий за парсинг сообщений в формате JSON
    /// во внутренние классы программы
    /// </summary>
    static public class Parser
    {
        /// <summary>
        /// Парсинг ответа от API с производственным календарем
        /// </summary>
        /// <param name="json">Строка JSON, полученная от API</param>
        /// <returns>Производственный календарь</returns>
        static public Holiday ParseCalendar(string json)
        {
            try
            {
                HolidaysList holidays = JsonSerializer.Deserialize<HolidaysList>(json);
                if (holidays == null)
                    return null;

                if (holidays.Holidays == null)
                    return new Holiday();

                List<DateTime> dates = new List<DateTime>();
                foreach (DateTime date in holidays.Holidays)
                    dates.Add(date);

                Holiday result = new Holiday
                {
                    Dates = dates
                };

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Парсинг ответа от API с информацией о сотруднике
        /// </summary>
        /// <param name="json">Строка JSON, полученная от API</param>
        /// <returns>Сотрудник</returns>
        static public Employee ParseEmployee(string json)
        {
            try
            {
                Employee employee = JsonSerializer.Deserialize<Employee>(json);
                return employee;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Парсинг ответа от API со списком сотрудников
        /// </summary>
        /// <param name="json">Строка JSON, полученная от API</param>
        /// <returns>Список сотрудников</returns>
        static public List<Employee> ParseEmployees(string json)
        {
            try
            {
                EmployeesList list = JsonSerializer.Deserialize<EmployeesList>(json);
                if (list == null)
                    return null;

                // проверка списка на пустоту
                if (list.Employees == null)
                    return new List<Employee>();

                return new List<Employee>(list.Employees);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Парсинг ответа от API с данными о подразделении
        /// </summary>
        /// <param name="json">Строка JSON, полученная от API</param>
        /// <returns>Подразделение</returns>
        static public Department ParseDepartment(string json)
        {
            try
            {
                Department dep = JsonSerializer.Deserialize<Department>(json);
                return dep;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Парсинг ответа от API со списком подразделений
        /// </summary>
        /// <param name="json">Строка JSON, полученная от API</param>
        /// <returns>Список подразделений</returns>
        static public List<Department> ParseDepartments(string json)
        {
            try
            {
                DepartmentsList list = JsonSerializer.Deserialize<DepartmentsList>(json);
                if (list == null)
                    return null;

                if (list.Departments == null)
                    return new List<Department>();

                return new List<Department>(list.Departments);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Парсинг ответа от API с данными о должности
        /// </summary>
        /// <param name="json">Строка JSON, полученная от API</param>
        /// <returns>Должность</returns>
        static public Position ParsePosition(string json)
        {
            try
            {
                Position position = JsonSerializer.Deserialize<Position>(json);
                return position;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Парсинг ответа от API со списком должностей
        /// </summary>
        /// <param name="json">Строка JSON, полученная от API</param>
        /// <returns>Список должностей</returns>
        static public List<Position> ParsePositions(string json)
        {
            try
            {
                PositionsList list = JsonSerializer.Deserialize<PositionsList>(json);
                if (list == null)
                    return null;

                if (list.Positions == null)
                    return new List<Position>();

                return new List<Position>(list.Positions);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Парсинг ответа от API со списком должностей в подразделениях
        /// </summary>
        /// <param name="json">Строка JSON, полученная от API</param>
        /// <returns>Список должностей в подразделениях</returns>
        static public List<PositionInDepartment> ParsePositionsInDepartments(string json)
        {
            try
            {
                PositionsInDepartments list = JsonSerializer.Deserialize<PositionsInDepartments>(json);
                if (list == null)
                    return null;

                if (list.Positions == null)
                    return new List<PositionInDepartment>();

                return new List<PositionInDepartment>(list.Positions);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }
    }
}