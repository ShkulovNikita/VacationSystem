using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    [Table("employees_in_groups")]
    public class EmployeeInGroup
    {
        [Required, MaxLength(50)]
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        [Required]
        public int GroupId { get; set; }
        public Group Group { get; set; }

        public DateTime Date { get; set; }
    }
}