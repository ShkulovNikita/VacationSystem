using System.Collections.Generic;
using VacationSystem.Models;
using VacationSystem.ParsingClasses;

namespace VacationSystem.Classes
{
    /// <summary>
    /// Класс для преобразования объектов, 
    /// полученных из API, в объекты моделей
    /// </summary>
    static public class ModelConverter
    {
        static public Employee ConvertToEmployee(EmployeeParsed emp)
        {
            Employee emp_result = new Employee
            {
                Id = emp.Id,
                FirstName = emp.FirstName,
                MiddleName = emp.MiddleName,
                LastName = emp.LastName
            };

            return emp_result;
        }

        /// <summary>
        /// Конвертация списка с полной информации о сотрудникех в формате API
        /// в формат модели БД
        /// </summary>
        /// <param name="emps">Список с полной информацией о сотрудниках в формате API</param>
        /// <returns>Список объектов сотрудников в формате БД</returns>
        static public List<Employee> ConvertToEmployees(List<EmployeeParsed> emps)
        {
            List<Employee> emps_result = new List<Employee>();

            foreach(EmployeeParsed emp in emps)
                emps_result.Add(ConvertToEmployee(emp));

            return emps_result;
        }

        /// <summary>
        /// Конвертация списка с краткой информацией о сотрудниках в формате API
        /// в формат модели БД
        /// </summary>
        /// <param name="emps">Список с краткой информацией о сотрудниках в формате API</param>
        /// <returns>Список объектов сотрудников в формате БД</returns>
        static public List<Employee> ConvertToEmployees(List<EmployeeInfo> emps)
        {
            // список сотрудников в формате модели БД
            List<Employee> emps_result = new List<Employee>();

            // список с полной информацией о сотрудниках
            List<EmployeeParsed> emps_full = new List<EmployeeParsed>();

            // перебор всех сотрудников
            // с целью получения полной информации о них
            foreach(EmployeeInfo emp in emps)
                emps_full.Add(Connector.GetParsedEmployee(emp.Id));

            // преобразовать список с полной информацией в объекты модели БД
            emps_result = ConvertToEmployees(emps_full);

            return emps_result;
        }

        /// <summary>
        /// Преобразование подразделения из объекта API в объект модели
        /// </summary>
        /// <param name="dep">Подразделение в формате API</param>
        /// <param name="simpleForm">false - полная информация, true - только имя и идентификатор</param>
        /// <returns>Подразделение в формате модели БД</returns>
        static public Department ConvertToDepartment(DepartmentInfo dep, bool simple)
        {
            Department dep_result = new Department();

            dep_result.Id = dep.Id;
            dep_result.Name = dep.Name;

            // требуется полная информация
            if(!simple)
            {
                dep_result.HeadDepartmentId = dep.HeadDepId;
                dep_result.HeadEmployeeId = dep.Head;
            }

            return dep_result;
        }

        /// <summary>
        /// Преобразование подразделения из объекта API в объект модели
        /// </summary>
        /// <param name="dep">Подразделение в формате API</param>
        /// <returns>Подразделение в формате модели БД</returns>
        static public Department ConvertToDepartment(DepartmentParsed dep)
        {
            Department dep_result = new Department
            {
                Id = dep.Id,
                Name = dep.Name,
                HeadDepartmentId = dep.HeadDepID,
                HeadEmployeeId = dep.Head
            };

            return dep_result;
        }

        /// <summary>
        /// Преобразование списка подразделений из формата API в формат моделей
        /// </summary>
        /// <param name="deps">Список подразделений в формате API</param>
        /// <param name="simple">false - полная информация, true - только имя и идентификатор</param>
        /// <returns>Список подразделений в формате модели БД</returns>
        static public List<Department> ConvertToDepartments(List<DepartmentInfo> deps, bool simple)
        {
            List<Department> deps_result = new List<Department>();

            foreach (DepartmentInfo dep in deps)
                deps_result.Add(ConvertToDepartment(dep, simple));

            return deps_result;
        }

        static public Position ConvertToPosition(PositionInfo pos)
        {
            Position pos_result = new Position
            {
                Id = pos.Id,
                Name = pos.Name
            };

            return pos_result;
        }

        static public List<Position> ConvertToPositions(List<PositionInfo> positions)
        {
            List<Position> positions_result = new List<Position>();

            foreach (PositionInfo pos in positions)
                positions_result.Add(ConvertToPosition(pos));

            return positions_result;
        }

        static public List<EmployeeInDepartment> ConvertToPositionsInDepartments(EmployeeParsed emp)
        {
            List<EmployeeInDepartment> result = new List<EmployeeInDepartment>();

            // пройтись по всем должностям сотрудника в подразделениях
            foreach(DepartmentEmployeeInfo depInfo in emp.Departments)
            {
                result.Add(new EmployeeInDepartment
                {
                    EmployeeId = emp.Id,
                    DepartmentId = depInfo.Id,
                    PositionId = depInfo.Position
                });
            }

            if (result.Count > 0)
                return result;
            else
                return null;
        }

        /// <summary>
        /// Получить должности сотрудников указанного подразделения
        /// </summary>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <param name="emps">Список с информацией о сотрудниках в подразделении</param>
        /// <returns>Список должностей сотрудников в данном подразделении</returns>
        static public List<EmployeeInDepartment> ConvertToPositionsInDepartments(string depId, List<EmployeeInfo> emps)
        {
            List<EmployeeInDepartment> result = new List<EmployeeInDepartment>();

            // проход по информации о каждом сотруднике
            foreach (EmployeeInfo emp in emps)
            {
                result.Add(new EmployeeInDepartment
                {
                    EmployeeId = emp.Id,
                    DepartmentId = depId,
                    PositionId = emp.Position
                });
            }

            if (result.Count > 0)
                return result;
            else
                return null;
        }
    }
}