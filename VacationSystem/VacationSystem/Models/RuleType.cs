using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    /// <summary>
    /// Типы правил, устанавливаемых для сотрудников
    /// </summary>
    
    [Table("rule_types")]
    public class RuleType
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; }

        public List<EmployeeRule> EmployeeRules { get; set; } = new List<EmployeeRule>();
    }
}