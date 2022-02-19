using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    /// <summary>
    /// Подразделение ТПУ
    /// </summary>

    [Table("departments")]
    public class Department
    {
        [Key, Required, MaxLength(50)]
        public string Id { get; set; }

        /// <summary>
        /// Наименование подразделения
        /// </summary>
        [Required, MaxLength(250)]
        public string Name { get; set; }

        /// <summary>
        /// Является ли данная запись на данный момент действительной
        /// </summary>
        public bool isActive { get; set; } = true;

        /// <summary>
        /// Старшее подразделение
        /// </summary>
        [MaxLength(50)]
        public string HeadDepartmentId { get; set; }
        public Department HeadDepartment { get; set; }

        /// <summary>
        /// Руководитель подразделения
        /// </summary>
        [MaxLength(50)]
        public string HeadEmployeeId { get; set; }
        public Employee HeadEmployee { get; set; }

        /// <summary>
        /// Младшие подразделения
        /// </summary>
        public List<Department> ChildDepartments { get; set; } = new List<Department>();

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

        /// <summary>
        /// Сотрудники, занимающие должности в данном подразделении
        /// </summary>
        public List<EmployeeInDepartment> EmployeeInDepartments { get; set; } = new List<EmployeeInDepartment>();
    }
}
