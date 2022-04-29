using System.Collections.Generic;

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
        /// Периоды отпуска
        /// </summary>
        public List<VacationDatesViewModel> Dates { get; set; }
    }
}