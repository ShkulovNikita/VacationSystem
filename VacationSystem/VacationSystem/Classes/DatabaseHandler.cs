using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VacationSystem.Models;
using VacationSystem.ParsingClasses;

namespace VacationSystem.Classes
{
    /// <summary>
    /// Класс для выполнения различных операций с БД,
    /// связанных с выгрузкой данных из API
    /// </summary>

    static public class DatabaseHandler
    {
        /// <summary>
        /// Получение и загрузка начальных данных в БД
        /// </summary>
        static public void LoadData()
        {
            try
            {
                ClearData();
                FillPositions();
                FillDepartments();
                FillEmployees();
                FillAdministrators();
                FillPositionsInDepartments();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Заполнение таблицы должностей
        /// </summary>
        static public void FillPositions()
        {
            // получить список должностей
            List<Position> positions = ModelConverter.ConvertToPositions(Connector.GetParsedPositionsList());

            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    // заполнить таблицу БД должностей
                    foreach (Position pos in positions)
                        db.Positions.Add(pos);

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Заполнение таблицы отделений
        /// </summary>
        static public void FillDepartments()
        {
            // получить все отделения
            List<Department> departments = ModelConverter.ConvertToDepartments(Connector.GetParsedDepartmentsList());

            try
            {
                // добавление подразделений
                using (ApplicationContext db = new ApplicationContext())
                {
                    // пройтись по списку подразделений
                    foreach (Department dep in departments)
                    {
                        db.Departments.Add(dep);
                    }    

                    db.SaveChanges();
                }

                // добавление связей между подразделениями
                using (ApplicationContext db = new ApplicationContext())
                {
                    // пройтись по всем подразделениям
                    foreach (Department dep in departments)
                    {
                        // получить полную информацию о подразделении
                        Department department = ModelConverter.ConvertToDepartment(Connector.GetParsedDepartment(dep.Id));

                        // получить подразделение из БД
                        Department dp = DataHandler.GetDepartmentById(db, dep.Id);

                        if (department.HeadDepartmentId != null)
                        {
                            // указать старшее подразделение
                            dp.HeadDepartmentId = department.HeadDepartmentId;

                            // сохранить изменение в БД
                            db.Departments.Update(dp);

                            db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Заполнение таблицы сотрудников
        /// </summary>
        static public void FillEmployees()
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    // получить идентификаторы отделений
                    List<string> deps_ids = GetDepartmentsIds(db);

                    // идентификаторы добавляемых сотрудников
                    List<string> emp_ids = new List<string>();

                    // получить и записать информацию о сотрудниках всех подразделений
                    foreach (string depId in deps_ids)
                    {
                        // сотрудники очередного подразделения
                        List<Employee> emps = GetEmployees(depId);

                        // добавление в БД
                        LoadEmployees(emps, emp_ids, db);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Заполнение таблицы должностей сотрудников в подразделениях
        /// </summary>
        static public void FillPositionsInDepartments()
        {
            // список подразделений
            List<DepartmentInfo> departments = Connector.GetParsedDepartmentsList();

            foreach (DepartmentInfo dep in departments)
            {
                // получение сотрудников подразделений
                List<EmployeeInfo> employees = Connector.GetParsedEmployeeList(dep.Id);

                // проход по найденным сотрудникам
                foreach (EmployeeInfo emp in employees)
                {
                    try
                    {
                        using (ApplicationContext db = new ApplicationContext())
                        {
                            // подразделение сотрудника
                            Department department = DataHandler.GetDepartmentById(db, dep.Id);

                            // сотрудник
                            Employee employee = DataHandler.GetEmployeeById(db, emp.Id);

                            // должность сотрудника
                            Position position = DataHandler.GetPositionById(db, emp.Position);

                            // факт руководства
                            bool head = false;
                            if (emp.Head == true)
                                head = true;

                            if ((department != null) && (employee != null) && (position != null))
                            {
                                EmployeeInDepartment empDep = new EmployeeInDepartment
                                {
                                    Employee = employee,
                                    Position = position,
                                    Department = department,
                                    IsHead = head
                                };

                                db.EmployeesInDepartments.Add(empDep);

                                db.SaveChanges();
                            }
                            else
                                continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// Добавление записей о сотрудниках в БД
        /// </summary>
        /// <param name="emps">Сотрудники одного подразделения</param>
        /// <param name="emp_ids">Идентификаторы уже добавленных в БД сотрудников</param>
        static private void LoadEmployees(List<Employee> emps, List<string> emp_ids, ApplicationContext db)
        {
            foreach (Employee emp in emps)
                // если не был добавлен в БД ранее
                if (!emp_ids.Contains(emp.Id))
                {
                    // добавить запись о сотруднике в таблицу сотрудников
                    db.Employees.Add(emp);

                    emp_ids.Add(emp.Id);
                }

            db.SaveChanges();
        }

        /// <summary>
        /// Получение идентификаторов всех подразделений
        /// </summary>
        /// <returns>Список идентификаторов подразделений ТПУ</returns>
        static private List<string> GetDepartmentsIds(ApplicationContext db)
        {
            // список идентификаторов
            List<string> deps_ids = new List<string>();

            // все отделения
            List<Department> deps = new List<Department>();

            deps = db.Departments.ToList();

            // получить все идентификаторы отделений
            foreach (Department dep in deps)
                deps_ids.Add(dep.Id);

            return deps_ids;
        }

        /// <summary>
        /// Получение списка сотрудников подразделения
        /// </summary>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <returns>Список сотрудников указанного подразделения</returns>
        static private List<Employee> GetEmployees(string depId)
        {
            // получение краткой информации о сотрудниках подразделения
            List<EmployeeInfo> employees = Connector.GetParsedEmployeeList(depId);

            // полная информация о сотрудниках подразделения
            List<EmployeeParsed> emps_result = new List<EmployeeParsed>();

            // получить полную информацию по идентификаторам
            foreach(EmployeeInfo emp in employees)
                emps_result.Add(Connector.GetParsedEmployee(emp.Id));

            // конвертировать в класс модели
            List<Employee> result = ModelConverter.ConvertToEmployees(emps_result);

            return result;
        }

        /// <summary>
        /// Создание профиля администратора
        /// </summary>
        static public void FillAdministrators()
        {
            Administrator adm = new Administrator
            {
                Id = "admin",
                Login = "admin",
                Password = "admin"
            };

            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    db.Administrators.Add(adm);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Удаление начальных данных из БД
        /// </summary>
        static private void ClearData()
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    // все сотрудники
                    List<Employee> emps = db.Employees.ToList();

                    // все отделения
                    List<Department> deps = db.Departments.ToList();

                    // все должности
                    List<Position> positions = db.Positions.ToList();

                    // все должности сотрудников в подразделениях
                    List<EmployeeInDepartment> empDeps = db.EmployeesInDepartments.ToList();

                    // все администраторы
                    List<Administrator> admins = db.Administrators.ToList();

                    // удалить все строки
                    foreach (Administrator admin in admins)
                        db.Administrators.Remove(admin);

                    foreach (EmployeeInDepartment empDep in empDeps)
                        db.EmployeesInDepartments.Remove(empDep);

                    foreach (Employee emp in emps)
                        db.Employees.Remove(emp);

                    foreach (Department dep in deps)
                        db.Departments.Remove(dep);

                    foreach (Position pos in positions)
                        db.Positions.Remove(pos);

                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Пересоздание БД
        /// </summary>
        static public void RecreateDB()
        {
            using (ApplicationContext db = new ApplicationContext())
                db.RecreateDatabase();
        }
    }
}