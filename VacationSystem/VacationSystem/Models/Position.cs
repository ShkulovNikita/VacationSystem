using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VacationSystem.Models
{
    public class Position
    {
        [Key, Required, MaxLength(50)]
        public string Id { get; set; }

        public List<RuleForPosition> RuleForPositions { get; set; } = new List<RuleForPosition>();
    }
}
