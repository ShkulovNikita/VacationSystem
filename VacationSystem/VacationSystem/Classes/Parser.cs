using System.Collections.Generic;
using VacationSystem.Models;
using System.Text.Json;
using System;
using VacationSystem.ParsingClasses;

namespace VacationSystem.Classes
{
    public class Parser
    {
        static public List<Holiday> ParseHolidays(string json)
        {
            try
            {
                CalendarHoliday data = JsonSerializer.Deserialize<CalendarHoliday>(json);

                // массив с праздниками по календарю
                List<Holiday> holidays = new List<Holiday>();

                // получение из JSON всех периодов праздников
                foreach (HolidayPeriod period in data.Holidays)
                {
                    Holiday hlday = new Holiday();

                    if (period.Name != "null")
                        hlday.Name = period.Name;
                    else
                        hlday.Name = null;

                    hlday.StartDate = DateTime.Parse(period.StartDate);
                    hlday.EndDate = DateTime.Parse(period.EndDate);

                    holidays.Add(hlday);
                }

                return holidays;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        static public EmployeeParsed ParseEmployee(string json) 
        {
            try
            {
                EmployeeParsed data = JsonSerializer.Deserialize<EmployeeParsed>(json);
                return data;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        static public DepartmentParsed ParseDepartment(string json)
        {
            try
            {
                DepartmentParsed data = JsonSerializer.Deserialize<DepartmentParsed>(json);
                return data;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        static public List<EmployeeInfo> ParseEmployeeList(string json)
        {
            try
            {
                EmployeeList data = JsonSerializer.Deserialize<EmployeeList>(json);

                // получение списка сотрудников из ответа
                List<EmployeeInfo> list = new List<EmployeeInfo>();

                foreach (EmployeeInfo emp in data.Employees)
                {
                    list.Add(new EmployeeInfo
                    {
                        Id = emp.Id,
                        Position = emp.Position,
                        Head = emp.Head
                    });
                }

                if (list.Count > 0)
                    return list;
                else
                    return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        static public List<PositionInfo> ParsePositionsList(string json)
        {
            try
            {
                PositionsList data = JsonSerializer.Deserialize<PositionsList>(json);

                // получение списка должностей из ответа
                List<PositionInfo> list = new List<PositionInfo>();

                foreach(PositionInfo position in data.Positions)
                {
                    list.Add(new PositionInfo
                    {
                        Id = position.Id,
                        Name = position.Name
                    });
                }

                if (list.Count > 0)
                    return list;
                else
                    return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        static public List<DepartmentInfo> ParseDepartmentsList(string json)
        {
            try
            {
                DepartmentsList data = JsonSerializer.Deserialize<DepartmentsList>(json);

                // получение списка отделений из ответа
                List<DepartmentInfo> list = new List<DepartmentInfo>();

                foreach(DepartmentInfo dep in data.Departments)
                {
                    list.Add(new DepartmentInfo
                    {
                        Id = dep.Id,
                        Name = dep.Name,
                        Head = dep.Head
                    });
                }

                if (list.Count > 0)
                    return list;
                else
                    return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}