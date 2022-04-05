using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    /// <summary>
    /// Правило, устанавливаемое на сотрудников
    /// </summary>
    [Table("employees_rules")]
    public class EmployeeRule
    {
        public int Id { get; set; }

        /// <summary>
        /// Дата создания правила
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Описание правила
        /// </summary>
        [MaxLength(100)]
        public string Description { get; set; }

        /// <summary>
        /// Тип устанавливаемого правила
        /// </summary>
        [Required]
        public int RuleTypeId { get; set; }
        public RuleType RuleType { get; set; }

        /// <summary>
        /// Подразделение, в котором действует правило
        /// </summary>
        [Required, MaxLength(50)]
        public string DepartmentId { get; set; }
        public Department Department { get; set; }

        /// <summary>
        /// Руководитель, установивший правило
        /// </summary>
        [Required, MaxLength(50)]
        public string HeadEmployeeId { get; set; }
        public Employee HeadEmployee { get; set; }

        /// <summary>
        /// Сотрудники, к которым было применено правило
        /// </summary>
        public List<EmployeeInRule> EmployeeInRules { get; set; } = new List<EmployeeInRule>();
    }
}