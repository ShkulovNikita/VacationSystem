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
                // добавить недостающие должности
                return AddNewPositions(positionsForUpdate);
        }

        /// <summary>
        /// Добавить должности из API, которых ещё нет в БД
        /// </summary>
        /// <param name="positionsForUpdate">Список должностей из API</param>
        /// <returns>Успешность выполнения операции</returns>
        static private bool AddNewPositions(List<Position> positionsForUpdate)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    // получить все должности в БД
                    List<Position> positionsInDb = DataHandler.GetPositions(db);

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

                    // проверка соответствия имен в БД и API
                    foreach(Position position in positionsForUpdate)
                    {
                        Position pos = DataHandler.GetPositionById(position.Id);
                        if (pos.Name != position.Name)
                            pos.Name = position.Name;
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
                return AddNewDepartments(depsForUpdate);
        }

        /// <summary>
        /// Добавить подразделения, которых ещё нет в БД
        /// </summary>
        /// <param name="depsForUpdate">Список подразделений из API</param>
        /// <returns>Успешность выполнения операции</returns>
        static private bool AddNewDepartments(List<Department> depsForUpdate)
        {
           try
           {
                using (ApplicationContext db = new ApplicationContext())
                {
                    // получить все подразделения в базе данных
                    List<Department> depsInDb = DataHandler.GetDepartments(db);

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

                    // подразделения, которые есть в БД, но нет в API
                    // их нужно "отключить"
                    List<Department> depsForDeleting = depsInDb
                        .Where(d => !depsForUpdate.Any(dep => d.Id == dep.Id))
                        .ToList();

                    // есть подразделения, которые больше не актуальны
                    if (depsForDeleting.Count > 0)
                    {
                       foreach(Department dep in depsForDeleting)
                       {
                            Department deletedDep = DataHandler.GetDepartmentById(db, dep.Id);
                            deletedDep.isActive = false;
                       }

                        db.SaveChanges();
                    }

                    // проверка соответствия имен подразделений в БД
                    // именам, полученным из API
                    foreach(Department dep in depsForUpdate)
                    {
                        Department depInDb = DataHandler.GetDepartmentById(db, dep.Id);
                        if (depInDb.Name != dep.Name)
                            depInDb.Name = dep.Name;
                    }

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


    }
}