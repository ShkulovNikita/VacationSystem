using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    [Table("deputies")]
    public class Deputy
    {
        [Required, MaxLength(50)]
        public string HeadEmployeeId { get; set; }
        public Employee HeadEmployee { get; set; }

        [Required, MaxLength(50)]
        public string DeputyEmployeeId { get; set; }
        public Employee DeputyEmployee { get; set; }

        [Required, MaxLength(50)]
        public string DepartmentId { get; set; }
        public Department Department { get; set; }

        public DateTime Date { get; set; }
    }
}