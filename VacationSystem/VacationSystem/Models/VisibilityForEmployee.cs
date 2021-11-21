using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace VacationSystem.Models
{
    /// <summary>
    /// видимость отпусков одного сотрудника 
    /// другому сотруднику
    /// </summary>
    public class VisibilityForEmployee
    {
        public int Id { get; set; }

        public bool Visibility { get; set; }
    }
}