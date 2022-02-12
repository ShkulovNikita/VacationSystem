using VacationSystem.Models;

namespace VacationSystem.ProgramClasses
{
    /// <summary>
    /// Класс для хранения пар значений подразделение-должность
    /// </summary>
    public class DepartmentPosition
    {
        public DepartmentPosition() { }
        public Dep Department { get; set; }
        public Position Position { get; set; }
    }
}