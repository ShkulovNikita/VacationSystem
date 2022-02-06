using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    /// <summary>
    /// Стиль управления конкретного руководителя
    /// в конкретном отделении
    /// </summary>
    
    [Table("heads_styles")]
    public class HeadStyle
    {
        /// <summary>
        /// Дата, в которую был установлен стиль
        /// руководства
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Выбранный стиль руководителя в подразделении
        /// </summary>
        public int ManagementStyleId { get; set; }
        public ManagementStyle ManagementStyle { get; set; }

        /// <summary>
        /// Руководитель, выбравший стиль управления
        /// </summary>
        [Required, MaxLength(50)]
        public string HeadEmployeeId { get; set; }
        public Employee HeadEmployee { get; set; }

        /// <summary>
        /// Подразделение, для которого руководитель
        /// выбрал стиль управления
        /// </summary>
        [Required, MaxLength(50)]
        public string DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}