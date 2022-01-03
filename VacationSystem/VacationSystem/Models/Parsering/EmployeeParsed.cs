namespace VacationSystem.Models.Parsering
{
    public class EmployeeParsed
    {
        public EmployeeParsed() { }

        public string Id { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Birthdate { get; set; }
        
        public DepartmentParsed[] Departments { get; set; }
    }
}