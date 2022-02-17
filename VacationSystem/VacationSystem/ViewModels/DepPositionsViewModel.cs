using VacationSystem.Models;
using System.Collections.Generic;

namespace VacationSystem.ViewModels
{
    /// <summary>
    /// Модель представления, хранящая информацию о должностях
    /// сотрудника в подразделении
    /// </summary>
    public class DepPositionsViewModel
    {
        public DepPositionsViewModel() { }

        public Department Department { get; set; }

        public List<Position> Positions { get; set; } = new List<Position>();
    }
}
