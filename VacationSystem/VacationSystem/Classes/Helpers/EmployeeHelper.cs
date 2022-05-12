using System.Collections.Generic;
using System.Linq;
using VacationSystem.Models;
using VacationSystem.ViewModels;
using VacationSystem.ViewModels.ListItems;

namespace VacationSystem.Classes.Helpers
{
    /// <summary>
    /// Класс для различных операций с сотрудниками
    /// </summary>
    static public class EmployeeHelper
    {
        /// <summary>
        /// Отфильтровать имеющийся список сотрудников согласно поисковому запросу
        /// </summary>
        /// <param name="employees">Список всех сотрудников</param>
        /// <param name="query">Поисковый запрос</param>
        /// <returns>Список сотрудников, удовлетворяющих запросу</returns>
        static public List<Employee> SearchEmployees(List<Employee> employees, string query)
        {
            return (from emp in employees
                    where emp.FirstName.ToLower().Contains(query.ToLower())
                    || emp.MiddleName.ToLower().Contains(query.ToLower())
                    || emp.LastName.ToLower().Contains(query.ToLower())
                    select emp).ToList();
        }

        /// <summary>
        /// Получить должности сотрудника во всех его подразделениях
        /// </summary>
        /// <param name="id">Идентификатор сотрудника</param>
        /// <returns>Список, содержащий данные о должностях сотрудника в подразделениях</returns>
        static public List<DepPositionsViewModel> GetPositionsInDepartments(string id)
        {
            // все должности сотрудника в подразделениях
            List<PositionInDepartment> positions = Connector.GetEmployeePositions(id);

            if (positions == null)
                return null;

            if (positions.Count == 0)
                return null;

            // должности по подразделениям
            List<DepPositionsViewModel> posInDeps = new List<DepPositionsViewModel>();

            // пройти по всем должностям сотрудника
            foreach (PositionInDepartment pos in positions)
            {
                // проверить, было ли добавлено подразделение данной должности в список
                // уже есть такое подразделение - добавить к нему
                if (posInDeps.Any(p => p.Department.Id == pos.Department))
                {
                    // получить соответствующую должность из API
                    Position position = Connector.GetPosition(pos.Position);

                    if (position == null)
                        continue;

                    // добавить должность к уже добавленному подразделению
                    posInDeps.Find(p => p.Department.Id == pos.Department)
                        .Positions.Add(position);
                }
                // для такого подразделения ещё не были добавлены должности
                else
                {
                    // создание новой пары подразделение-должность
                    DepPositionsViewModel depPos = new DepPositionsViewModel();

                    // получить из API данные о подразделении и должности
                    Position position = Connector.GetPosition(pos.Position);
                    Department department = Connector.GetDepartment(pos.Department);

                    if ((position == null) || (department == null))
                        return null;

                    // добавить пару подразделение-должность
                    depPos.Department = department;
                    depPos.Positions.Add(position);

                    // сохранить пару в общий список
                    posInDeps.Add(depPos);
                }
            }

            return posInDeps;
        }

        /// <summary>
        /// Получить всех сотрудников указанных подразделений
        /// </summary>
        /// <param name="deps">Список подразделений, для которых нужно получить сотрудников</param>
        /// <returns>Список сотрудников указанных подразделений</returns>
        static public List<Employee> GetEmployees(List<Department> deps)
        {
            List<Employee> employees = new List<Employee>();

            // пройтись по всем подразделениям, чтобы получить их сотрудников
            foreach (Department dep in deps)
            {
                // сотрудники одного подразделения
                List<Employee> empsOfDep = Connector.GetEmployeesOfDepartment(dep.Id);

                if (empsOfDep == null)
                    continue;

                // те сотрудники, которые ещё не были добавлены в общий список
                List<Employee> newEmps = empsOfDep
                    .Where(e => !employees.Any(emp => emp.Id == e.Id))
                    .ToList();

                employees.AddRange(newEmps);
            }

            return employees;
        }

