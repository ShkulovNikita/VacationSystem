using System;
using System.Collections.Generic;
using VacationSystem.Models;

namespace VacationSystem.ViewModels
{
    public class CalendarViewModel
    {
        public CalendarViewModel() { }

        /// <summary>
        /// Просматриваемое подразделение
        /// </summary>
        public Department Department { get; set; }

        /// <summary>
        /// Год отпусков
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Начальная дата календаря
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Конечная дата календаря
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Тип отпусков: запланированные и утвержденные
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Выбранный тип отпусков
        /// </summary>
        public string CurrentType { get; set; }

        /// <summary>
        /// Список с данными об отпусках сотрудников
        /// </summary>
        public List<EmpVacationViewModel> Vacations { get; set; } = new List<EmpVacationViewModel>();
    }
}
