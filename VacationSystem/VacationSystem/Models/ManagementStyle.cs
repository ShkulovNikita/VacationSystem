using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace VacationSystem.Models
{
    public class ManagementStyle
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; }
    }
}
