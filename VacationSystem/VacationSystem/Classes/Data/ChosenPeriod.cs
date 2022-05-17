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

        /// <summary>
        /// Тип периода отпуска: false - запланированный, true - утвержденный
        /// </summary>
        public bool Type { get; set; } = false;
    }
}
