using System.Collections.Generic;
using VacationSystem.Models;

namespace VacationSystem.ViewModels
{
    public class DepartmentViewModel
    {
        public DepartmentViewModel() { }

        public string Id { get; set; }

        public string Name { get; set; }

        public Employee Head { get; set; }

        public Department HeadDepartment { get; set; }

        public List<Department> ChildDepartments { get; set; } = new List<Department>();
    }
}