        /// <summary>
        /// Преобразование списка сотрудников в формат ViewModel
        /// </summary>
        /// <param name="employees">Список сотрудников в формате модели</param>
        /// <returns>Информация о сотрудниках в формате ViewModel</returns>
        static public EmployeesViewModel ConvertEmployeesToViewModel(List<Employee> employees)
        {
            // создание модели представления
            EmployeesViewModel emps = new EmployeesViewModel();

            // создать список сотрудников
            List<EmpDepViewModel> employeesInUni = new List<EmpDepViewModel>();

            // конвертировать формат БД в формат модели представления
            foreach (Employee employee in employees)
                employeesInUni.Add(new EmpDepViewModel(employee));

            // передать список сотрудников
            emps.Employees = employeesInUni
                .OrderBy(e => e.LastName)
                .ThenBy(e => e.FirstName)
                .ThenBy(e => e.MiddleName)
                .ToList();

            return emps;
        }

        /// <summary>
        /// Преобразование списка сотрудников в формат ViewModel с их должностями в подразделении
        /// </summary>
        /// <param name="employees">Список сотрудников в формате модели</param>
        /// <param name="dep">Подразделение сотрудников</param>
        /// <returns>Информация о сотрудниках с их должностями в указанном подразделении в формате ViewModel</returns>
        static private List<EmpDepViewModel> ConvertEmpsAndPositionsToViewModel(List<Employee> employees, Department dep)
        {
            // список сотрудников в подразделении с их должностями
            List<EmpDepViewModel> empsInDep = new List<EmpDepViewModel>();

            // перебрать полученный список сотрудников подразделения
            foreach (Employee employee in employees)
            {
                // передать в модель представления идентификатор и имя сотрудника
                EmpDepViewModel empInDep = new EmpDepViewModel(employee);

                // получить должности сотрудника в подразделении
                List<PositionInDepartment> positions = Connector.GetPositionsInDepartment(dep.Id, employee.Id);
                if (positions == null)
                    continue;

                List<Position> posOfDemp = new List<Position>();
                foreach (PositionInDepartment pos in positions)
                {
                    Position newPos = Connector.GetPosition(pos.Position);
                    if (newPos != null)
                        posOfDemp.Add(newPos);
                }

                if (posOfDemp.Count > 0)
                {
                    empInDep.Positions = posOfDemp;
                    empsInDep.Add(empInDep);
                }
            }

            return empsInDep;
        }

        /// <summary>
        /// Преобразование списка сотрудников в формат ViewModel
        /// </summary>
        /// <param name="employees">Список сотрудников в формате модели</param>
        /// <param name="dep">Подразделение сотрудников</param>
        /// <returns>Информация о сотрудниках в формате ViewModel</returns>
        static public EmployeesViewModel ConvertEmployeesToViewModel(List<Employee> employees, Department dep)
        {
            // список сотрудников в подразделении с их должностями
            List<EmpDepViewModel> empsInDep = ConvertEmpsAndPositionsToViewModel(employees, dep);

            return new EmployeesViewModel
            {
                Department = dep,
                Employees = empsInDep
                    .OrderBy(e => e.LastName)
                    .ThenBy(e => e.FirstName)
                    .ThenBy(e => e.MiddleName)
                    .ToList()
            };
        }

        /// <summary>
        /// Преобразовать объект сотрудника во ViewModel
        /// </summary>
        /// <param name="emp">Объект сотрудника в формате модели</param>
        /// <returns>Объект сотрудника в формате ViewModel</returns>
        static public EmployeeViewModel ConvertEmployeeToViewModel(Employee emp)
        {
            // объект модели представления с данными о сотруднике
            EmployeeViewModel employee = new EmployeeViewModel
            {
                Id = emp.Id,
                FirstName = emp.FirstName,
                MiddleName = emp.MiddleName,
                LastName = emp.LastName
            };

            // должности сотрудника в его подразделениях
            List<DepPositionsViewModel> positions = EmployeeHelper.GetPositionsInDepartments(employee.Id);
            if (positions != null)
                employee.PositionsInDepartments = positions;

            // получить подразделения, которыми управляет данный сотрудник
            List<Department> subordinateDepartments = Connector.GetSubordinateDepartments(employee.Id);
            if (subordinateDepartments != null)
                employee.SubordinateDepartments = subordinateDepartments
                    .OrderBy(d => d.Name)
                    .ToList();

            return employee;
        }

