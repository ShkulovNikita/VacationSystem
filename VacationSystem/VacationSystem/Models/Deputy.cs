using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    /// <summary>
    /// Заместитель руководителя для назначения отпусков
    /// </summary>

    [Table("deputies")]
    public class Deputy
    {
        public int Id { get; set; }

        /// <summary>
        /// Руководитель заместителя
        /// </summary>
        [Required, MaxLength(50)]
        public string HeadEmployeeId { get; set; }
        public Employee HeadEmployee { get; set; }

        /// <summary>
        /// Заместитель руководителя
        /// </summary>
        [Required, MaxLength(50)]
        public string DeputyEmployeeId { get; set; }
        public Employee DeputyEmployee { get; set; }

        /// <summary>
        /// Подразделение, в котором сотрудник
        /// является заместителем руководителя
        /// </summary>
        [Required, MaxLength(50)]
        public string DepartmentId { get; set; }
        public Department Department { get; set; }

        /// <summary>
        /// Дата назначения заместителя
        /// </summary>
        public DateTime Date { get; set; }
    }
}