using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    /// <summary>
    /// Желаемый сотрудником период отпуска,
    /// который он выбирает в указанный руководителем
    /// период времени (ChoicePeriod)
    /// </summary>
    
    [Table("wished_vacation_periods")]
    public class WishedVacationPeriod
    {
        public int Id { get; set; }

        /// <summary>
        /// Желаемое начало периода отпуска
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Желаемый конец периода отпуска
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Приоритет периода отпуска
        /// при наличии нескольких
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Порядковый номер периода отпуска,
        /// если он разбит на несколько частей
        /// </summary>
        public int Part { get; set; }

        /// <summary>
        /// Дата, в которую сотрудник выбрал
        /// данный желаемый период
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Сотрудник, выбирающий период отпуска
        /// </summary>
        [Required, MaxLength(50)]
        public string EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
