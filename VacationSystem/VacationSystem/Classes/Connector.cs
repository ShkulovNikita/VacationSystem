using System;
using System.Collections.Generic;
using System.Diagnostics;
using VacationSystem.Models;
using System.IO;

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
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
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
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return null;
                }
            }
            else
                return null;
        }

        /// <summary>
        /// Получение сотрудника по его идентификатору
        /// </summary>
        /// <param name="id">Идентификатор сотрудника</param>
        /// <returns>Сотрудник</returns>
        public static Employee GetEmployee(string id)
        {
            string emp = ReadReply("employees/emp" + id);
            Employee employee = (Employee)Parse(Parser.ParseEmployee, emp);
            return employee;
        }

        /// <summary>
        /// Получение производственного календаря по году
        /// </summary>
        /// <param name="year">Год для получения производственного календаря</param>
        /// <returns>Производственный календарь за указанный год</returns>
        public static Holiday GetCalendar(int year)
        {
            string calendar = ReadReply("calendar/calendar" + year.ToString());
            Holiday holiday = (Holiday)Parse(Parser.ParseCalendar, calendar);
            return holiday;
        }

        /// <summary>
        /// Получение подразделения по его идентификатору
        /// </summary>
        /// <param name="id">Идентификатор подразделения</param>
        /// <returns>Подразделение</returns>
        public static Department GetDepartment(string id)
        {
            string dep = ReadReply("departments/dep" + id);
            Department department = (Department)Parse(Parser.ParseDepartment, dep);
            return department;
        }

        /// <summary>
        /// Получение списка сотрудников подразделения
        /// </summary>
        /// <param name="id">Идентификатор подразделения</param>
        /// <returns>Список сотрудников</returns>
        public static List<Employee> GetEmployeesOfDepartment(string id)
        {
            string emps = ReadReply("emps_in_deps/dep" + id);
            List<Employee> employees = (List<Employee>)Parse(Parser.ParseEmployees, emps);
            return employees;
        }

        /// <summary>
        /// Получение списка всех подразделений ТПУ
        /// </summary>
        /// <returns>Список подразделений</returns>
        public static List<Department> GetDepartments()
        {
            string deps = ReadReply("dep_list");
            List<Department> departments = (List<Department>)Parse(Parser.ParseDepartments, deps);
            return departments;
        }

        /// <summary>
        /// Получение списка должностей указанного сотрудника
        /// </summary>
        /// <param name="id">Идентификатор сотрудника</param>
        /// <returns>Список должностей сотрудника</returns>
        public static List<PositionInDepartment> GetEmployeePositions(string id)
        {
            string pos = ReadReply("emp_positions/emp" + id);
            List<PositionInDepartment> positions = (List<PositionInDepartment>)Parse(Parser.ParsePositionsInDepartments, pos);
            return positions;
        }

        /// <summary>
        /// Получение списка должностей
        /// </summary>
        /// <returns>Список всех должностей</returns>
        public static List<Position> GetPositions()
        {
            string pos = ReadReply("pos_list");
            List<Position> positions = (List<Position>)Parse(Parser.ParsePositions, pos);
            return positions;
        }

        /// <summary>
        /// Получение старшего подразделения для указанного
        /// </summary>
        /// <param name="id">Идентификатор подразделения</param>
        /// <returns>Старшее подразделение относительно заданного подразделения</returns>
        public static Department GetHeadDepartment(string id)
        {
            string dep = ReadReply("head_deps/dep" + id);
            Department department = (Department)Parse(Parser.ParseDepartment, dep);
            return department;
        }

        /// <summary>
        /// Получение руководителя подразделения
        /// </summary>
        /// <param name="id">Идентификатор подразделения</param>
        /// <returns>Руководитель указанного подразделения</returns>
        public static Employee GetHeadOfDepartment(string id)
        {
            string emp = ReadReply("heads_of_deps/dep" + id);
            Employee employee = (Employee)Parse(Parser.ParseEmployee, emp);
            return employee;
        }

        /// <summary>
        /// Получение должностей сотрудника в указанном подразделении
        /// </summary>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <param name="empId">Идентификатор сотрудника</param>
        /// <returns>Список должностей указанного сотрудника в заданном подразделении</returns>
        public static List<PositionInDepartment> GetPositionsInDepartment(string depId, string empId)
        {
            string pos = ReadReply("pos_in_deps/dep" + depId + "/emp" + empId);
            List<PositionInDepartment> positions = (List<PositionInDepartment>)Parse(Parser.ParsePositionsInDepartments, pos);
            return positions;
        }

        /// <summary>
        /// Получение указанной должности
        /// </summary>
        /// <param name="id">Идентификатор должности</param>
        /// <returns>Должность</returns>
        public static Position GetPosition(string id)
        {
            string pos = ReadReply("positions/pos" + id);
            Position position = (Position)Parse(Parser.ParsePosition, pos);
            return position;
        }

        /// <summary>
        /// Получение младшних подразделений
        /// </summary>
        /// <param name="id">Идентификатор старшего подразделения</param>
        /// <returns>Список младших подразделений относительно заданного
        /// старшего подразделения</returns>
        public static List<Department> GetLowerDepartments(string id)
        {
            string deps = ReadReply("sub_deps/dep" + id);
            List<Department> departments = (List<Department>)Parse(Parser.ParseDepartments, deps);
            return departments;
        }

        /// <summary>
        /// Получение подчиненных подразделений
        /// </summary>
        /// <param name="id">Идентификатор руководителя</param>
        /// <returns>Список подчиненных подразделений указанного руководителя</returns>
        public static List<Department> GetSubordinateDepartments(string id)
        {
            string deps = ReadReply("sub_deps_head/emp" + id);
            List<Department> departments = (List<Department>)Parse(Parser.ParseDepartments, deps);
            return departments;
        }

        /// <summary>
        /// Получение подчиненных сотрудников
        /// </summary>
        /// <param name="id">Идентификатор руководителя</param>
        /// <returns>Список сотрудников, подчиненных указанному руководителю</returns>
        public static List<Employee> GetSubordinateEmployees(string id)
        {
            string emps = ReadReply("sub_emps/emp" + id);
            List<Employee> employees = (List<Employee>)Parse(Parser.ParseEmployees, emps);
            return employees;
        }
    }
}