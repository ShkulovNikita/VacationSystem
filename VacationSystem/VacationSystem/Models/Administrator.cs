using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    [Table("administrators")]
    public class Administrator
    {
        [Key, Required, MaxLength(50)]
        public string Id { get; set; }

        [Required, MaxLength(50)]
        public string Login { get; set; }

        [Required, MaxLength(50)]
        public string Password { get; set; }
    }
}