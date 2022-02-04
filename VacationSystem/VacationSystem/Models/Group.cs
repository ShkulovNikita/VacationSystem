using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    [Table("groups")]
    public class Group
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [Required, MaxLength(50)]
        public string HeadId { get; set; }
        public Employee Head { get; set; }

        [Required, MaxLength(50)]
        public string DepartmentId { get; set; }
        public Department Department { get; set; }

        public DateTime Date { get; set; }
    }
}