using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace VacationSystem.Models
{
    /// <summary>
    /// дни отпуска, имеющиеся в распоряжении
    /// сотрудника
    /// </summary>
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
    }
}
