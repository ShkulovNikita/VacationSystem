using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    /// <summary>
    /// Видимость отпусков одного сотрудника 
    /// другому сотруднику
    /// </summary>
    
    [Table("visibility_for_employees")]
    public class VisibilityForEmployee
    {
        public int Id { get; set; }

        /// <summary>
        /// Факт видимости: отпуска видны или не видны
        /// </summary>
        public bool Visibility { get; set; }

        /// <summary>
        /// Дата, в которую было установлено
        /// правило видимости
        /// </summary>
        public DateTime SetDate { get; set; }

        /// <summary>
        /// Отделение, в котором работает данное правило
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

        /// <summary>
        /// Сотрудник, которому видны или не видны отпуска
        /// другого сотрудника
        /// </summary>
        [Required, MaxLength(50)]
        public string VisibilityEmployeeId { get; set; }
        public Employee VisibilityEmployee { get; set; }

        /// <summary>
        /// Сотрудник, отпуска которого открываются или
        /// скрываются от другого сотрудника
        /// </summary>
        [Required, MaxLength(50)]
        public string TargetEmployeeId { get; set; }
        public Employee TargetEmployee { get; set; }
    }
}