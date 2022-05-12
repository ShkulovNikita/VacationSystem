using System.Collections.Generic;
using VacationSystem.Models;

namespace VacationSystem.ViewModels
{
    /// <summary>
    /// Модель представления для списка отпусков сотрудника
    /// </summary>
    public class VacationIndexViewModel
    {
        public VacationIndexViewModel () { }

        /// <summary>
        /// Сотрудник, отпуска которого отображаются
        /// </summary>
        public Employee Employee { get; set; }

        /// <summary>
        /// Список отпусков сотрудника
        /// </summary>
        public List<VacationViewModel> Vacations { get; set; } = new List<VacationViewModel>();
    }
}
