using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    /// <summary>
    /// Сотрудник ТПУ
    /// </summary>

    [Table("employees")]
    public class Employee
    {
        [Key, Required, MaxLength(50)]
        public string Id { get; set; }

        /// <summary>
        /// Имя сотрудника
        /// </summary>
        [Required, MaxLength(50)]
        public string FirstName { get; set; }

        /// <summary>
        /// Отчество сотрудника
        /// </summary>
        [MaxLength(50)]
        public string MiddleName { get; set; }

        /// <summary>
        /// Фамилия сотрудника
        /// </summary>
        [Required, MaxLength(50)]
        public string LastName { get; set; }

        /// <summary>
        /// Дата рождения
        /// </summary>
        public string Birthdate { get; set; }

        /// <summary>
        /// Дата начала работы в ТПУ
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Правила для сотрудников, заданные данным руководителем
        /// </summary>
        public List<EmployeeRule> EmployeeRules { get; set; } = new List<EmployeeRule>();

        /// <summary>
        /// Правила видимости для подразделений, заданные данным руководителем
        /// </summary>
        public List<VisibilityForDepartment> VisibilityForDepartments { get; set; } = new List<VisibilityForDepartment>();

        /// <summary>
        /// Периоды отпуска, заданные данному сотруднику
        /// </summary>
        public List<SetVacation> SetVacations { get; set; } = new List<SetVacation>();

        /// <summary>
        /// Желаемые периоды отпуска данного сотрудника
        /// </summary>
        public List<WishedVacationPeriod> WishedVacationPeriods { get; set; } = new List<WishedVacationPeriod>();

        /// <summary>
        /// Дни отпуска, причисленные данному сотруднику
        /// </summary>
        public List<VacationDay> VacationDays { get; set; } = new List<VacationDay>();

        /// <summary>
        /// Стили управления, используемые в подразделениях данным руководителем
        /// </summary>
        public List<HeadStyle> HeadStyles { get; set; } = new List<HeadStyle>();

        /// <summary>
        /// Правила видимости для сотрудников, заданные данным руководителем
        /// </summary>
        public List<VisibilityForEmployee> VisibilityHeads { get; set; } = new List<VisibilityForEmployee>();

        /// <summary>
        /// Правила видимости для сотрудников, в которых отображаются
        /// или скрываются отпуска другого сотрудника для данного
        /// </summary>
        public List<VisibilityForEmployee> VisibilityTargets { get; set; } = new List<VisibilityForEmployee>();

        /// <summary>
        /// Правила видимости для сотрудников, в которых отображаются
        /// или скрываются отпуска данного сотрудника
        /// </summary>
        public List<VisibilityForEmployee> VisibilityEmployees { get; set; } = new List<VisibilityForEmployee>();

        /// <summary>
        /// Заместители, назначенные данным руководителем
        /// </summary>
        public List<Deputy> DeputyHeads { get; set; } = new List<Deputy>();

        /// <summary>
        /// Назначения данного сотрудника заместителем руководителя
        /// </summary>
        public List<Deputy> DeputyEmployees { get; set; } = new List<Deputy>();

        /// <summary>
        /// Группы сотрудников, заданные данным руководителем
        /// </summary>
        public List<Group> Groups { get; set; } = new List<Group>();

        /// <summary>
        /// Группы, в которые был включен данный сотрудник
        /// </summary>
        public List<EmployeeInGroup> EmployeeInGroups { get; set; } = new List<EmployeeInGroup>();

        /// <summary>
        /// Выданные данному сотруднику индивидуальные периоды
        /// выбора отпуска
        /// </summary>
        public List<IndividualChoicePeriod> IndividualChoicePeriods { get; set; } = new List<IndividualChoicePeriod>();

        /// <summary>
        /// Должности данного сотрудника в подразделениях
        /// </summary>
        public List<EmployeeInDepartment> EmployeeInDepartments { get; set; } = new List<EmployeeInDepartment>();
    }
}