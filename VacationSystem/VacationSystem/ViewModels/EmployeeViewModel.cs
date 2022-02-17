using VacationSystem.Models;
using System.Collections.Generic;

namespace VacationSystem.ViewModels
{
    /// <summary>
    /// Модель представления для отображения информации о сотруднике
    /// </summary>
    public class EmployeeViewModel
    {
        public EmployeeViewModel() { }

        public string Id { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }
        
        public string LastName { get; set; }

        /// <summary>
        /// Должности сотрудника в подразделениях
        /// </summary>
        public List<DepPositionsViewModel> PositionsInDepartments { get; set; } = new List<DepPositionsViewModel>();

        /// <summary>
        /// Подразделения, которыми руководит данный сотрудник
        /// </summary>
        public List<Department> SubordinateDepartments { get; set; } = new List<Department>();
    }
}
