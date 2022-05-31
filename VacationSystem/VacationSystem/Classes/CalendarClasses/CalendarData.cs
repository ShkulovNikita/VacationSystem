using VacationSystem.Models;

namespace VacationSystem.Classes.CalendarClasses
{
    public class CalendarData
    {
        public CalendarData() { }

        public string DepartmentId { get; set; }

        public string DepartmentName { get; set; }

        public int Year { get; set; }

        public CalendarEmployee[] Employees { get; set; }
    }
}
