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

        static public List<Employee> ConvertToEmployees(List<EmployeeParsed> emps)
        {
            List<Employee> emps_result = new List<Employee>();

            foreach(EmployeeParsed emp in emps)
                emps_result.Add(ConvertToEmployee(emp));

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
    }
}