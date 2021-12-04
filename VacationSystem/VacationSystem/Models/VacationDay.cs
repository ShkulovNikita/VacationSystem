﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    /// <summary>
    /// дни отпуска, имеющиеся в распоряжении
    /// сотрудника
    /// </summary>

    [Table("vacation_days")]
    public class VacationDay
    {
        public int Id { get; set; }

        public int NumberOfDays { get; set; }

        public int UsedDays { get; set; }

        public int Year { get; set; }

        [MaxLength(500)]
        public string Notes { get; set; }

        [Required]
        public bool Paid { get; set; }

        public int VacationTypeId { get; set; }
        public VacationType VacationType { get; set; }

        [Required, MaxLength(50)]
        public string EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
