using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    /// <summary>
    /// стиль управления конкретного руководителя
    /// в конкретном отделении
    /// </summary>
    
    [Table("heads_styles")]
    public class HeadStyle
    {
        public DateTime StyleSetDate { get; set; }

        public int ManagementStyleId { get; set; }
        public ManagementStyle ManagementStyle { get; set; }

        [Required, MaxLength(50)]
        public string EmployeeId { get; set; }
        public Employee Employee { get; set; }

        [Required, MaxLength(50)]
        public string DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}
