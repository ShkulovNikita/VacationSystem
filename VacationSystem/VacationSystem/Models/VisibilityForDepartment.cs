using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    /// <summary>
    /// Видимость отпусков других людей
    /// </summary>
    
    [Table("visibility_for_departments")]
    public class VisibilityForDepartment
    {
        public int Id { get; set; }

        [Required]
        public bool IsVisible { get; set; }

        [Required, MaxLength(50)]
        public string DepartmentId { get; set; }
        public Department Department { get; set; }

        [Required, MaxLength(50)]
        public string EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
