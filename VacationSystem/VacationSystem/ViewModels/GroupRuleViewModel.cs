using VacationSystem.Models;
using System.Collections.Generic;

namespace VacationSystem.ViewModels
{
    public class GroupRuleViewModel
    {
        public GroupRuleViewModel() { }

        /// <summary>
        /// Правило для группы
        /// </summary>
        public GroupRule Rule { get; set; }

        /// <summary>
        /// Список сотрудников группы, затронутые правилом
        /// </summary>
        public List<Employee> Employees { get; set; } = new List<Employee>();
    }
}
