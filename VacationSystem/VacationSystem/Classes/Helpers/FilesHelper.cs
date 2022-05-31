using VacationSystem.Classes.CalendarClasses;
using VacationSystem.Classes.Database;
using VacationSystem.Models;

using System.Collections.Generic;
using System.Text.Json;
using System.Diagnostics;
using System;
using System.IO;
using System.Text.Encodings.Web;

namespace VacationSystem.Classes.Helpers
{
    /// <summary>
    /// Класс для создания файлов с календарем отпусков сотрудников
    /// </summary>
    static public class FilesHelper
    {
        static readonly JsonSerializerOptions opt = new JsonSerializerOptions
        {
            //Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
        };

        static public bool CreateJsonFile(Department department, int year, string path)
        {
            try
            {
                CalendarData calendar = new CalendarData();
                List<CalendarEmployee> calEmps = new List<CalendarEmployee>();

                // список всех сотрудников подразделения
                List<Employee> emps = Connector.GetEmployeesOfDepartment(department.Id);

                // проход по всем сотрудникам с получением их отпусков
                foreach (Employee emp in emps)
                {
                    // отпуска сотрудника
                    List<SetVacation> vacations = VacationDataHandler.GetSetVacations(emp.Id, year);
                    
                    if (vacations == null)
                        continue;
                    if (vacations.Count == 0)
                        continue;

                    List<VacationPeriod> periods = new List<VacationPeriod>();
                    foreach (SetVacation vacation in vacations)
                    {
                        periods.Add(new VacationPeriod
                        {
                            StartDate = vacation.StartDate,
                            EndDate = vacation.EndDate
                        });
                    }

                    string empName = "";
                    if (emp.MiddleName != null)
                        empName = emp.LastName + " " + emp.FirstName + " " + emp.MiddleName;
                    else
                        empName = emp.LastName + " " + emp.FirstName;

                    CalendarEmployee calendarEmployee = new CalendarEmployee
                    {
                        Id = emp.Id,
                        Name = empName,
                        VacationPeriods = periods.ToArray()
                    };
                    calEmps.Add(calendarEmployee);
                }

                calendar.DepartmentId = department.Id;
                calendar.DepartmentName = department.Name;
                calendar.Year = year;
                calendar.Employees = calEmps.ToArray();

                string json = JsonSerializer.Serialize<CalendarData>(calendar, opt);

                FileStream stream = new FileStream(path, FileMode.CreateNew);
                using (StreamWriter sw = new StreamWriter(stream, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine(json);
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
