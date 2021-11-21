using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VacationSystem.Models
{
    /// <summary>
    /// Праздники, устанавливаемые календарем
    /// </summary>
    public class Holiday
    {
        public int Id { get; set; }

        // название праздника/периода выходных
        [MaxLength(150)]
        public string Name { get; set; }

        // начало периода выходных дней
        [Required]
        public DateTime StartDate { get; set; }

        // конец периода выходных дней
        [Required]
        public DateTime EndDate { get; set; }
    }
}
