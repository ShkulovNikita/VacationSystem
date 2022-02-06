using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    /// <summary>
    /// Период, в течение которого сотрудникам 
    /// отделения запрещено брать отпуска
    /// </summary>
    
    [Table("forbidden_periods")]
    public class ForbiddenPeriod
    {
        public int Id { get; set; }

        /// <summary>
        /// Начало периода
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Конец периода
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Дата создания запрещенного периода
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Руководитель, задавший запрещенный период
        /// </summary>
        [Required, MaxLength(50)]
        public string HeadEmployeeId { get; set; }
        public Employee HeadEmployee { get; set; }

        /// <summary>
        /// Подразделение, в котором действует правило
        /// </summary>
        [Required, MaxLength(50)]
        public string DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}