        /// <summary>
        /// На основе списка идентификаторов сотрудников получить 
        /// список существующих сотрудников
        /// </summary>
        /// <param name="emps">Список идентификаторов запрашиваемых сотрудников</param>
        /// <returns>Список сотрудников</returns>
        static public List<Employee> CheckEmployeesInApi(string[] emps)
        {
            List<Employee> result = new List<Employee>();

            foreach (string emp in emps)
            {
                Employee newEmp = Connector.GetEmployee(emp);
                if (newEmp != null)
                    result.Add(newEmp);
            }

            return result;
        }

        /// <summary>
        /// Преобразование объекта сотрудника в формат для списка
        /// </summary>
        /// <param name="headId">Идентификатор руководителя подразделения</param>
        /// <param name="emp">Сотрудник как объект класса модели</param>
        /// <param name="dep">Подразделение сотрудника</param>
        /// <returns>Объект сотрудника в формате ViewModel для выпадающего списка сотрудников</returns>
        static public EmpListItem GetEmployeeListItem(Employee emp, DepListItem dep)
        {
            // новый сотрудник в формате ViewModel
            return new EmpListItem
            {
                EmpId = emp.Id,
                Name = emp.LastName + " " + emp.FirstName + " " + emp.MiddleName,
                Department = dep,
                DepartmentId = dep.Id
            };
        }

        /// <summary>
        /// Получить список сотрудников указанных подразделений
        /// для формирования выпадающего списка
        /// </summary>
        /// <param name="departments">Список подразделений сотрудников</param>
        /// <returns>Список сотрудников</returns>
        static public List<EmpListItem> GetEmployeesList(List<DepListItem> departments)
        {
            List<EmpListItem> emps = new List<EmpListItem>();

            // получить сотрудников всех подразделений
            foreach (DepListItem dep in departments)
            {
                // получить всех сотрудников данного подразделения
                List<Employee> employees = Connector.GetEmployeesOfDepartment(dep.Id);
                if (employees == null)
                    continue;

                // добавить сотрудников в списки
                foreach (Employee emp in employees)
                {
                    EmpListItem newEmp = GetEmployeeListItem(emp, dep);

                    // добавить в список сотрудников подразделения
                    dep.Employees.Add(newEmp);

                    // добавить в список всех сотрудников
                    emps.Add(newEmp);
                }
            }

            return emps;
        }

        /// <summary>
        /// Получить ФИО сотрудника
        /// </summary>
        /// <param name="emp">Сотрудник</param>
        /// <returns>ФИО сотрудника</returns>
        static public string GetFullName(Employee emp)
        {
            if (emp.MiddleName == "")
                return emp.LastName + " " + emp.FirstName[0] + ".";
            else
                return emp.LastName + " " + emp.FirstName[0] + ". " + emp.MiddleName[0] + ".";
        }

        /// <summary>
        /// Проверить, является ли заданный руководитель главой указанного сотрудника
        /// </summary>
        /// <param name="headId">Предполагаемый руководитель сотрудника</param>
        /// <param name="empId">Сотрудник</param>
        /// <returns>true - является руководителем, bool - нет</returns>
        static public bool IsHead(string headId, string empId)
        {
            // получить список подчиненных сотрудников руководителя
            List<Employee> subEmps = Connector.GetSubordinateEmployees(headId);

            // найти среди них сотрудника с идентификатором empId
            Employee emp = subEmps.FirstOrDefault(e => e.Id == empId);

            if (emp != null)
                return true;
            else
                return false;
        }
    }
}