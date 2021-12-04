using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    /// <summary>
    /// Правила, устанавливаемые на конкретного сотрудника
    /// </summary>
    
    [Table("employees_rules")]
    public class EmployeeRule
    {
        public int Id { get; set; }

        public int RuleTypeId { get; set; }
        public RuleType RuleType { get; set; }

        [Required, MaxLength(50)]
        public string DepartmentId { get; set; }
        public Department Department { get; set; }

        [Required, MaxLength(50)]
        public string EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public List<EmployeeInRule> EmployeeInRules { get; set; } = new List<EmployeeInRule>();
    }
}