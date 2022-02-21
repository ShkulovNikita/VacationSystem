using System;
using System.Linq;
using System.Collections.Generic;
using VacationSystem.Models;

namespace VacationSystem.Classes.Database
{
    /// <summary>
    /// Класс для загрузки и обновления данных в БД о
    /// сотрудниках, получаемых из API
    /// </summary>
    static public class EmployeesUpdater
    {
        /// <summary>
        /// Загрузить из API данные о сотрудниках
        /// </summary>
        /// <returns>Успешность выполнения операции</returns>
        static public bool LoadEmployees()
        {
            if (DataHandler.GetEmployeesCount() == 0)
                return FillEmployees();
            else
                return UpdateEmployees();
        }

        /// <summary>
        /// Загрузить данные о сотрудниках в БД из API
        /// </summary>
        /// <returns>Успешность выполнения операции</returns>
        static private bool FillEmployees()
        {
            try
            {
                using(ApplicationContext db = new ApplicationContext())
                {
                    // получить список идентификаторов подразделений в БД
                    List<string> depIds = DataHandler.GetDepartments(db).Select(d => d.Id).ToList();
                    if (depIds == null)
                        return false;
                    if (depIds.Count == 0)
                        return false;

                    // получить сотрудников всех подразделений
                    List<Employee> employees = GetEmployees(depIds);
                    if (employees.Count == 0)
                        return false;

                    // добавить сотрудников в БД
                    db.Employees.AddRange(employees);
                    db.SaveChanges();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Получение списка сотрудников из API
        /// </summary>
        /// <param name="depIds">Список идентификаторов подразделений в БД</param>
        /// <returns>Список сотрудников из API</returns>
        static private List<Employee> GetEmployees(List<string> depIds)
        {
            List<Employee> result = new List<Employee>();

            // проход по всем подразделениям
            // с целью получения их сотрудников
            foreach(string depId in depIds)
                result = AddEmployeesFromDepartment(result, depId);

            return result;
        }

        /// <summary>
        /// Добавление сотрудников подразделения к общему списку сотрудников
        /// </summary>
        /// <param name="empList">Список, в который необходимо добавить новых сотрудников</param>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <returns>Список сотрудников с добавленными сотрудниками из указанного подразделения</returns>
        static private List<Employee> AddEmployeesFromDepartment(List<Employee> empList, string depId)
        {
            // сотрудники подразделения
            List<Employee> employees = ModelConverter.ConvertToEmployees(Connector.GetParsedEmployeeList(depId));
            if (employees != null)
            {
                // добавить тех сотрудников, которых ещё нет в списке result
                List<Employee> newEmps = employees
                    .Where(e => !empList.Any(emp => e.Id == emp.Id))
                    .ToList();
                empList.AddRange(newEmps);
            }
            return empList;
        }

        /// <summary>
        /// Обновить имеющиеся в БД данные о сотрудниках
        /// </summary>
        /// <returns>Успешность выполнения операции</returns>
        static private bool UpdateEmployees()
        {
            try
            {
                List<string> depIds = new List<string>();

                // идентификаторы всех подразделений
                using (ApplicationContext db = new ApplicationContext())
                {
                    depIds = DataHandler.GetDepartments(db).Select(d => d.Id).ToList();
                }

                if (depIds == null)
                    return false;
                if (depIds.Count == 0)
                    return false;

                // список сотрудников из API
                List<Employee> empsForUpdate = new List<Employee>();

                // получить всех сотрудников из API
                foreach(string depId in depIds)
                    empsForUpdate = AddEmployeesFromDepartment(empsForUpdate, depId);

                if (empsForUpdate.Count == 0)
                    return false;
                else
                {
                    using (ApplicationContext db = new ApplicationContext())
                    {
                        // получить список сотрудников из БД
                        List<Employee> empsInDb = DataHandler.GetEmployees(db);

                        // добавить в БД сотрудников, которые есть 
                        // в ответе от API, но нет в БД
                        bool addingNewEmployees = AddNewEmployees(db, empsForUpdate, empsInDb);

                        // включить отключенных сотрудников, которые
                        // появились в ответе от API
                        bool activatingEmployees = ActivateEmployees(db, empsForUpdate, empsInDb);

                        // отключение сотрудников, которые больше
                        // не появляются в ответах API
                        bool deactivatingEmployees = DeactivateEmployees(db, empsForUpdate, empsInDb);

                        return addingNewEmployees && activatingEmployees && deactivatingEmployees;
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
        /// Добавление в БД сотрудников, которых нет в БД, но есть в API
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="empsForUpdate">Список сотрудников из API</param>
        /// <param name="empsInDb">Список сотрудников из БД</param>
        /// <returns>Успешность выполнения операции</returns>
        static private bool AddNewEmployees(ApplicationContext db, List<Employee> empsForUpdate, List<Employee> empsInDb)
        {
            try
            {
                // сотрудники, которые есть в API, но нет в БД
                List<Employee> newEmps = empsForUpdate
                    .Where(e => !empsInDb.Any(emp => e.Id == emp.Id))
                    .ToList();

                if (newEmps.Count > 0)
                {
                    db.Employees.AddRange(newEmps);
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
        /// Переключение статуса активности тем сотрудникам, которые
        /// ранее были отключены, но снова появились в ответе API 
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="empsForUpdate">Список сотрудников из API</param>
        /// <param name="empsInDb">Список сотрудников из БД</param>
        /// <returns>Успешность выполнения операции</returns>
        static private bool ActivateEmployees(ApplicationContext db, List<Employee> empsForUpdate, List<Employee> empsInDb)
        {
            try
            {
                // подразделения, которые есть в ответе API, но отключены в БД
                List<Employee> notActiveEmps = empsForUpdate
                    .Where(e => empsInDb.Any(emp => e.Id == emp.Id && emp.IsActive == false))
                    .ToList();
                // сделать активными
                foreach (Employee emp in notActiveEmps)
                {
                    Employee empForActive = DataHandler.GetEmployeeById(db, emp.Id);
                    empForActive.IsActive = true;
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
        /// "Отключение" сотрудников, которые больше
        /// не появляются в ответах от API
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="empsForUpdate">Список сотрудников из API</param>
        /// <param name="empsInDb">Список сотрудников из БД</param>
        /// <returns>Успешность выполнения операции</returns>
        static private bool DeactivateEmployees(ApplicationContext db, List<Employee> empsForUpdate, List<Employee> empsInDb)
        {
            try
            {
                // сотрудники, которые есть в БД, но отсутствуют в ответе API
                List<Employee> empsForDeleting = empsInDb
                    .Where(e => !empsForUpdate.Any(emp => e.Id == emp.Id))
                    .ToList();

                // проход по сотрудникам, которые больше не активны
                if (empsForDeleting.Count > 0)
                {
                    foreach (Employee emp in empsForDeleting)
                    {
                        Employee deletedEmp = DataHandler.GetEmployeeById(db, emp.Id);
                        deletedEmp.IsActive = false;
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
        /// Обновить данные у сотрудников, у которых они не совпадают
        /// с данными из API
        /// </summary>
        /// <param name="db"></param>
        /// <param name="empsForUpdate"></param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool RenameEmployees(ApplicationContext db, List<Employee> empsForUpdate)
        {
            try
            {
                // проверка соответствия данных о сотрудниках в БД и API
                foreach (Employee emp in empsForUpdate)
                {
                    Employee empInDb = DataHandler.GetEmployeeById(db, emp.Id);
                    // если что-то не совпадает - поменять
                    // имя
                    if (empInDb.FirstName != emp.FirstName)
                        empInDb.FirstName = emp.FirstName;
                    // отчество
                    if (empInDb.LastName != emp.LastName)
                        empInDb.LastName = emp.LastName;
                    // фамилия
                    if (empInDb.MiddleName != emp.MiddleName)
                        empInDb.MiddleName = emp.MiddleName;
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