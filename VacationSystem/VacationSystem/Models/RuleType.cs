using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VacationSystem.Models
{
    /// <summary>
    /// Типы правил, устанавливаемых для сотрудников
    /// </summary>
    public class RuleType
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; }

        public List<EmployeeRule> EmployeeRules { get; set; } = new List<EmployeeRule>();
    }
}