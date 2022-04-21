using System;

namespace VacationSystem.Classes.Data
{
    /// <summary>
    /// Период дней внутри отпуска сотрудника
    /// </summary>
    public class ChosenPeriod
    {
        public ChosenPeriod() { }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
