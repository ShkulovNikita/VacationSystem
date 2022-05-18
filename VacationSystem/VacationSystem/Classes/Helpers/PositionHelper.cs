using System.Collections.Generic;
using VacationSystem.Models;
using VacationSystem.ViewModels.ListItems;
using System.Linq;

namespace VacationSystem.Classes.Helpers
{
    /// <summary>
    /// Класс с различными методами для работы с должностями
    /// </summary>
    static public class PositionHelper
    {
        /// <summary>
        /// Получить список должностей сотрудников одного подразделения
        /// </summary>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <returns>Список должностей сотрудников подразделения</returns>
        static public List<Position> GetDepartmentPositions(string depId)
        {
            // список сотрудников подразделения
            List<Employee> employees = Connector.GetEmployeesOfDepartment(depId);
            if (employees == null)
                return new List<Position>();

            // идентификаторы всех должностей всех сотрудников подразделения
            List<string> posIds = new List<string>();

            foreach (Employee employee in employees)
            {
                List<PositionInDepartment> empPositions = Connector.GetPositionsInDepartment(depId, employee.Id);
                if (empPositions != null)
                    posIds.AddRange(empPositions.Select(e => e.Position).ToList());
            }

            // удалить повторяющиеся должности
            posIds = posIds.Distinct().ToList();

            // итоговый список должностей
            List<Position> result = new List<Position>();
            foreach (string posId in posIds)
            {
                Position position = Connector.GetPosition(posId);
                if (position != null)
                    result.Add(position);
            }
            
            return result;
        }

        /// <summary>
        /// Получить список должностей для указанных подразделений
        /// для формирования выпадающего списка
        /// </summary>
        /// <param name="departments">Список подразделений</param>
        /// <returns>Список должностей сотрудников указанных подразделений</returns>
        static public List<PosListItem> GetPositionsList(List<DepListItem> departments)
        {
            List<PosListItem> result = new List<PosListItem>();

            // получить должности всех подразделений
            foreach (DepListItem dep in departments)
            {
                // получить все должности данного подразделения
                List<Position> positions = GetDepartmentPositions(dep.Id);
                if (positions == null)
                    continue;

                // добавить должности в списки
                foreach (Position pos in positions)
                {
                    PosListItem newPos = GetPositionListItem(pos, dep);

                    // добавить в список должностей подразделения
                    dep.Positions.Add(newPos);

                    // добавить в список всех должностей
                    result.Add(newPos);
                }
            }

            return result;
        }

        /// <summary>
        /// Преобразование должности в формат для списка
        /// </summary>
        /// <param name="pos">Должность в формате модели</param>
        /// <param name="dep">Подразделение должности</param>
        /// <returns>Должность в формате, пригодном для отображения
        ///  в выпадающем списке</returns>
        static public PosListItem GetPositionListItem(Position pos, DepListItem dep)
        {
            // новая должность в подходящем формате
            return new PosListItem
            {
                PosId = pos.Id,
                Name = pos.Name,
                Department = dep,
                DepartmentId = dep.Id
            };
        }

        /// <summary>
        /// Конвертация должностей из формата PositionInDepartment в Position
        /// </summary>
        /// <param name="positionInDepartments">Список должностей в формате пар "подразделение - должность"</param>
        /// <returns>Список должностей в формате модели Position</returns>
        static public List<Position> ConvertPositionsInDepartmentToPositions(List<PositionInDepartment> positionInDepartments)
        {
            List<Position> result = new List<Position>();

            foreach (PositionInDepartment pos in positionInDepartments)
            {
                Position posFromApi = Connector.GetPosition(pos.Position);
                if (posFromApi != null)
                    result.Add(posFromApi);
            }

            return result;
        }

        /// <summary>
        /// Получить список должностей сотрудников
        /// </summary>
        /// <param name="emps">Список сотрудников</param>
        /// <returns>Список должностей указанных сотрудников</returns>
        static public List<Position> GetEmployeesPositions(List<Employee> emps)
        {
            List<Position> result = new List<Position>();

            List<string> positionIds = new List<string>();

            foreach (Employee emp in emps)
            {
                List<PositionInDepartment> positions = Connector.GetEmployeePositions(emp.Id);
                positionIds.AddRange(positions.Select(p => p.Position).ToList());
            }

            positionIds = positionIds.Distinct().ToList();
            foreach (string posId in positionIds)
                result.Add(Connector.GetPosition(posId));

            return result.OrderBy(p => p.Name).ToList();
        }

        /// <summary>
        /// Получить список должностей сотрудников
        /// </summary>
        /// <param name="emps">Список сотрудников</param>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <returns>Список должностей указанных сотрудников</returns>
        static public List<Position> GetEmployeesPositions(List<Employee> emps, string depId)
        {
            List<Position> result = new List<Position>();

            List<string> positionIds = new List<string>();

            foreach (Employee emp in emps)
            {
                List<PositionInDepartment> positions = Connector.GetPositionsInDepartment(depId, emp.Id);
                positionIds.AddRange(positions.Select(p => p.Position).ToList());
            }

            positionIds = positionIds.Distinct().ToList();
            foreach (string posId in positionIds)
                result.Add(Connector.GetPosition(posId));

            return result.OrderBy(p => p.Name).ToList();
        }
    }
}
