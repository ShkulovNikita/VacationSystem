﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    /// <summary>
    /// Класс, указывающий должности сотрудников
    /// в подразделениях
    /// </summary>
    
    [Table("employees_in_departments")]
    public class EmployeeInDepartment
    {
        public EmployeeInDepartment() { }

        /// <summary>
        /// Сотрудник
        /// </summary>
        public string EmployeeId { get; set; }
        public Employee Employee { get; set; }

        /// <summary>
        /// Подразделение сотрудника
        /// </summary>
        public string DepartmentId { get; set; }
        public Department Department { get; set; }

        /// <summary>
        /// Должность сотрудника в подразделении
        /// </summary>
        public string PositionId { get; set; }
        public Position Position { get; set; }

        /// <summary>
        /// Факт руководства сотрудником подразделения
        /// </summary>
        [Required]
        public bool IsHead { get; set; }
    }
}