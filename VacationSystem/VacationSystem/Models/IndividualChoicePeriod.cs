using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    /// <summary>
    /// Заданный сотруднику индивидуальный период
    /// для выбора отпуска
    /// </summary>

    [Table("individual_choice_periods")]
    public class IndividualChoicePeriod
    {
        public int Id { get; set; }

        /// <summary>
        /// Начало периода выбора отпуска
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Конец периода выбора отпуска
        /// </summary>
        [Required]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Дата назначения данного периода выбора 
        /// отпуска сотруднику
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Сотрудник, которому был задан данный
        /// период выбора отпуска
        /// </summary>
        [Required, MaxLength(50)]
        public string EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
