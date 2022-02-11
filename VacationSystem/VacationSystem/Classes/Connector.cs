using System;
using System.Collections.Generic;
using VacationSystem.Models;
using System.IO;
using VacationSystem.ParsingClasses;
using VacationSystem.ProgramClasses;

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

        /// <summary>
        /// Чтение файла с данными от API
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
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
        public static List<Holiday> GetParsedCalendar(string year="")
        {   
            string calendar = ReadReply("calendar" + year);
            List<Holiday> holidays = (List<Holiday>)Parse(Parser.ParseHolidays, calendar);
            if (holidays != null)
                return holidays;
            else
                return null;
        }

        /// <summary>
        /// Получение информации о конкретном сотруднике (формат API)
        /// </summary>
        /// <param name="id">Идентификатор сотрудника</param>
        /// <returns>Объект с данными о сотруднике</returns>
        static public EmployeeParsed GetParsedEmployee(string id)
        {
            string emp = ReadReply("employees/emp" + id);
            EmployeeParsed employee = (EmployeeParsed)Parse(Parser.ParseEmployee, emp);
            if (employee != null)
                return employee;
            else
                return null;
        }

        /// <summary>
        /// Получение информации о конкретном подразделении (формат API)
        /// </summary>
        /// <param name="id">Идентификатор подразделения</param>
        /// <returns>Объект с данными о подразделении</returns>
        static public DepartmentParsed GetParsedDepartment(string id)
        {
            string dep = ReadReply("departments/dep" + id);
            DepartmentParsed department = (DepartmentParsed)Parse(Parser.ParseDepartment, dep);
            if (department != null)
                return department;
            else
                return null;
        }

        /// <summary>
        /// Получение списка сотрудников подразделения (формат API)
        /// </summary>
        /// <param name="id">Идентификатор подразделения</param>
        /// <returns>Список сотрудников подразделения</returns>
        static public List<EmployeeInfo> GetParsedEmployeeList(string id)
        {
            string emps = ReadReply("emp_in_deps/emp_list" + id);
            List<EmployeeInfo> list = (List<EmployeeInfo>)Parse(Parser.ParseEmployeeList, emps);
            if (list != null)
                return list;
            else
                return null;
        }

        /// <summary>
        /// Получение списка должностей в ТПУ (формат API)
        /// </summary>
        /// <returns>Список должностей</returns>
        static public List<PositionInfo> GetParsedPositionsList()
        {
            string positions = ReadReply("pos_list");
            List<PositionInfo> list = (List<PositionInfo>)Parse(Parser.ParsePositionsList, positions);
            if (list != null)
                return list;
            else
                return null;
        }

        /// <summary>
        /// Получение списка подразделений (формат API)
        /// </summary>
        /// <returns>Список подразделений</returns>
        static public List<DepartmentInfo> GetParsedDepartmentsList()
        {
            string departments = ReadReply("dep_list");
            List<DepartmentInfo> list = (List<DepartmentInfo>)Parse(Parser.ParseDepartmentsList, departments);
            if (list != null)
                return list;
            else
                return null;
        }

        /// <summary>
        /// Получение информации о сотруднике
        /// </summary>
        /// <param name="id">Идентификатор сотрудника</param>
        /// <returns>Объект с данными о сотруднике</returns>
        static public Emp GetEmployee(string id)
        {
            EmployeeParsed emp = GetParsedEmployee(id);

            if (emp != null)
            {
                Emp employeeResult = APIConverter.ConvertEmployee(emp);
                if (employeeResult != null)
                    return employeeResult;
                else
                    return null;
            }
            else
                return null;
        }

        /// <summary>
        /// Получение информации о подразделении
        /// </summary>
        /// <param name="id">Идентификатор подразделения</param>
        /// <returns>Объект с данными о подразделении</returns>
        static public Dep GetDepartment(string id)
        {
            DepartmentParsed dep = GetParsedDepartment(id);

            if (dep != null)
            {
                Dep departmentResult = APIConverter.ConvertDepartment(dep);
                if (departmentResult != null)
                    return departmentResult;
                else
                    return null;
            }
            else
                return null;
        }

        /// <summary>
        /// Получение списка сотрудников подразделения
        /// </summary>
        /// <param name="id">Идентификатор подразделения</param>
        /// <returns>Список сотрудников указанного подразделения</returns>
        static public List<EmpInfo> GetEmployeeList(string id)
        {
            // получение списка с краткими данными о сотрудниках
            List<EmployeeInfo> list = GetParsedEmployeeList(id);

            if (list != null)
            {
                // объект списка в формате API
                EmployeeList empList = new EmployeeList { Employees = list.ToArray() };

                // объект списка в формате программы
                EmpList empsInfo = APIConverter.ConvertEmployeeList(empList);

                if (empsInfo != null)
                {
                    List<EmpInfo> result = new List<EmpInfo>();
                    foreach (EmpInfo em in empsInfo.Employees)
                        result.Add(em);
                    return result;
                }
                else
                    return null;
            }
            else
                return null;
        }

        /// <summary>
        /// Получение списка подразделений ТПУ
        /// </summary>
        /// <returns>Список подразделений</returns>
        static public List<DepInfo> GetDepartmentList()
        {
            // получение списка с краткими данными о подразделениях
            List<DepartmentInfo> list = GetParsedDepartmentsList();

            if (list != null)
            {
                // создание объекта списка
                DepartmentsList depList = new DepartmentsList { Departments = list.ToArray() };

                // получение объекта списка в формате программы
                DepsList departmentsInfo = APIConverter.ConvertDepartmentsList(depList);

                if (departmentsInfo != null)
                {
                    List<DepInfo> result = new List<DepInfo>();
                    foreach (DepInfo dp in departmentsInfo.Departments)
                        result.Add(dp);
                    return result;
                }
                else 
                    return null;
            }
            else
                return null;
        }
    }
}