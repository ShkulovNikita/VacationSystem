using System.Collections.Generic;
using VacationSystem.Models;
using VacationSystem.Models.Parsering;
using System.Text.Json;
using System;

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

        static public EmployeeParsed ParseEmployee (string json) 
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
    }
}