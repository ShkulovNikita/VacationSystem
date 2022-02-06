using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    /// <summary>
    /// Период, в течение которого сотрудники 
    /// подразделения выбирают желаемое время отпуска
    /// </summary>

    [Table("choice_periods")]
    public class ChoicePeriod
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
        /// Дата назначения периода выбора отпусков
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Подразделение, в котором выполняется
        /// выбор отпусков
        /// </summary>
        [Required, MaxLength(50)]
        public string DepartmentId { get; set; }
        public Department Department { get; set; }

        /// <summary>
        /// Руководитель, назначивший период
        /// выбора отпусков
        /// </summary>
        public string HeadEmployeeId { get; set; }
        public Employee HeadEmployee { get; set; }
    }
}