using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    [NotMapped]
    public class PositionInDepartment
    {
        public string Position { get; set; }
        public string Department { get; set; }
    }
}
