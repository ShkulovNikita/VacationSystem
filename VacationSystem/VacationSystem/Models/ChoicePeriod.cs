using System;
using System.ComponentModel.DataAnnotations;

namespace VacationSystem.Models
{
    public class ChoicePeriod
    {
        [Key]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required, MaxLength(50)]
        public string DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}
