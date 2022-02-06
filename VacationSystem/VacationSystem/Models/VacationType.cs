using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    /// <summary>
    /// Тип отпуска, такой как основной 
    /// оплачиваемый отпуск,
    /// ненормированный рабочий день и т.д.
    /// </summary>

    [Table("vacation_types")]
    public class VacationType
    {
        public int Id { get; set; }

        /// <summary>
        /// Название типа отпуска
        /// </summary>
        [Required, MaxLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// Дни отпусков данного типа, полученные сотрудниками
        /// </summary>
        public List<VacationDay> VacationDays { get; set; } = new List<VacationDay>();
    }
}