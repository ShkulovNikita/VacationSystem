using VacationSystem.Models;
using System.Collections.Generic;

namespace VacationSystem.ViewModels
{
    /// <summary>
    /// Модель представления для хранения данных
    /// об одном сотруднике подразделения и его
    /// должностях в этом подразделении
    /// </summary>
    public class EmpDepViewModel
    {
        public EmpDepViewModel() { }

        public EmpDepViewModel(Employee emp) 
        {
            Id = emp.Id;
            FirstName = emp.FirstName;
            MiddleName = emp.MiddleName;
            LastName = emp.LastName;
        }

        public string Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public List<Position> Positions { get; set; } = new List<Position>();
    }
}
