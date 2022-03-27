using VacationSystem.Models;
using System.Collections.Generic;

namespace VacationSystem.ViewModels
{
    /// <summary>
    /// ViewModel для отображения информации о группах сотрудников
    /// </summary>
    public class GroupViewModel
    {
        /// <summary>
        /// Группа сотрудников
        /// </summary>
        public Group Group { get; set; }

        /// <summary>
        /// Подразделение группы
        /// </summary>
        public Department Department { get; set; }

        /// <summary>
        /// Список сотрудников в группе
        /// </summary>
        public List<Employee> Employees { get; set; }
    }
}