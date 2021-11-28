using System.ComponentModel.DataAnnotations;

namespace VacationSystem.Models
{
    /// <summary>
    /// видимость отпусков одного сотрудника 
    /// другому сотруднику
    /// </summary>
    public class VisibilityForEmployee
    {
        public int Id { get; set; }

        public bool Visibility { get; set; }

        [Required, MaxLength(50)]
        public string DepartmentId { get; set; }
        public Department Department { get; set; }

        [Required, MaxLength(50)]
        public string HeadEmployeeId { get; set; }
        public Employee HeadEmployee { get; set; }

        [Required, MaxLength(50)]
        public string VisibilityEmployeeId { get; set; }
        public Employee VisibilityEmployee { get; set; }

        [Required, MaxLength(50)]
        public string TargetEmployeeId { get; set; }
        public Employee TargetEmployee { get; set; }
    }
}