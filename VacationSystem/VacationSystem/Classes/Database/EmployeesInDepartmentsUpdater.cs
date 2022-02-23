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
                    List<EmployeeInDepartment> empsInDeps = GetEmployeesInDepartments(db);

                    db.EmployeesInDepartments.AddRange(empsInDeps);

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
        /// Получить список всех должностей сотрудников в подразделениях
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        static private List<EmployeeInDepartment> GetEmployeesInDepartments(ApplicationContext db)
        {
            // получить все подразделения из БД
            List<Department> departments = DataHandler.GetDepartments(db);

            List<EmployeeInDepartment> empsInDeps = new List<EmployeeInDepartment>();

            // для каждого подразделения получить списки должностей сотрудников
            // в этих подразделениях
            foreach (Department dep in departments)
            {
                List<EmployeeInDepartment> positions = ModelConverter.ConvertToPositionsInDepartments(dep.Id, Connector.GetParsedEmployeeList(dep.Id));
                positions = DataHandler.RemoveEmpInDepDuplicates(positions);

                if (positions != null)
                    empsInDeps.AddRange(positions);
            }

            return empsInDeps;
        }

        /// <summary>
        /// Обновить данные о должностях сотрудников в подразделениях
        /// </summary>
        /// <returns>Успешность выполнения операции</returns>
        static private bool UpdateEmpsInDeps()
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    // список должностей в подразделениях из API
                    List<EmployeeInDepartment> empsInDepsForUpdate = GetEmployeesInDepartments(db);

                    // проверка, удалось получить список из API
                    if (empsInDepsForUpdate.Count == 0)
                        return false;
                    else
                    {
                        // список должностей в подразделениях из БД
                        List<EmployeeInDepartment> empsInDepsInDb = DataHandler.GetEmployeesInDepartments(db);

                        // добавить в БД записи, которые есть в API
                        bool adding = AddNewEmpsInDeps(db, empsInDepsForUpdate, empsInDepsInDb);

                        // активировать те записи, которые снова появились в API
                        bool activating = ActivateEmpsInDeps(db, empsInDepsForUpdate, empsInDepsInDb);

                        // деактивировать те записи, которые больше не появляются в API
                        bool deactivating = DeactivateEmpsInDeps(db, empsInDepsForUpdate, empsInDepsInDb);

                        return adding && activating && deactivating;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Добавление в БД тех записей о должностях в подразделениях,
        /// которых нет в БД, но есть в API
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="empsForUpdate">Список записей из API</param>
        /// <param name="empsInDb">Список записей из БД</param>
        /// <returns>Успешность выполнения операции</returns>
        static private bool AddNewEmpsInDeps(ApplicationContext db, List<EmployeeInDepartment> empsForUpdate, List<EmployeeInDepartment> empsInDb)
        {
            try
            {
                // записи, которые есть в API, но нет в БД
                List<EmployeeInDepartment> newRecords = empsForUpdate
                    .Where(forUpdate => 
                    !empsInDb.Any(inDb => 
                           forUpdate.EmployeeId == inDb.EmployeeId &&
                           forUpdate.DepartmentId == inDb.DepartmentId &&
                           forUpdate.PositionId == inDb.PositionId))
                    .ToList();

                if (newRecords.Count > 0)
                {
                    db.EmployeesInDepartments.AddRange(newRecords);
                    db.SaveChanges();
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Активировать те записи, которые снова появились в API
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="empsForUpdate">Список записей из API</param>
        /// <param name="empsInDb">Список записей из БД</param>
        /// <returns>Успешность выполнения операции</returns>
        static private bool ActivateEmpsInDeps(ApplicationContext db, List<EmployeeInDepartment> empsForUpdate, List<EmployeeInDepartment> empsInDb)
        {
            try
            {
                // записи, которые есть в ответе API, но отключены в БД
                List<EmployeeInDepartment> notActiveRecords = empsForUpdate
                    .Where(forUpdate => empsInDb.Any(inDb =>
                    forUpdate.EmployeeId == inDb.EmployeeId &&
                    forUpdate.DepartmentId == inDb.DepartmentId &&
                    forUpdate.PositionId == inDb.PositionId &&
                    inDb.IsActive == false))
                    .ToList();

                // сделать активными
                foreach (EmployeeInDepartment emp in notActiveRecords)
                {
                    EmployeeInDepartment empInDb = DataHandler.GetEmpInDep(db, emp.EmployeeId, emp.DepartmentId, emp.PositionId);
                    if (empInDb != null)
                        empInDb.IsActive = true;
                }
                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Деактивировать те записи, которые больше не появляются в API
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="empsForUpdate">Список записей из API</param>
        /// <param name="empsInDb">Список записей из БД</param>
        /// <returns>Успешность выполнения операции</returns>
        static private bool DeactivateEmpsInDeps(ApplicationContext db, List<EmployeeInDepartment> empsForUpdate, List<EmployeeInDepartment> empsInDb)
        {
            try
            {
                List<EmployeeInDepartment> recordsForDeleting = empsInDb
                    .Where(inDb => !empsForUpdate.Any(forUpdate =>
                    inDb.EmployeeId == forUpdate.EmployeeId
                    && inDb.DepartmentId == forUpdate.DepartmentId
                    && inDb.PositionId == forUpdate.PositionId))
                    .ToList();

                // проход по записям, которые больше не активны
                foreach (EmployeeInDepartment emp in recordsForDeleting)
                {
                    EmployeeInDepartment empInDb = DataHandler.GetEmpInDep(db, emp.EmployeeId, emp.DepartmentId, emp.PositionId);
                    empInDb.IsActive = false;
                }

                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
