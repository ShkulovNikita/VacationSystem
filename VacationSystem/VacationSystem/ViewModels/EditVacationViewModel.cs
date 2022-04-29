using System.Collections.Generic;

namespace VacationSystem.ViewModels
{
    public class EditVacationViewModel
    {
        public EditVacationViewModel() { }

        public int Id { get; set; }

        public List<VacationDatesViewModel> Dates { get; set; }
    }
}