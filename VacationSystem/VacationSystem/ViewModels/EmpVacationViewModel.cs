using System.Collections.Generic;
using VacationSystem.Models;

namespace VacationSystem.ViewModels
{
    /// <summary>
    /// Модель представления с данными об отпусках сотрудника для 
    /// календаря отпусков
    /// </summary>
    public class EmpVacationViewModel
    {
        /// <summary>
        /// Идентификатор сотрудника
        /// </summary>
        public string EmployeeId { get; set; }

        /// <summary>
        /// Текущее подразделение
        /// </summary>
        public Department Department { get; set; }

        /// <summary>
        /// ФИО сотрудника
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Отпуска сотрудника
        /// </summary>
        public List<VacationViewModel> Vacations { get; set; }
    }
}
