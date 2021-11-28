using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VacationSystem.Models
{
    public class VacationType
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        public List<VacationDay> VacationDays { get; set; } = new List<VacationDay>();
    }
}
