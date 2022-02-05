using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    [Table("groups_rules")]
    public class GroupRule
    {
        public int Id { get; set; }

        [Required]
        public int RuleTypeId { get; set; }
        public RuleType RuleType { get; set; }

        [Required]
        public int GroupId { get; set; }
        public Group Group { get; set; }

        public DateTime Date { get; set; }
    }
}
