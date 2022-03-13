using System.Collections.Generic;
using VacationSystem.Models;

namespace VacationSystem.ViewModels
{
    public class EditStyleViewModel
    {
        public EditStyleViewModel() { }

        public Department Department { get; set; }

        public List<ManagementStyle> Styles { get; set; }

        public int CurrentStyle { get; set; }
    }
}
