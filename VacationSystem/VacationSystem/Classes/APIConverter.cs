using VacationSystem.ParsingClasses;
using VacationSystem.ProgramClasses;

namespace VacationSystem.Classes
{
    /// <summary>
    /// Класс для преобразования объектов,
    /// полученных из API, во внутренние классы
    /// программы
    /// </summary>
    
    static public class APIConverter
    {
        /// <summary>
        /// Преобразование кратких данных о подразделении
        /// </summary>
        /// <param name="department">Подразделение в формате API</param>
        /// <returns>Подразделение в формате программы</returns>
        static public Dep ConvertDepartment(DepartmentParsed department)
        {
            return new Dep
            {
                Id = department.Id,
                Name = department.Name,
                Head = department.Head
            };
        }

        /// <summary>
        /// Преобразование данных о подразделении сотрудника
        /// </summary>
        /// <param name="info">Подразделение сотрудника в формате API</param>
        /// <returns>Подразделение сотрудника в формате программы</returns>
        static public DepEmpInfo ConvertDepartmentEmployeeInfo(DepartmentEmployeeInfo info)
        {
            return new DepEmpInfo
            {
                Id = info.Id,
                HeadOfDepartment = info.HeadOfDepartment,
                Position = info.Position
            };
        }

        /// <summary>
        /// Преобразование данных о подразделении
        /// </summary>
        /// <param name="dep">Подразделение в формате API</param>
        /// <returns>Подразделение в формате программы</returns>
        static public DepInfo ConvertDepartmentInfo(DepartmentInfo dep)
        {
            return new DepInfo
            {
                Id = dep.Id,
                Head = dep.Head,
                HeadDepId = dep.HeadDepId,
                Name = dep.Name
            };
        }

        /// <summary>
        /// Преобразование списка подразделений
        /// </summary>
        /// <param name="deps">Список подразделений ТПУ в формате API</param>
        /// <returns>Список подразделений в формате программы</returns>
        static public DepsList ConvertDepartmentsList(DepartmentsList deps)
        {
            DepInfo[] departmentsInfo = new DepInfo[deps.Departments.Length];

            for (int i = 0; i < departmentsInfo.Length; i++)
                departmentsInfo[i] = ConvertDepartmentInfo(deps.Departments[i]);

            return new DepsList
            {
                Departments = departmentsInfo
            };
        }

        /// <summary>
        /// Преобразование данных о сотруднике
        /// </summary>
        /// <param name="employee">Данные о сотруднике в формате API</param>
        /// <returns>Данные о сотруднике в формате программы</returns>
        static public Emp ConvertEmployee(EmployeeParsed employee)
        {
            DepEmpInfo[] depEmps = new DepEmpInfo[employee.Departments.Length];

            for (int i = 0; i < depEmps.Length; i++)
                depEmps[i] = ConvertDepartmentEmployeeInfo(employee.Departments[i]);

            return new Emp
            {
                Id = employee.Id,
                Birthdate = employee.Birthdate,
                Departments = depEmps,
                FirstName = employee.FirstName,
                MiddleName = employee.MiddleName,
                LastName = employee.LastName
            };
        }

        /// <summary>
        /// Преобразование данных о сотруднике в подразделении
        /// </summary>
        /// <param name="employee">Данные о сотруднике подразделения в формате API</param>
        /// <returns>Данные о сотруднике подразделения в формате программы</returns>
        static public EmpInfo ConvertEmployeeInfo(EmployeeInfo employee)
        {
            return new EmpInfo
            {
                Id = employee.Id,
                Head = employee.Head,
                Positions = new string[] { employee.Position }
            };
        }

        /// <summary>
        /// Преобразование списка сотрудников
        /// </summary>
        /// <param name="employees">Список сотрудников подразделения в формате API</param>
        /// <returns>Список сотрудников в формате программы</returns>
        static public EmpList ConvertEmployeeList(EmployeeList employees)
        {
            EmpInfo[] empInfo = new EmpInfo[employees.Employees.Length];

            for (int i = 0; i < empInfo.Length; i++)
                empInfo[i] = ConvertEmployeeInfo(employees.Employees[i]);

            return new EmpList
            {
                Id = employees.Id,
                Employees = empInfo
            };
        }
    }
}