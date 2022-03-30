using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    /// <summary>
    /// Праздник/выходные дни, устанавливаемые производственным
    /// календарем
    /// </summary>
    
    [NotMapped]
    [Table("holidays")]
    public class Holiday
    {
        public int id { get; set; }

        public List<DateTime> Dates { get; set; } = new List<DateTime>();

        public Holiday() { }
    }
}
