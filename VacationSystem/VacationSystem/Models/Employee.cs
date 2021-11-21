using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace VacationSystem.Models
{
    public class Employee
    {
        [Key, Required, MaxLength(50)]
        public string Id { get; set; }
    }
}
