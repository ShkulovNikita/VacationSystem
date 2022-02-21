using System;
using System.Linq;
using System.Collections.Generic;
using VacationSystem.Models;

namespace VacationSystem.Classes.Database
{
    /// <summary>
    /// Класс для загрузки и обновления данных в БД
    /// о подразделениях, получаемых через API
    /// </summary>
    static public class DepartmentsUpdater
    {
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
            catch (Exception ex)
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
                    .Where(d => depsInDb.Any(dep => d.Id == dep.Id && dep.IsActive == false))
                    .ToList();
                // сделать их активными
                foreach (Department dep in notActiveDeps)
                {
                    Department depForActive = DataHandler.GetDepartmentById(db, dep.Id);
                    depForActive.IsActive = true;
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
                        deletedDep.IsActive = false;
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

        /// <summary>
        /// Загрузить данные о старших подразделениях
        /// </summary>
        /// <returns></returns>
        static public bool LoadHeadDepartments()
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    // список подразделений в БД
                    List<Department> depsInDb = DataHandler.GetDepartments(db);

                    if (depsInDb == null)
                        return false;

                    // список подразделений из API
                    List<Department> depsInApi = new List<Department>();
                    foreach(Department dep in depsInDb)
                        depsInApi.Add(ModelConverter.ConvertToDepartment(Connector.GetParsedDepartment(dep.Id)));

                    if (depsInApi.Count == 0)
                        return false;
                    else
                        return UpdateHeadDepartments(db, depsInDb, depsInApi);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Обновить данные о старших подразделениях
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="depsInDb">Список подразделений из БД</param>
        /// <param name="depsInApi">Список подразделений из API</param>
        /// <returns>Успешность выполнения операции</returns>
        static private bool UpdateHeadDepartments(ApplicationContext db, List<Department> depsInDb, List<Department> depsInApi)
        {
            try
            {
                // получить подразделения, для которых не совпадают данные
                // по старшим подразделениям
                List<Department> depForUpdate = depsInDb
                    .Where(depInDb => depsInApi.Any(depInApi => depInApi.Id == depInDb.Id
                    && depInApi.HeadDepartmentId != depInDb.HeadDepartmentId))
                    .ToList();

                // пройтись по подразделениям и обновить данные
                foreach(Department dep in depForUpdate)
                {
                    // получить подразделение из БД
                    Department depInDb = DataHandler.GetDepartmentById(db, dep.Id);

                    // новое значение старшего подразделения
                    string headDep = null;

                    // соответствующее подразделение из API
                    Department depInApi = depsInApi.Where(d => d.Id == depInDb.Id).FirstOrDefault();

                    // получить новое значение старшего подразделения, если оно не пустое
                    if (depInApi != null)
                        if ((depInApi.HeadDepartmentId != "null") && (depInApi.HeadDepartmentId != null) && (depInApi.HeadDepartmentId != ""))
                            headDep = depInApi.HeadDepartmentId;

                    depInDb.HeadDepartmentId = headDep;
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
        /// Удалить записи о старших подразделениях в БД для тех
        /// подразделений, у которых в API больше нет старших 
        /// подразделений
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="depsInDb">Список подразделений из БД</param>
        /// <param name="depsInApi">Список подразделений из API</param>
        /// <returns>Успешность выполнения операции</returns>
        static private bool RemoveHeadDepartments(ApplicationContext db, List<Department> depsInDb, List<Department> depsInApi)
        {
            try
            {
                // найти те подразделения в БД, у которых значение
                // старшего подразделения в API равно null,
                // а в БД - не null
                List<Department> depsForDeleteHead = depsInDb
                    .Where(depInDb => depsInApi.Any(depInApi => depInDb.Id == depInApi.Id 
                                                                && (depInApi.HeadDepartmentId == "null"
                                                                || depInApi.HeadDepartmentId == null
                                                                || depInApi.HeadDepartmentId == "")
                                                                && depInDb.HeadDepartmentId != null))
                    .ToList();

                // пройтись по ним и обнулить их старшие подразделения
                foreach (Department dep in depsForDeleteHead)
                {
                    Department depInDb = DataHandler.GetDepartmentById(db, dep.Id);
                    if (depInDb != null)
                        depInDb.HeadDepartmentId = null;
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
        /// Добавить данные о старших подразделениях
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="depsInDb">Список подразделений из БД</param>
        /// <param name="depsInApi">Список подразделений из API</param>
        /// <returns>Успешность выполнения операции</returns>
        static private bool AddHeadDepartments(ApplicationContext db, List<Department> depsInDb, List<Department> depsInApi)
        {
            try
            {
                // получить те старшие подразделения из БД, которые есть в ответе API
                // но не имеют старшего подразделения в БД
                List<Department> depsForAddingHead = depsInDb
                    .Where(depInDb => depsInApi.Any(depInApi => depInDb.Id == depInApi.Id
                                                                && (depInApi.HeadDepartmentId != "null"
                                                                && depInApi.HeadDepartmentId != null
                                                                && depInApi.HeadDepartmentId != "")
                                                                && depInDb.HeadDepartmentId == null))
                    .ToList();

                // для всех таких подразделений - добавить старшее подразделение
                foreach (Department dep in depsForAddingHead)
                {
                    Department depFromDb = DataHandler.GetDepartmentById(db, dep.Id);
                    depFromDb.HeadDepartmentId = depsInApi.Where(d => d.Id == dep.Id).FirstOrDefault().HeadDepartmentId;
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
        /// Загрузить данные о руководителях подразделений
        /// </summary>
        /// <returns>Успешность выполнения операции</returns>
        static public bool LoadHeadsOfDepartments()
        {
            try
            {
                using(ApplicationContext db = new ApplicationContext())
                {
                    // список подразделений в БД
                    List<Department> depsInDb = DataHandler.GetDepartments(db);

                    if (depsInDb == null)
                        return false;

                    // список подразделений из API
                    List<Department> depsInApi = new List<Department>();
                    foreach (Department dep in depsInDb)
                        depsInApi.Add(ModelConverter.ConvertToDepartment(Connector.GetParsedDepartment(dep.Id)));

                    if (depsInApi.Count == 0)
                        return false;
                    else
                        return UpdateHeadsOfDepartments(db, depsInDb, depsInApi);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Обновить данные в БД о главах подразделений в соответствии
        /// с данными из API
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="depsInDb">Список подразделений из БД</param>
        /// <param name="depsInApi">Список подразделений из API</param>
        /// <returns>Успешность выполнения операции</returns>
        static private bool UpdateHeadsOfDepartments(ApplicationContext db, List<Department> depsInDb, List<Department> depsInApi)
        {
            try
            {
                // получить те подразделения, у которых не совпадают данные
                // по руководителям в БД и в API
                List<Department> depForChange = depsInDb
                    .Where(depInDb => depsInApi.Any(depInApi => depInApi.Id == depInDb.Id
                    && depInApi.HeadEmployeeId != depInDb.HeadEmployeeId))
                    .ToList();

                // пройтись по этим подразделениям и обновить данные
                foreach(Department dep in depForChange)
                {
                    // получить подразделение из БД
                    Department depInDb = DataHandler.GetDepartmentById(db, dep.Id);

                    // новое значение руководителя подразделения
                    string headOfDep = null;

                    // соответствующее подразделение из API
                    Department depInApi = depsInApi.Where(d => d.Id == depInDb.Id).FirstOrDefault();
                    
                    // получить новое значение руководителя, если оно не пустое
                    if (depInApi != null)
                        if ((depInApi.HeadEmployeeId != "null") && (depInApi.HeadEmployeeId != null) && (depInApi.HeadEmployeeId != ""))
                            headOfDep = depInApi.HeadEmployeeId;

                    depInDb.HeadEmployeeId = headOfDep;
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