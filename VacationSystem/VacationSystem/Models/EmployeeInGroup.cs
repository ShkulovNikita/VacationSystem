using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    /// <summary>
    /// Добавление сотрудника в группу
    /// </summary>
    
    [Table("employees_in_groups")]
    public class EmployeeInGroup
    {
        /// <summary>
        /// Сотрудник, добавляемый в группу
        /// </summary>
        [Required, MaxLength(50)]
        public string EmployeeId { get; set; }
        public Employee Employee { get; set; }

        /// <summary>
        /// Группа, в которую добавляется сотрудник
        /// </summary>
        [Required]
        public int GroupId { get; set; }
        public Group Group { get; set; }

        /// <summary>
        /// Дата добавления сотрудника в группу
        /// </summary>
        public DateTime Date { get; set; }
    }
}