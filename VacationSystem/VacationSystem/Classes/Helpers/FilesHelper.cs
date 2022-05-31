using VacationSystem.Classes.CalendarClasses;
using VacationSystem.Classes.Database;
using VacationSystem.Models;

using System.Collections.Generic;
using System.Text.Json;
using System.Diagnostics;
using System;
using System.IO;
using System.Text.Encodings.Web;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

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

        static public bool CreateExcelFile(Department department, int year, string path)
        {
            try
            {
                // получить всех сотрудников
                List<Employee> emps = Connector.GetEmployeesOfDepartment(department.Id);

                // получить все отпуска сотрудников
                foreach (Employee emp in emps)
                {
                    List<SetVacation> vacations = VacationDataHandler.GetSetVacations(emp.Id, year);

                    if (vacations == null)
                    {
                        emp.SetVacations = new List<SetVacation>();
                    }
                    else
                        emp.SetVacations = vacations;
                }

                // все даты указанного года
                List<Month> months = FillMonths(year);

                // создание файла

                // новая книга Эксель
                var workbook = new XSSFWorkbook();

                // лист с графиком
                var sheet = workbook.CreateSheet("Отпуска");

                // первая строка с заголовками
                var firstRow = sheet.CreateRow(0);
                // вторая строка с заголовками
                var secondRow = sheet.CreateRow(1);

                // первая ячейка первой строки - "сотрудник"
                firstRow.CreateCell(0);
                secondRow.CreateCell(0);

                // сдвиг из-за количества уже использованных ячеек
                int shift = 1;

                // стиль для центрирования текста
                var cellStyleCenter = workbook.CreateCellStyle();
                cellStyleCenter.BorderLeft = BorderStyle.Thick;
                cellStyleCenter.BorderRight = BorderStyle.Thick;
                cellStyleCenter.Alignment = HorizontalAlignment.Center;
                cellStyleCenter.VerticalAlignment = VerticalAlignment.Center;

                // стиль для границы справа
                var rightBorder = workbook.CreateCellStyle();
                rightBorder.BorderRight = BorderStyle.Thick;

                // стиль для границы снизу
                var bottomBorder = workbook.CreateCellStyle();
                bottomBorder.BorderBottom = BorderStyle.Thick;

                // снизу и слева
                var borderBottomLeft = workbook.CreateCellStyle();
                borderBottomLeft.BorderBottom = BorderStyle.Thick;
                borderBottomLeft.BorderLeft = BorderStyle.Thick;

                // снизу и справа
                var borderBottomRight = workbook.CreateCellStyle();
                borderBottomRight.BorderRight = BorderStyle.Thick;
                borderBottomRight.BorderBottom = BorderStyle.Thick;

                // сотрудник
                var employeeStyle = workbook.CreateCellStyle();
                employeeStyle.BorderBottom = BorderStyle.Thick;
                employeeStyle.BorderRight = BorderStyle.Thick;
                employeeStyle.VerticalAlignment = VerticalAlignment.Center;

                var thinBorderStyle = workbook.CreateCellStyle();
                thinBorderStyle.BorderBottom = BorderStyle.Thin;

                var thinBorderRightStyle = workbook.CreateCellStyle();
                thinBorderRightStyle.BorderBottom = BorderStyle.Thin;
                thinBorderRightStyle.BorderRight = BorderStyle.Thick;

                // стиль для ячейки отпуска
                var coloredStyle = workbook.CreateCellStyle();
                coloredStyle.FillPattern = FillPattern.SolidForeground;
                coloredStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.BlueGrey.Index;
                coloredStyle.BorderBottom = BorderStyle.Thin;

                var coloredBorderStyle = workbook.CreateCellStyle();
                coloredBorderStyle.FillPattern = FillPattern.SolidForeground;
                coloredBorderStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.BlueGrey.Index;
                coloredBorderStyle.BorderBottom = BorderStyle.Thin;
                coloredBorderStyle.BorderRight = BorderStyle.Thick;

                // создать ячейки для последующих месяцев
                foreach (Month month in months)
                {
                    for (int i = 0; i < month.Days; i++)
                    {
                        firstRow.CreateCell(i + shift);
                        secondRow.CreateCell(i + shift);

                        // указать номер дня
                        var cellDayNumb = sheet.GetRow(1).GetCell(i + shift);
                        cellDayNumb.SetCellType(CellType.String);
                        cellDayNumb.SetCellValue(i + 1);

                        if (i == 0)
                            cellDayNumb.CellStyle = borderBottomLeft;
                        else if (i == month.Days - 1)
                            cellDayNumb.CellStyle = borderBottomRight;
                        else
                            cellDayNumb.CellStyle = bottomBorder;
                    }

                    // объединить созданные ячейки
                    var cra = new CellRangeAddress(0, 0, 0 + shift, month.Days + shift - 1);
                    sheet.AddMergedRegion(cra);

                    var cell = sheet.GetRow(0).GetCell(0 + shift);
                    cell.SetCellType(CellType.String);
                    cell.SetCellValue(month.Name);
                    cell.CellStyle = cellStyleCenter;

                    shift += month.Days;

                    GC.Collect();
                }

                var craEmp = new CellRangeAddress(0, 1, 0, 0);
                sheet.AddMergedRegion(craEmp);

                var empCell = sheet.GetRow(0).GetCell(0);
                empCell.SetCellType(CellType.String);
                empCell.SetCellValue("Сотрудник");
                empCell.CellStyle = employeeStyle;

                // добавить сотрудников с их фамилиями и отпусками
                
                // проход по сотрудникам
                for (int i = 0; i < emps.Count; i++)
                {
                    // создание строки для очередного сотрудника
                    var nRow = sheet.CreateRow(i + 2);

                    // ячейка с ФИО сотрудника
                    nRow.CreateCell(0);

                    var fioCell = sheet.GetRow(i + 2).GetCell(0);
                    fioCell.SetCellType(CellType.String);
                    fioCell.SetCellValue(EmployeeHelper.GetFullName(emps[i]));
                    fioCell.CellStyle = rightBorder;

                    List<bool> vacations = GetVacations(emps[i], year);

                    // перебор от первого до последнего дня года
                    for (int j = 0; j < vacations.Count; j++)
                    {
                        nRow.CreateCell(j + 1);
                        // входит в отпуск
                        if (vacations[j])
                        {
                            var coloredCell = sheet.GetRow(i + 2).GetCell(j + 1);
                            if (j != vacations.Count - 1)
                                coloredCell.CellStyle = coloredStyle;
                            else
                                coloredCell.CellStyle = coloredBorderStyle;
                        }
                        else
                        {
                            var anotherCell = sheet.GetRow(i + 2).GetCell(j + 1);
                            if (j != vacations.Count - 1)
                                anotherCell.CellStyle = thinBorderStyle;
                            else
                                anotherCell.CellStyle = thinBorderRightStyle;
                        }
                    }

                    if (i == emps.Count - 1)
                    {
                        var firstBottomCell = sheet.GetRow(i + 2).GetCell(0);
                        firstBottomCell.CellStyle = borderBottomRight;

                        for (int j = 1; j < sheet.GetRow(i + 2).LastCellNum; j++)
                        {
                            var bottomCell = sheet.GetRow(i + 2).GetCell(j);
                            if (j != sheet.GetRow(i + 2).LastCellNum - 1)
                                bottomCell.CellStyle = bottomBorder;
                            else
                                bottomCell.CellStyle = borderBottomRight;
                        }
                    }

                    GC.Collect();
                }

                shift = 1;
                // все месяцы
                foreach (Month month in months)
                {
                    // все дни месяца
                    for (int i = 0; i < month.Days; i++)
                    {
                        if (i == month.Days - 1)
                        {
                            // все сотрудники
                            for (int j = 0; j < emps.Count; j++)
                            {
                                var edgeCell = sheet.GetRow(j + 2).GetCell(month.Days + shift - 1);

                                if (edgeCell.CellStyle.FillForegroundColor == NPOI.HSSF.Util.HSSFColor.BlueGrey.Index)
                                    edgeCell.CellStyle = coloredBorderStyle;
                                else if (j != emps.Count - 1)
                                    edgeCell.CellStyle = thinBorderRightStyle;
                                else
                                    edgeCell.CellStyle = borderBottomRight;
                            }
                        }
                    }

                    shift += month.Days;
                }

                // поменять ширину столбцов
                int yearDays = 0;
                foreach (Month month in months)
                    yearDays += month.Days;

                sheet.AutoSizeColumn(0);
                for (int i = 1; i < yearDays + 1; i++)
                    sheet.SetColumnWidth(i, 750);

                GC.Collect();

                var empTitleCell = sheet.GetRow(0).GetCell(0);
                empTitleCell.CellStyle = borderBottomRight;

                // сохранить Excel-файл
                using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    workbook.Write(stream);
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        static private List<Month> FillMonths(int year)
        {
            List<Month> result = new List<Month>();

            List<string> names = new List<string>
            {
                "Январь",
                "Февраль",
                "Март",
                "Апрель",
                "Май",
                "Июнь",
                "Июль",
                "Август",
                "Сентябрь",
                "Октябрь",
                "Ноябрь",
                "Декабрь"
            };

            for (int i = 0; i < names.Count; i++)
            {
                Month month = new Month();
                month.Name = names[i];
                month.Days = DateTime.DaysInMonth(year, i + 1);
                result.Add(month);
            }

            return result;
        }

        static private List<bool> GetVacations(Employee emp, int year)
        {
            List<bool> result = new List<bool>();
            List<DateTime> days = DateHelper.GetYearDates(year);

            for (int i = 0; i < days.Count; i++)
            {
                if (IsAmongVacation(emp, days[i]))
                    result.Add(true);
                else
                    result.Add(false);
            }

            return result;
        }

        static private bool IsAmongVacation(Employee emp, DateTime date)
        {
            foreach (SetVacation setVacation in emp.SetVacations)
                if ((date >= setVacation.StartDate) && (date <= setVacation.EndDate))
                    return true;

            return false;
        }
    }

    /// <summary>
    /// Вспомогательный класс для хранения данных о днях
    /// </summary>
    class Month
    {
        public Month () { }

        public string Name { get; set; }

        public int Days { get; set; }
    }
}
