using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    /// <summary>
    /// Подразделение ТПУ
    /// </summary>

    [NotMapped, Table("departments")]
    public class Department
    {
        [Key, Required, MaxLength(50)]
        public string Id { get; set; }

        [NotMapped, MaxLength(150)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string HeadDepartment { get; set; }

        [MaxLength (50)]
        public string Head { get; set; }

        /// <summary>
        /// Правила для должностей внутри подразделения
        /// </summary>
        public List<RuleForPosition> RuleForPositions { get; set; } = new List<RuleForPosition>();

        /// <summary>
        /// Периоды, на которые запрещено брать отпуска
        /// </summary>
        public List<ForbiddenPeriod> ForbiddenPeriods { get; set; } = new List<ForbiddenPeriod>();

        /// <summary>
        /// Периоды, в течение которых сотрудники выбирают отпуска
        /// </summary>
        public List<ChoicePeriod> ChoicePeriods { get; set; } = new List<ChoicePeriod>();

        /// <summary>
        /// Правила отпусков для сотрудников внутри подразделения
        /// </summary>
        public List<EmployeeRule> EmployeeRules { get; set; } = new List<EmployeeRule>();

        /// <summary>
        /// Правила видимости отпусков внутри подразделения
        /// </summary>
        public List<VisibilityForDepartment> VisibilityForDepartments { get; set; } = new List<VisibilityForDepartment>();

        /// <summary>
        /// Заданные руководителями стили управления отпусками подразделения
        /// </summary>
        public List<HeadStyle> HeadStyles { get; set; } = new List<HeadStyle>();

        /// <summary>
        /// Правила видимости отпусков для конкретных сотрудников
        /// </summary>
        public List<VisibilityForEmployee> VisibilityForEmployees { get; set; } = new List<VisibilityForEmployee>();

        /// <summary>
        /// Заместители руководителя в подразделении
        /// </summary>
        public List<Deputy> DeputyEmployees { get; set; } = new List<Deputy>();

        /// <summary>
        /// Группы сотрудников в подразделении
        /// </summary>
        public List<Group> Groups { get; set; } = new List<Group>();
    }
}
