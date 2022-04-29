using System;

namespace VacationSystem.ViewModels
{
    /// <summary>
    /// Модель представления, используемая для редактирования
    /// выбранных дат отпусков
    /// </summary>
    public class VacationDatesViewModel
    {
        public VacationDatesViewModel() { }

        /// <summary>
        /// Начальная дата периода отпуска
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Конечная дата периода отпуска
        /// </summary>
        public DateTime EndDate { get; set; }
    }
}