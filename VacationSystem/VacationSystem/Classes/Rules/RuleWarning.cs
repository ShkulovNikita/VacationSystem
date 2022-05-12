using VacationSystem.Models;
using System.Collections.Generic;

namespace VacationSystem.Classes.Rules
{
    /// <summary>
    /// Класс для объектов, указывающих, какие правила не были соблюдены
    /// при попытке утверждения отпусков
    /// </summary>
    public class RuleWarning
    {
        /// <summary>
        /// Идентификатор правила, которое было нарушено
        /// </summary>
        public int RuleId { get; set; }

        /// <summary>
        /// Тип правила:
        /// emp - для сотрудников;
        /// pos - для должностей;
        /// group - для групп
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Описание нарушения правила
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Список сотрудников, нарушающих правило
        /// </summary>
        public List<Employee> Employees { get; set; }

        /// <summary>
        /// Список должностей, у которых было нарушено правило
        /// </summary>
        public List<Position> Positions { get; set; }
    }
}
