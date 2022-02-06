using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    /// <summary>
    /// Видимость отпусков людей
    /// внутри целого подразделения
    /// </summary>
    
    [Table("visibility_for_departments")]
    public class VisibilityForDepartment
    {
        public int Id { get; set; }

        /// <summary>
        /// Факт видимости отпусков друг друга для
        /// сотрудников одного отделения
        /// </summary>
        [Required]
        public bool IsVisible { get; set; }

        /// <summary>
        /// Дата, в которую было установлено правило видимости
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Подразделение, в котором устанавливаются правила видимости 
        /// </summary>
        [Required, MaxLength(50)]
        public string DepartmentId { get; set; }
        public Department Department { get; set; }

        /// <summary>
        /// Руководитель, которым было установлено правило
        /// </summary>
        [Required, MaxLength(50)]
        public string HeadEmployeeId { get; set; }
        public Employee HeadEmployee { get; set; }
    }
}