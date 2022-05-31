namespace VacationSystem.Classes.CalendarClasses
{
    public class CalendarEmployee
    {
        public CalendarEmployee () { }

        public string Id { get; set; }

        public string Name { get; set; }

        public VacationPeriod[] VacationPeriods { get; set; }
    }
}
