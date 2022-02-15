using System.Collections.Generic;
using VacationSystem.Models;

namespace VacationSystem.ViewModels
{
    public class EmployeesViewModel
    {
        public EmployeesViewModel() { }

        public Department Department { get; set; }

        public List<Employee> Employees { get; set; } = new List<Employee>();
    }
}
