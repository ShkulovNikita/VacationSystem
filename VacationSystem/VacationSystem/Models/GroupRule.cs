using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    /// <summary>
    /// Правило, заданное для группы
    /// </summary>

    [Table("groups_rules")]
    public class GroupRule
    {
        public int Id { get; set; }

        /// <summary>
        /// Дата, в которую было задано правило
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Описание правила
        /// </summary>
        [MaxLength(100)]
        public string Description { get; set; }

        /// <summary>
        /// Тип задаваемого правила
        /// </summary>
        [Required]
        public int RuleTypeId { get; set; }
        public RuleType RuleType { get; set; }

        /// <summary>
        /// Группа, к которой применяется правило
        /// </summary>
        [Required]
        public int GroupId { get; set; }
        public Group Group { get; set; }
    }
}
