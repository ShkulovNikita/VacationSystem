using VacationSystem.Models;
using System;

namespace VacationSystem.ViewModels
{
    public class RuleViewModel
    {
        public RuleViewModel() { }

        /// <summary>
        /// Наименование правила
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Описание правила
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Тип правила
        /// </summary>
        public string RuleType { get; set; }

        /// <summary>
        /// Дата создания правила
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Подразделение, в котором действует правило
        /// </summary>
        public Department Department { get; set; }

        /// <summary>
        /// Описание правила на основе его типа и содержания
        /// </summary>
        public string SystemDescription { get; set; }
    }
}
