using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    /// <summary>
    /// Даты обновления дней отпусков сотрудников
    /// </summary>

    [Table("updates")]
    public class Update
    {
        public int Id { get; set; }

        /// <summary>
        /// Дата, в которую происходило обновление дней отпусков
        /// </summary>
        [Required]
        public DateTime Date { get; set; }
    }
}
