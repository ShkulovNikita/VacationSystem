using System.Collections.Generic;
using VacationSystem.Models;

namespace VacationSystem.ViewModels
{
    public class EditVacationViewModel
    {
        public EditVacationViewModel() { }

        /// <summary>
        /// Идентификатор отпуска
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Сотрудник, отпуск которого редактируется
        /// </summary>
        public Employee Employee { get; set; }

        /// <summary>
        /// Периоды отпуска
        /// </summary>
        public List<VacationDatesViewModel> Dates { get; set; }
    }
}