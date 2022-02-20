using System;
using System.Linq;
using System.Collections.Generic;
using VacationSystem.Models;

namespace VacationSystem.Classes
{
    static public class DatabaseUpdater
    {
        /* ---------------------------- */
        /* Загрузка данных о должностях */
        /* ---------------------------- */

        /// <summary>
        /// Загрузить из API данные о должностях
        /// </summary>
        /// <returns>Успешность выполнения операции</returns>
        static public bool LoadPositions()
        {
            if (DataHandler.GetPositionsCount() == 0)
                return FillPositions();
            else
                return UpdatePositions();
        }

        /// <summary>
        /// Заполнение таблицы с должностями
        /// </summary>
        /// <returns>Успешность выполнения операции</returns>
        static private bool FillPositions()
        {
            // получить должности из API
            List<Position> positions = ModelConverter.ConvertToPositions(Connector.GetParsedPositionsList());

            // должности были успешно получены
            if (positions == null)
                return false;
            else
                try
                {
                    using (ApplicationContext db = new ApplicationContext())
                    {
                        db.Positions.AddRange(positions);
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
        /// Обновление данных о должностях
        /// </summary>
        /// <returns>Успешность выполнения операции</returns>
        static private bool UpdatePositions()
        {
            // получить из API новые данные о должностях
            List<Position> positionsForUpdate = ModelConverter.ConvertToPositions(Connector.GetParsedPositionsList());

            // проверка получения данных из API
            if (positionsForUpdate == null)
                return false;
            else
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    // получить все должности в БД
                    List<Position> positionsInDb = DataHandler.GetPositions(db);
                    if (positionsInDb == null)
                        return false;
                    else
                    {
                        // добавить новые должности в БД
                        bool addingNewPositions = AddNewPositions(db, positionsForUpdate, positionsInDb);

                        // обновить названия должностей
                        bool renamingPositions = RenamePositions(db, positionsForUpdate);

                        return addingNewPositions && renamingPositions;
                    }
                }
            }
        }

        /// <summary>
        /// Добавить должности из API, которых ещё нет в БД
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="positionsForUpdate">Список должностей из API</param>
        /// <param name="positionsInDb">Список должностей в БД</param>
        /// <returns>Успешность выполнения операции</returns>
        static private bool AddNewPositions(ApplicationContext db, List<Position> positionsForUpdate, List<Position> positionsInDb)
        {
            try
            {
                // должности из ответа API, которых ещё нет в БД
                List<Position> newPositions = positionsForUpdate
                    .Where(p => !positionsInDb.Any(pos => pos.Id == p.Id))
                    .ToList();

                // добавить недостающие должности в БД
                if (newPositions.Count > 0)
                {
                    db.Positions.AddRange(newPositions);
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
        /// Обновить имена должностей, которые не совпадают в БД
        /// и в ответе от API
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="positionsForUpdate">Список должностей из API</param>
        /// <returns>Успешность выполнения операции</returns>
        static private bool RenamePositions(ApplicationContext db, List<Position> positionsForUpdate)
        {
            try
            {
                // проверка соответствия имен в БД и API
                foreach (Position position in positionsForUpdate)
                {
                    Position pos = DataHandler.GetPositionById(position.Id);
                    if (pos.Name != position.Name)
                        pos.Name = position.Name;
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

        /* -------------------------------- */
        /* Загрузка данных о подразделениях */
        /* -------------------------------- */

        /// <summary>
        /// Загрузить из API данные о подразделениях
        /// </summary>
        /// <returns>Успешность выполнения операции</returns>
        static public bool LoadDepartments()
        {
            if (DataHandler.GetDepartmentsCount() == 0)
                return FillDepartments();
            else
                return UpdateDepartments();
        }

        /// <summary>
        /// Заполнение таблицы с подразделениями
        /// </summary>
        /// <returns>Успешность выполнения операции</returns>
        static private bool FillDepartments()
        {
            // получить подразделения из API
            List<Department> departments = ModelConverter.ConvertToDepartments(Connector.GetParsedDepartmentsList(), true);

            // успешность получения должностей
            if (departments == null)
                return false;
            else
                try
                {
                    using (ApplicationContext db = new ApplicationContext())
                    {
                        db.Departments.AddRange(departments);
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
        /// Обновить данные о подразделениях
        /// </summary>
        /// <returns>Успешность выполнения операции</returns>
        static private bool UpdateDepartments()
        {
            // получить данные о подразделениях из API
            List<Department> depsForUpdate = ModelConverter.ConvertToDepartments(Connector.GetParsedDepartmentsList(), true);

            // проверка получения данных из API
            if (depsForUpdate == null)
                return false;
            else
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    // получить список подразделений из БД
                    List<Department> depsInDb = DataHandler.GetDepartments(db);
                    if (depsInDb == null)
                        return false;
                    else
                    {
                        // добавить в БД подразделения,
                        // которые есть в ответе API, но нет в БД
                        bool addingNewDeps = AddNewDepartments(db, depsForUpdate, depsInDb);

                        // активировать подразделения, 
                        // отключенные ранее, но появившиеся
                        // в ответе от API снова
                        bool activatingDeps = ActivateDepartments(db, depsForUpdate, depsInDb);

                        // отключение подразделений, которые
                        // есть в БД, но больше не появляются в
                        // ответе от API
                        bool deactivatingDeps = DeactivateDepartments(db, depsForUpdate, depsInDb);

                        // переименовать подразделения в БД,
                        // которые теперь имеют другое имя в API
                        bool renamingDeps = RenameDepartments(db, depsForUpdate);

                        // если все операции выполнились успешно - вернуть true, иначе false
                        return addingNewDeps && activatingDeps && deactivatingDeps && renamingDeps;
                    }
                }
            }
        }

        /// <summary>
        /// Добавить подразделения, которых ещё нет в БД
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="depsForUpdate">Список подразделений из API</param>
        /// <param name="depsInDb">Список подразделений из БД</param>
        /// <returns>Успешность выполнения операции</returns>
        static private bool AddNewDepartments(ApplicationContext db, List<Department> depsForUpdate, List<Department> depsInDb)
        {
           try
           {
                // подразделения из ответа API, которых ещё нет в БД
                List<Department> newDepartments = depsForUpdate
                    .Where(d => !depsInDb.Any(dep => d.Id == dep.Id))
                    .ToList();

                // добавить недостающие подразделения в БД
                if (newDepartments.Count > 0)
                {
                    db.Departments.AddRange(newDepartments);
                    db.SaveChanges();
                }

                return true;
           }
           catch(Exception ex)
           {
               Console.WriteLine(ex.Message);
               return false;
           }
        }

        /// <summary>
        /// Переключение статуса активности тем подразделениям, которые
        /// ранее были отключены, но снова появились в ответе API
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="depsForUpdate">Список подразделений из API</param>
        /// <param name="depsInDb">Список подразделений из БД</param>
        /// <returns>Успешность выполнения операции</returns>
        static private bool ActivateDepartments(ApplicationContext db, List<Department> depsForUpdate, List<Department> depsInDb)
        {
            try
            {
                // подразделения из ответа API, которые уже есть в БД, но "отключены"
                List<Department> notActiveDeps = depsForUpdate
                    .Where(d => depsInDb.Any(dep => d.Id == dep.Id && dep.isActive == false))
                    .ToList();
                // сделать их активными
                foreach (Department dep in notActiveDeps)
                {
                    Department depForActive = DataHandler.GetDepartmentById(db, dep.Id);
                    depForActive.isActive = true;
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
        /// "Отключение" подразделений, которые больше
        /// не появляются в ответах от API
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="depsForUpdate">Список подразделений из API</param>
        /// <param name="depsInDb">Список подразделений из БД</param>
        /// <returns>Успешность выполнения операции</returns>
        static private bool DeactivateDepartments(ApplicationContext db, List<Department> depsForUpdate, List<Department> depsInDb)
        {
            try
            {
                // подразделения, которые есть в БД, но нет в ответе API
                List<Department> depsForDeleting = depsInDb
                    .Where(d => !depsForUpdate.Any(dep => d.Id == dep.Id))
                    .ToList();

                // есть подразделения, которые больше не актуальны
                if (depsForDeleting.Count > 0)
                {
                    foreach (Department dep in depsForDeleting)
                    {
                        Department deletedDep = DataHandler.GetDepartmentById(db, dep.Id);
                        deletedDep.isActive = false;
                    }

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
        /// Переименование подразделений, имена в БД которых "устарели"
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="depsForUpdate">Список подразделений из API</param>
        /// <returns>Успешность выполнения операции</returns>
        static private bool RenameDepartments(ApplicationContext db, List<Department> depsForUpdate)
        {
            try
            {
                // проверка соответствия имен подразделений в БД
                // именам, полученным из API
                foreach (Department dep in depsForUpdate)
                {
                    Department depInDb = DataHandler.GetDepartmentById(db, dep.Id);
                    // имена не совпали - поменять имя в БД
                    if (depInDb.Name != dep.Name)
                        depInDb.Name = dep.Name;
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

        /* ----------------------------- */
        /* Загрузка данных о сотрудниках */
        /* ----------------------------- */

        /// <summary>
        /// Загрузить из API данные о сотрудниках
        /// </summary>
        /// <returns></returns>
        static public bool LoadEmployees()
        {

        }
    }
}