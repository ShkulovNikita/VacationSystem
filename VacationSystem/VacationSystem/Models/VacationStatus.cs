using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VacationSystem.Models
{
    public class VacationStatus
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        public List<SetVacation> SetVacations { get; set; } = new List<SetVacation>();
    }
}
