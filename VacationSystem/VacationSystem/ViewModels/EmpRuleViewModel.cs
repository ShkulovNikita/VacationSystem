using System.Collections.Generic;
using VacationSystem.Models;

namespace VacationSystem.ViewModels
{
    /// <summary>
    /// Модель представления с информацией о правиле для сотрудников
    /// </summary>
    public class EmpRuleViewModel
    {
        public EmpRuleViewModel() { }

        /// <summary>
        /// Отображаемое правило для сотрудников
        /// </summary>
        public EmployeeRule Rule { get; set; }

        /// <summary>
        /// Список сотрудников, затронутых правилом
        /// </summary>
        public List<Employee> Employees { get; set; } = new List<Employee>();
    }
}
