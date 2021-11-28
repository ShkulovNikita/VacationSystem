using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VacationSystem.Models
{
    /// <summary>
    /// уже установленный отпуск
    /// </summary>
    public class SetVacation
    {
        public int Id { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public int VacationStatusId { get; set; }
        public VacationStatus VacationStatus { get; set; }

        [Required, MaxLength(50)]
        public string EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
