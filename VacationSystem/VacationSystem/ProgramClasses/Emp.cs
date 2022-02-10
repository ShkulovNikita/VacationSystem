namespace VacationSystem.ProgramClasses
{
    public class Emp
    {
        public Emp() { }

        public string Id { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Birthdate { get; set; }

        public DepEmpInfo[] Departments { get; set; }
    }
}
