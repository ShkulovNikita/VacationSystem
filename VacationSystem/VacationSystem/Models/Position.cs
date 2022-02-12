using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    /// <summary>
    /// Должность сотрудника ТПУ
    /// </summary>

    [Table("positions")]
    public class Position
    {
        [Key, Required, MaxLength(50)]
        public string Id { get; set; }

        /// <summary>
        /// Наименование должности
        /// </summary>
        [Required, MaxLength(150)]
        public string Name { get; set; }

        /// <summary>
        /// Правила, установленные на данную должность
        /// </summary>
        public List<RuleForPosition> RuleForPositions { get; set; } = new List<RuleForPosition>();

        /// <summary>
        /// Сотрудники, занимающие данную должность в подразделениях
        /// </summary>
        public List<EmployeeInDepartment> EmployeeInDepartments { get; set; } = new List<EmployeeInDepartment>();
    }
}
