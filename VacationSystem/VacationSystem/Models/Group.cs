using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    /// <summary>
    /// Группа сотрудников, создаваемая
    /// руководителем для применения 
    /// правил отпусков
    /// </summary>

    [Table("groups")]
    public class Group
    {
        public int Id { get; set; }

        /// <summary>
        /// Наименование группы
        /// </summary>
        [MaxLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// Дата создания группы
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Описание группы
        /// </summary>
        [MaxLength(500)]
        public string Description { get; set; }

        /// <summary>
        /// Руководитель, создавший группу
        /// </summary>
        [Required, MaxLength(50)]
        public string HeadEmployeeId { get; set; }
        public Employee HeadEmployee { get; set; }

        /// <summary>
        /// Подразделение группы сотрудников
        /// </summary>
        [Required, MaxLength(50)]
        public string DepartmentId { get; set; }
        public Department Department { get; set; }

        /// <summary>
        /// Записи о сотрудниках в группе
        /// </summary>
        public List<EmployeeInGroup> EmployeesInGroup { get; set; } = new List<EmployeeInGroup>();
    }
}