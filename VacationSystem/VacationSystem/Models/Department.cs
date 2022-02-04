using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    [Table("departments")]
    public class Department
    {
        [Key, Required, MaxLength(50)]
        public string Id { get; set; }

        [Required, MaxLength(250)]
        public string Name { get; set; }

        public List<RuleForPosition> RuleForPositions { get; set; } = new List<RuleForPosition>();

        public List<ForbiddenPeriod> ForbiddenPeriods { get; set; } = new List<ForbiddenPeriod>();

        public List<ChoicePeriod> ChoicePeriods { get; set; } = new List<ChoicePeriod>();

        public List<EmployeeRule> EmployeeRules { get; set; } = new List<EmployeeRule>();

        public List<VisibilityForDepartment> VisibilityForDepartments { get; set; } = new List<VisibilityForDepartment>();

        public List<HeadStyle> HeadStyles { get; set; } = new List<HeadStyle>();

        public List<VisibilityForEmployee> VisibilityForEmployees { get; set; } = new List<VisibilityForEmployee>();

        public List<Deputy> DeputyEmployees { get; set; } = new List<Deputy>();

        public List<Group> Groups { get; set; } = new List<Group>();
    }
}
