using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    /// <summary>
    /// Статус отпуска, например, "назначен",
    /// "продлен", "перенесен" и т.д.
    /// </summary>

    [Table("vacation_statuses")]
    public class VacationStatus
    {
        public int Id { get; set; }

        /// <summary>
        /// Название статуса отпуска
        /// </summary>
        [Required, MaxLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// Назначенные отпуска, которым был присвоен
        /// данный статус
        /// </summary>
        public List<SetVacation> SetVacations { get; set; } = new List<SetVacation>();
    }
}