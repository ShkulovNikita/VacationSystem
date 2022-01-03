using System;
using System.Collections.Generic;
using VacationSystem.Models;
using VacationSystem.Models.Parsering;
using System.IO;

namespace VacationSystem.Classes
{
    /// <summary>
    /// класс для обеспечения соединения с API ТПУ
    /// и получения данных из API
    /// </summary>
    public class Connector
    {
        // постоянная часть ссылки на API
        private const string Url = "JSON/";

        // чтение нужного файла с данными
        static public string ReadReply(string data)
        {
            string path = Url + data;

            path += ".json";

            string text = "";

            try
            {
                using (StreamReader sr = new StreamReader(path, System.Text.Encoding.UTF8))
                    text = sr.ReadToEnd();

                if (text != "")
                    return text;
                else
                    return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        // парсинг заданных данных
        static public object Parse(Func<string, object> parsingMethod, string data)
        {
            if (data != null)
            {
                try
                {
                    return parsingMethod(data);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
            }
            else
                return null;
        }

        // получение производственного календаря
        static public List<Holiday> GetCalendar(string year="")
        {   
            string calendar = ReadReply("calendar" + year);
            List<Holiday> holidays = (List<Holiday>)Parse(Parser.ParseHolidays, calendar);
            if (holidays != null)
                return holidays;
            else
                return null;
        }

        // получение информации о конкретном сотруднике
        static public EmployeeParsed GetEmployee(string id)
        {
            string emp = ReadReply("emp" + id);
            EmployeeParsed employee = (EmployeeParsed)Parse(Parser.ParseEmployee, emp);
            if (employee != null)
                return employee;
            else
                return null;
        }

        // получение информации о конкретном отделении
        static public DepartmentParsed GetDepartment(string id)
        {
            string dep = ReadReply("dep" + id);
            DepartmentParsed department = (DepartmentParsed)Parse(Parser.ParseDepartment, dep);
            if (department != null)
                return department;
            else
                return null;
        }

        // получение списка сотрудников отделения
        static public List<EmployeeBrief> GetEmployeeList(string id)
        {
            string emps = ReadReply("emp_list" + id);
            List<EmployeeBrief> list = (List<EmployeeBrief>)Parse(Parser.ParseEmployeeList, emps);
            if (list != null)
                return list;
            else
                return null;
        }

        // получение списка должностей
        static public List<PositionInfo> GetPositionsList()
        {
            string positions = ReadReply("pos_list");
            List<PositionInfo> list = (List<PositionInfo>)Parse(Parser.ParsePositionsList, positions);
            if (list != null)
                return list;
            else
                return null;
        }

        // получение списка отделений
        static public List<DepartmentInfo> GetDepartmentsList()
        {
            string departments = ReadReply("dep_list");
            List<DepartmentInfo> list = (List<DepartmentInfo>)Parse(Parser.ParseDepartmentsList, departments);
            if (list != null)
                return list;
            else
                return null;
        }
    }
}