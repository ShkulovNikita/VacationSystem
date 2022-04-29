using System;
using System.Collections.Generic;
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
        /// Приоритет периода отпуска
        /// при наличии нескольких
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Дата, в которую сотрудник выбрал
        /// данный желаемый период
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Год, на который запланирован отпуск
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Сотрудник, выбирающий период отпуска
        /// </summary>
        [Required, MaxLength(50)]
        public string EmployeeId { get; set; }
        public Employee Employee { get; set; }

        /// <summary>
        /// Составные части данного периода отпуска
        /// </summary>
        public List<VacationPart> VacationParts { get; set; } = new List<VacationPart>();
    }
}
