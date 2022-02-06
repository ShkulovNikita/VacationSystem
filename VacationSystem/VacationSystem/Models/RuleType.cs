using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    /// <summary>
    /// Тип правила, устанавливаемого для сотрудников и групп
    /// </summary>
    
    [Table("rule_types")]
    public class RuleType
    {
        public int Id { get; set; }

        /// <summary>
        /// Имя типа правила
        /// </summary>
        [Required, MaxLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// Правила для сотрудников данного типа
        /// </summary>
        public List<EmployeeRule> EmployeeRules { get; set; } = new List<EmployeeRule>();

        /// <summary>
        /// Правила для групп данного типа
        /// </summary>
        public List<GroupRule> GroupRules { get; set; } = new List<GroupRule>();
    }
}