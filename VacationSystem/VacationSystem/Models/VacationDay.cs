using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    /// <summary>
    /// Дни отпуска, имеющиеся в распоряжении
    /// сотрудника
    /// </summary>

    [Table("vacation_days")]
    public class VacationDay
    {
        public int Id { get; set; }

        /// <summary>
        /// Количество дней отпуска
        /// </summary>
        public int NumberOfDays { get; set; }

        /// <summary>
        /// Уже использованное количество дней
        /// </summary>
        public int UsedDays { get; set; }

        /// <summary>
        /// Количество дней, уже включенных в некоторый утвержденный и неотмененный отпуск
        /// </summary>
        public int TakenDays { get; set; }

        /// <summary>
        /// Год, на который назначены дни отпуска
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Примечания или обоснование дней отпуска
        /// </summary>
        [MaxLength(500)]
        public string Notes { get; set; }

        /// <summary>
        /// Являются ли эти дни отпуска оплачиваемыми
        /// </summary>
        [Required]
        public bool Paid { get; set; }

        /// <summary>
        /// Дата, в которую сотруднику были назначены
        /// данные дни отпуска
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Тип, которому принадлежат дни отпуска
        /// </summary>
        public int VacationTypeId { get; set; }
        public VacationType VacationType { get; set; }

        /// <summary>
        /// Сотрудник, имеющий данные дни отпуска
        /// </summary>
        [Required, MaxLength(50)]
        public string EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}