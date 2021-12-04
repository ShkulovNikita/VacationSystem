using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    [Table("employees_in_rules")]
    public class EmployeeInRule
    {
        public int EmployeeRuleId { get; set; }
        public EmployeeRule EmployeeRule { get; set; }

        [Required, MaxLength(50)]
        public string EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
