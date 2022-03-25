using System.Collections.Generic;
using System.Linq;
using VacationSystem.Models;
using VacationSystem.ViewModels;

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
    }
}
