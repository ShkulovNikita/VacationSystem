using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    [Table("management_styles")]
    public class ManagementStyle
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; }

        public List<HeadStyle> HeadStyles { get; set; } = new List<HeadStyle>();
    }
}
