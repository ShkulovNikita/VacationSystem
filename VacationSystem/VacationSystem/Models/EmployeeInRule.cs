using System.ComponentModel.DataAnnotations;

namespace VacationSystem.Models
{
    public class EmployeeInRule
    {
        public int EmployeeRuleId { get; set; }
        public EmployeeRule EmployeeRule { get; set; }

        [Required, MaxLength(50)]
        public string EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
