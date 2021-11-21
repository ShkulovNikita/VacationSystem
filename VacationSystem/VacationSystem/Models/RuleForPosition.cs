using System.ComponentModel.DataAnnotations;

namespace VacationSystem.Models
{
    /// <summary>
    /// Правила, устанавливаемые на должности
    /// сотрудников в отделениях
    /// </summary>
    public class RuleForPosition
    {
        public int Id { get; set; }

        // количество сотрудников определенной должности,
        // которые должны находиться одновременно на рабочем месте
        public int PeopleNumber { get; set; }

        [Required, MaxLength(50)]
        public string PositionId { get; set; }
        public Position Position { get; set; }

        [Required, MaxLength(50)]
        public string DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}
