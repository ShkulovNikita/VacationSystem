namespace VacationSystem.Classes.Database
{
    /// <summary>
    /// Класс для загрузки и обновления данных в БД,
    /// получаемых через API
    /// </summary>
    static public class DatabaseUpdater
    {
        static public bool UpdateDatabase()
        {
            PositionsUpdater.LoadPositions();
            DepartmentsUpdater.LoadDepartments();
            EmployeesUpdater.LoadEmployees();

            return true;

            /*
            bool updatingPositions = PositionsUpdater.LoadPositions();
            bool updatingDepartments = DepartmentsUpdater.LoadDepartments();
            bool updatingEmployees = EmployeesUpdater.LoadEmployees();
            bool updatingHeadDepartments = DepartmentsUpdater.LoadHeadDepartments();
            bool updatingHeadsOfDepartments = DepartmentsUpdater.LoadHeadsOfDepartments();
            bool updatingEmpsInDepartments = EmployeesInDepartmentsUpdater.LoadEmployeesInDepartments();

            return updatingPositions && updatingDepartments && 
                updatingEmployees && updatingHeadDepartments 
                && updatingHeadsOfDepartments && updatingEmpsInDepartments;
            */
        }
    }
}