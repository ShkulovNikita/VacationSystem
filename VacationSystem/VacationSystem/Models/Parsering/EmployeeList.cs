namespace VacationSystem.Models.Parsering
{
    public class EmployeeList
    {
        public EmployeeList() { }

        public string Id { get; set; }

        public EmployeeBrief[] Employees { get; set; }
    }
}
