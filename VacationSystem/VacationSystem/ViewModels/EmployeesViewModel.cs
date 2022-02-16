using System.Collections.Generic;
using VacationSystem.Models;

namespace VacationSystem.ViewModels
{
    /// <summary>
    /// Модель представления для отображения списка сотрудников
    /// (всех или одного подразделения)
    /// </summary>
    public class EmployeesViewModel
    {
        public EmployeesViewModel() { }

        public Department Department { get; set; }

        public List<EmpDepViewModel> Employees { get; set; } = new List<EmpDepViewModel>();
    }
}
