using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    [Table("positions")]
    public class Position
    {
        [Key, Required, MaxLength(50)]
        public string Id { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; }

        public List<RuleForPosition> RuleForPositions { get; set; } = new List<RuleForPosition>();
    }
}
