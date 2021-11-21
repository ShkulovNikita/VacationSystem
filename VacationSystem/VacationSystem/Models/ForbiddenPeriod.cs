using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace VacationSystem.Models
{
    /// <summary>
    /// периоды, в течение которых сотрудникам 
    /// отделения запрещено брать отпуска
    /// </summary>
    public class ForbiddenPeriod
    {
        public int Id { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}
