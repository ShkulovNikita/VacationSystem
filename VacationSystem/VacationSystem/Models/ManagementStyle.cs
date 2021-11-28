using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VacationSystem.Models
{
    public class ManagementStyle
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; }

        public List<HeadStyle> HeadStyles { get; set; } = new List<HeadStyle>();
    }
}
