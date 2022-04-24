using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    /// <summary>
    /// Отдельная часть выбранного сотрудником периода отпуска
    /// </summary>
    [Table("vacation_parts")]
    public class VacationPart
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
        /// Порядковый номер периода отпуска,
        /// если он разбит на несколько частей
        /// </summary>
        public int Part { get; set; }

        /// <summary>
        /// Идентификатор периода
        /// </summary>
        public int WishedVacationPeriodId { get; set; }

        /// <summary>
        /// Ссылка на период
        /// </summary>
        public WishedVacationPeriod WishedVacationPeriod { get; set; }
    }
}
