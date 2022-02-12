using System.Collections.Generic;
using VacationSystem.Models;
using VacationSystem.ProgramClasses;

namespace VacationSystem.Classes
{
    static public class UserHandler
    {
        /// <summary>
        /// Получение списка должностей сотрудника в его подразделениях
        /// </summary>
        /// <param name="employee">Сотрудник</param>
        /// <returns>Список должностей в подразделениях указанного сотрудника</returns>
        static public List<DepartmentPosition> GetEmployeePositions(Emp employee)
        {
            // пары из подразделений и соответствующих должностей сотрудника
            List<DepartmentPosition> positions = new List<DepartmentPosition>();

            // перебрать данные о подразделениях сотрудника
            foreach(DepEmpInfo info in employee.Departments)
            {
                // получение должности из БД
                Position position = DataHandler.GetPositionById(info.Position);

                // получение данных о подразделении из API
                Dep department = Connector.GetDepartment(info.Id);

                positions.Add(new DepartmentPosition
                {
                    Position = position,
                    Department = department
                });
            }

            return positions;
        }
    }
}
