using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    /// <summary>
    /// Праздник/выходные дни, устанавливаемые производственным
    /// календарем
    /// </summary>
    
    [Table("holidays")]
    public class Holiday
    {
        public int Id { get; set; }

        /// <summary>
        /// Название праздника/периода выходных
        /// </summary>
        [MaxLength(150)]
        public string Name { get; set; }
 
        /// <summary>
        /// Начало периода выходных дней
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Конец периода выходных дней
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }

        public Holiday() { }
    }
}
