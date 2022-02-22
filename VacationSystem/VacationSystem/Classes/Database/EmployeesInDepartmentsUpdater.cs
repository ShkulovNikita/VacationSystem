using System;
using System.Linq;
using System.Collections.Generic;
using VacationSystem.Models;

namespace VacationSystem.Classes.Database
{
    /// <summary>
    /// Класс для загрузки и обновления данных в БД
    /// о должностях сотрудников в подразделениях, получаемых через API
    /// </summary>
    static public class EmployeesInDepartmentsUpdater
    {
        /// <summary>
        /// Загрузить из API данные о должностях сотрудников
        /// в подразделениях
        /// </summary>
        /// <returns>Успешность выполнения операции</returns>
        static public bool LoadEmployeesInDepartments()
        {
            if (DataHandler.GetEmployeesInDepartmentsCount() == 0)
                return FillEmpsInDeps();
            else
                return UpdateEmpsInDeps();
        }

        /// <summary>
        /// Заполнить таблицу БД с должностями сотрудников
        /// в подразделениях
        /// </summary>
        /// <returns>Успешность выполнения операции</returns>
        static private bool FillEmpsInDeps()
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    // получить все подразделения из БД
                    List<Department> departments = DataHandler.GetDepartments(db);

                    // для каждого подразделения получить списки должностей сотрудников
                    // в этих подразделениях
                    foreach (Department dep in departments)
                    {
                        List<EmployeeInDepartment> positions = ModelConverter
                            .ConvertToPositionsInDepartments(dep.Id, Connector.GetParsedEmployeeList(dep.Id));

                        if (positions != null)
                            db.EmployeesInDepartments.AddRange(positions);
                    }

                    db.SaveChanges();

                    if (DataHandler.GetEmployeesInDepartmentsCount() > 0)
                        return true;
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Обновить данные о должностях сотрудников в подразделениях
        /// </summary>
        /// <returns>Успешность выполнения операции</returns>
        static private bool UpdateEmpsInDeps()
        {
            return true;
        }
    }
}
