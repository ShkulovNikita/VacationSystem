using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    /// <summary>
    /// Стиль управления отпусками
    /// </summary>
    
    [Table("management_styles")]
    public class ManagementStyle
    {
        public int Id { get; set; }

        /// <summary>
        /// Название стиля управления
        /// </summary>
        [Required, MaxLength(200)]
        public string Name { get; set; }

        /// <summary>
        /// Стиль управления, используемый руководителями в отделениях
        /// </summary>
        public List<HeadStyle> HeadStyles { get; set; } = new List<HeadStyle>();
    }
}
