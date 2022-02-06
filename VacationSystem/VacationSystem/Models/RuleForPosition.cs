using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    /// <summary>
    /// Правило, установленное на должность 
    /// внутри подразделения
    /// </summary>
    
    [Table("rules_for_positions")]
    public class RuleForPosition
    {
        public int Id { get; set; }

        /// <summary>
        /// Количество сотрудников определенной должности,
        /// которые должны находиться одновременно на рабочем месте
        /// </summary>
        public int PeopleNumber { get; set; }

        /// <summary>
        /// Дата задания данного правила для должности
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Описание правила
        /// </summary>
        [MaxLength(100)]
        public string Description { get; set; }

        /// <summary>
        /// Должность, на которую устанавливается правило
        /// </summary>
        [Required, MaxLength(50)]
        public string PositionId { get; set; }
        public Position Position { get; set; }

        /// <summary>
        /// Подразделение, в котором действует правило
        /// </summary>
        [Required, MaxLength(50)]
        public string DepartmentId { get; set; }
        public Department Department { get; set; }

        /// <summary>
        /// Руководитель, которым было задано правило
        /// </summary>
        [Required, MaxLength(50)]
        public string HeadEmployeeId { get; set; }
        public Employee HeadEmployee { get; set; } 
    }
}