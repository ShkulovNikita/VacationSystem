using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    [Table("employees")]
    public class Employee
    {
        [Key, Required, MaxLength(50)]
        public string Id { get; set; }

        [Required, MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string MiddleName { get; set; }

        [Required, MaxLength(50)]
        public string LastName { get; set; }

        public List<EmployeeRule> EmployeeRules { get; set; } = new List<EmployeeRule>();

        public List<VisibilityForDepartment> VisibilityForDepartments { get; set; } = new List<VisibilityForDepartment>();

        public List<SetVacation> SetVacations { get; set; } = new List<SetVacation>();

        public List<WishedVacationPeriod> WishedVacationPeriods { get; set; } = new List<WishedVacationPeriod>();

        public List<VacationDay> VacationDays { get; set; } = new List<VacationDay>();

        public List<HeadStyle> HeadStyles { get; set; } = new List<HeadStyle>();

        public List<VisibilityForEmployee> VisibilityHeads { get; set; } = new List<VisibilityForEmployee>();

        public List<VisibilityForEmployee> VisibilityTargets { get; set; } = new List<VisibilityForEmployee>();

        public List<VisibilityForEmployee> VisibilityEmployees { get; set; } = new List<VisibilityForEmployee>();
    }
}