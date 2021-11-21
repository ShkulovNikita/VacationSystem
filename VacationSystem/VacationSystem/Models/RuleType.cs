using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace VacationSystem.Models
{
    /// <summary>
    /// Типы правил, устанавливаемых для сотрудников
    /// </summary>
    public class RuleType
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; }
    }
}
