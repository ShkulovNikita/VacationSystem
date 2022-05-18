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

        public List<Position> Positions { get; set; } = new List<Position>();

        public List<EmpDepViewModel> Employees { get; set; } = new List<EmpDepViewModel>();

        public string ChosenPosition;
    }
}
