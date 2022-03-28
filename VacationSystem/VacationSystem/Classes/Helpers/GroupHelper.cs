using System.Collections.Generic;
using VacationSystem.Models;
using VacationSystem.ViewModels;
using VacationSystem.Classes.Database;
using System.Linq;

namespace VacationSystem.Classes.Helpers
{
    /// <summary>
    /// Класс с различными методами для работы с группами сотрудников
    /// </summary>
    static public class GroupHelper
    {
        /// <summary>
        /// Получение списка групп сотрудников, созданных данным руководителем
        /// </summary>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <param name="departments">Список подразделений, которыми управляет данный руководитель</param>
        /// <returns>Список групп сотрудников, созданных руководителем</returns>
        static public List<Group> GetGroups(string headId, string depId, List<Department> departments)
        {
            List<Group> groups = new List<Group>();

            // получить данные о группах
            if (depId == null)
                groups = DataHandler.GetGroups(headId);
            else
                groups = DataHandler.GetGroups(headId, depId);

            if (groups == null)
                return null;

            // получение групп только из тех подразделений, которыми руководитель управляет в данный момент
            groups = groups
                .Where(group => departments.Any(dep => group.DepartmentId == dep.Id))
                .ToList();

            return groups;
        }

        /// <summary>
        /// Преобразование списка групп в формат ViewModel
        /// </summary>
        /// <param name="groups">Список групп в формате модели</param>
        /// <returns>Список групп в формате ViewModel</returns>
        static public List<GroupViewModel> ConvertGroupsToViewModel(List<Group> groups)
        {
            List<GroupViewModel> groupsList = new List<GroupViewModel>();

            foreach (Group group in groups)
            {
                GroupViewModel groupVm = ConvertGroupToViewModel(group);
                if (groupVm != null)
                    groupsList.Add(groupVm);
            }

            return groupsList;
        }

        /// <summary>
        /// Преобразование группы в формат ViewModel
        /// </summary>
        /// <param name="group">Группа в формате модели</param>
        /// <returns>Группа в формате ViewModel</returns>
        static public GroupViewModel ConvertGroupToViewModel(Group group)
        {
            // получить подразделение группы
            Department department = Connector.GetDepartment(group.DepartmentId);
            if (department == null)
                return null;

            // список сотрудников группы
            List<Employee> employees = GetGroupEmployees(group);
            if (employees == null)
                return null;

            return new GroupViewModel
            {
                Group = group,
                Department = department,
                Employees = employees
            };
        }

        /// <summary>
        /// Получить список сотрудников по идентификатору группы
        /// </summary>
        /// <param name="group">Группа сотрудников</param>
        /// <returns>Список сотрудников указанной группы</returns>
        static private List<Employee> GetGroupEmployees(Group group)
        {
            List<Employee> employees = new List<Employee>();

            // для всех идентификаторов получить соответствующих сотрудников
            foreach(string empId in group.EmployeesInGroup.Select(e => e.EmployeeId))
            {
                Employee emp = Connector.GetEmployee(empId);
                if (emp != null)
                    employees.Add(emp);
            }

            return employees
                .OrderBy(e => e.LastName)
                .ThenBy(e => e.FirstName)
                .ThenBy(e => e.MiddleName)
                .ToList();
        }
    }
}