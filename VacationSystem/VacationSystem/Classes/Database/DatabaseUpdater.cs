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
            bool updatingPositions = PositionsUpdater.LoadPositions();
            bool updatingDepartments = DepartmentsUpdater.LoadDepartments();

            return updatingPositions && updatingDepartments;
        }
    }
}