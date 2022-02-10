using System;
using System.Collections.Generic;
using VacationSystem.Models;
using System.IO;
using VacationSystem.ParsingClasses;

namespace VacationSystem.Classes
{
    /// <summary>
    /// Класс для обеспечения соединения с API ТПУ
    /// и получения данных из API
    /// </summary>
    
    static public class Connector
    {
        // постоянная часть ссылки на API
        private const string Url = "JSON/";

        // чтение нужного файла с данными
        public static string ReadReply(string data)
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

        /// <summary>
        /// Парсинг заданных данных
        /// </summary>
        /// <param name="parsingMethod">Метод, подходящий для принятого типа данных</param>
        /// <param name="data">Данные в формате JSON</param>
        /// <returns>Объект, полученный из JSON</returns>
        public static object Parse(Func<string, object> parsingMethod, string data)
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

        /// <summary>
        /// Получение производственного календаря
        /// </summary>
        /// <param name="year">Производственный год</param>
        /// <returns>Список выходных и праздничных дней</returns>
        public static List<Holiday> GetCalendar(string year="")
        {   
            string calendar = ReadReply("calendar" + year);
            List<Holiday> holidays = (List<Holiday>)Parse(Parser.ParseHolidays, calendar);
            if (holidays != null)
                return holidays;
            else
                return null;
        }

        /// <summary>
        /// Получение информации о конкретном сотруднике
        /// </summary>
        /// <param name="id">Идентификатор сотрудника</param>
        /// <returns>Объект с данными о сотруднике</returns>
        static public EmployeeParsed GetEmployee(string id)
        {
            string emp = ReadReply("employees/emp" + id);
            EmployeeParsed employee = (EmployeeParsed)Parse(Parser.ParseEmployee, emp);
            if (employee != null)
                return employee;
            else
                return null;
        }

        /// <summary>
        /// Получение информации о конкретном подразделении
        /// </summary>
        /// <param name="id">Идентификатор подразделения</param>
        /// <returns>Объект с данными о подразделении</returns>
        static public DepartmentParsed GetDepartment(string id)
        {
            string dep = ReadReply("departments/dep" + id);
            DepartmentParsed department = (DepartmentParsed)Parse(Parser.ParseDepartment, dep);
            if (department != null)
                return department;
            else
                return null;
        }

        /// <summary>
        /// Получение списка сотрудников подразделения
        /// </summary>
        /// <param name="id">Идентификатор подразделения</param>
        /// <returns>Список сотрудников подразделения</returns>
        static public List<EmployeeInfo> GetEmployeeList(string id)
        {
            string emps = ReadReply("emp_in_deps/emp_list" + id);
            List<EmployeeInfo> list = (List<EmployeeInfo>)Parse(Parser.ParseEmployeeList, emps);
            if (list != null)
                return list;
            else
                return null;
        }

        /// <summary>
        /// Получение списка должностей в ТПУ
        /// </summary>
        /// <returns>Список должностей</returns>
        static public List<PositionInfo> GetPositionsList()
        {
            string positions = ReadReply("pos_list");
            List<PositionInfo> list = (List<PositionInfo>)Parse(Parser.ParsePositionsList, positions);
            if (list != null)
                return list;
            else
                return null;
        }

        /// <summary>
        /// Получение списка подразделений
        /// </summary>
        /// <returns>Список подразделений</returns>
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