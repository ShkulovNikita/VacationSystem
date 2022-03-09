using System.ComponentModel.DataAnnotations.Schema;
using VacationSystem.Models;

namespace VacationSystem.ApiClasses
{
    [NotMapped]
    public class PositionsInDepartments
    {
        public PositionInDepartment[] Positions { get; set; }
    }
}
