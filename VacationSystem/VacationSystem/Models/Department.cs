using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VacationSystem.Models
{
    public class Department
    {
        [Key, Required, MaxLength(50)]
        public string Id { get; set; }

        public List<RuleForPosition> RuleForPositions { get; set; } = new List<RuleForPosition>();

        public List<ForbiddenPeriod> ForbiddenPeriods { get; set; } = new List<ForbiddenPeriod>();

        public List<ChoicePeriod> ChoicePeriods { get; set; } = new List<ChoicePeriod>();
    }
}
