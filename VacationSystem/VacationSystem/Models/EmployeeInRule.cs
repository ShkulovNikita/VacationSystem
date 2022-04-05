using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    /// <summary>
    /// Применение правила к сотруднику
    /// </summary>
    [Table("employees_in_rules")]
    public class EmployeeInRule
    {
        /// <summary>
        /// Правило, применяемое к сотруднику
        /// </summary>
        public int EmployeeRuleId { get; set; }
        public EmployeeRule EmployeeRule { get; set; }

        /// <summary>
        /// Сотрудник, к которому применяется правило 
        /// </summary>
        [Required, MaxLength(50)]
        public string EmployeeId { get; set; }
        public Employee Employee { get; set; }

        /// <summary>
        /// Дата применения правила к сотруднику
        /// </summary>
        public DateTime Date { get; set; }
    }
}
