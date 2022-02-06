using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    /// <summary>
    /// Назначенный период отпуска
    /// </summary>
    
    [Table("set_vacations")]
    public class SetVacation
    {
        public int Id { get; set; }

        /// <summary>
        /// Дата начала отпуска
        /// </summary>
        [Required]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Дата окончания отпуска
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Дата, в которую отпуск был назначен сотруднику
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Статус данного отпуска
        /// </summary>
        public int VacationStatusId { get; set; }
        public VacationStatus VacationStatus { get; set; }

        /// <summary>
        /// Сотрудник, имеющий данный отпуск
        /// </summary>
        [Required, MaxLength(50)]
        public string EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
