using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace VacationSystem.Models
{
    /// <summary>
    /// Видимость отпусков других людей
    /// </summary>
    public class VisibilityForDepartment
    {
        public int Id { get; set; }

        [Required]
        public bool IsVisible { get; set; }
    }
}
