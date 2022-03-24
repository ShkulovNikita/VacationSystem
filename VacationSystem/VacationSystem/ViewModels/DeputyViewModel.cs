using System.Collections.Generic;
using VacationSystem.Models;

namespace VacationSystem.ViewModels
{
    public class DeputyViewModel
    {
        public DeputyViewModel() { }

        public string Id { get; set; }

        public string FirstName { get ; set; }

        public string MiddleName { get ; set; }

        public string LastName { get; set; }

        public List<Department> Departments { get; set; }
    }
}