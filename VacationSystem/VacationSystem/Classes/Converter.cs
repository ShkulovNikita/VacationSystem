using System.Collections.Generic;
using VacationSystem.Models;
using VacationSystem.ParsingClasses;

namespace VacationSystem.Classes
{
    static public class Converter
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

        static public Department ConvertToDepartment(DepartmentInfo dep)
        {
            Department dep_result = new Department
            {
                Id = dep.Id,
                Name = dep.Name
            };

            return dep_result;
        }

        static public List<Department> ConvertToDepartments(List<DepartmentInfo> deps)
        {
            List<Department> deps_result = new List<Department>();

            foreach (DepartmentInfo dep in deps)
                deps_result.Add(ConvertToDepartment(dep));

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