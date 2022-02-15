﻿using System;
using System.Collections.Generic;
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
                LoadDepartments(departments);

                // добавление связей между подразделениями
                LoadHeadDepartments(departments);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Загрузка всех подразделений в базу данных
        /// </summary>
        /// <param name="departments">Список подразделений для добавления</param>
        static private void LoadDepartments(List<Department> departments)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                // пройтись по списку подразделений
                foreach (Department dep in departments)
                    db.Departments.Add(dep);

                db.SaveChanges();
            }
        }

        /// <summary>
        /// Загрузка связей между подразделениями в базу данных
        /// </summary>
        /// <param name="departments">Список подразделений, которым добавляется старшее подразделение</param>
        static private void LoadHeadDepartments(List<Department> departments)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                // добавление связи со старшим подразделением
                // для всех подразделений
                foreach (Department dep in departments)
                    AddHeadDepartment(db, dep);
            }
        }

        /// <summary>
        /// Добавление одной связи подразделение -> старшее подразделение
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="dep">Подразделение, для которого добавляется
        /// старшее подразделение</param>
        static private void AddHeadDepartment(ApplicationContext db, Department dep)
        {
            // получить полную информацию о подразделении
            Department department = ModelConverter.ConvertToDepartment(Connector.GetParsedDepartment(dep.Id));

            // получить подразделение из БД
            Department dp = DataHandler.GetDepartmentById(db, dep.Id);

            if ((department.HeadDepartmentId != "null") && (department.HeadDepartmentId != dp.Id))
            {
                // указать старшее подразделение
                dp.HeadDepartment = DataHandler.GetDepartmentById(db, department.HeadDepartmentId);

                // сохранить изменение в БД
                db.Departments.Update(dp);

                db.SaveChanges();
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
        /// Заполнение таблицы должностей сотрудников в подразделениях,
        /// а также указание руководителей подразделений
        /// </summary>
        static public void FillPositionsInDepartments()
        {
            // список подразделений
            List<DepartmentInfo> departments = Connector.GetParsedDepartmentsList();

            // заполнить данные о должностях сотрудников в каждом подразделении
            foreach (DepartmentInfo dep in departments)
                LoadPositionsInDepartment(dep.Id);

            // заполнить данные о главах подразделений
            foreach (DepartmentInfo dep in departments)
                LoadHeadOfDepartment(dep.Id);
        }

        /// <summary>
        /// Задание руководителя подразделения
        /// </summary>
        /// <param name="depId">Идентификатор подразделения</param>
        static public void LoadHeadOfDepartment(string depId)
        {
            // получить данные о подразделении из API
            Department departmentFromAPI = ModelConverter.ConvertToDepartment(Connector.GetParsedDepartment(depId));

            // у подразделения есть руководитель
            if (departmentFromAPI.HeadEmployeeId != "")
                using (ApplicationContext db = new ApplicationContext())
                {
                    // получение подразделения, для которого задается глава
                    Department department = DataHandler.GetDepartmentById(db, depId);

                    // получение сотрудника-главы подразделения
                    Employee head = DataHandler.GetEmployeeById(db, departmentFromAPI.HeadEmployeeId);

                    // задание руководителя подразделению
                    department.HeadEmployee = head;

                    db.Departments.Update(department);

                    db.SaveChanges();
                }
        }

        /// <summary>
        /// Добавление записей о должностях сотрудников в указанном подразделении
        /// </summary>
        /// <param name="depId">Идентификатор подразделения</param>
        static private void LoadPositionsInDepartment(string depId)
        {
            // получение сотрудников подразделений
            List<EmployeeInfo> employees = Connector.GetParsedEmployeeList(depId);

            // проход по найденным сотрудникам
            foreach (EmployeeInfo emp in employees)
                try
                {
                    LoadEmployeePositions(depId, emp);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    continue;
                }
        }

        /// <summary>
        /// Загрузить в БД данные о должностях указанного 
        /// сотрудника в указанном подразделении
        /// </summary>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <param name="emp">Данные о сотруднике подразделения из API</param>
        static private void LoadEmployeePositions(string depId, EmployeeInfo emp)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                // подразделение сотрудника
                Department department = DataHandler.GetDepartmentById(db, depId);

                // сотрудник
                Employee employee = DataHandler.GetEmployeeById(db, emp.Id);

                // должность сотрудника
                Position position = DataHandler.GetPositionById(db, emp.Position);

                if ((department != null) && (employee != null) && (position != null))
                {
                    EmployeeInDepartment empDep = new EmployeeInDepartment
                    {
                        Employee = employee,
                        Position = position,
                        Department = department
                    };

                    db.EmployeesInDepartments.Add(empDep);

                    db.SaveChanges();
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
                ClearEmployeesInDepartments();
                ClearAdministrators();
                ClearDepartments();
                ClearEmployees();
                ClearPositions();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Удалить все подразделения
        /// </summary>
        static private void ClearDepartments()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                List<Department> deps = db.Departments.ToList();
                db.Departments.RemoveRange(deps);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Удалить всех сотрудников
        /// </summary>
        static private void ClearEmployees()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                List<Employee> emps = db.Employees.ToList();
                db.Employees.RemoveRange(emps);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Удалить все должности
        /// </summary>
        static private void ClearPositions()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                List<Position> positions = db.Positions.ToList();
                db.Positions.RemoveRange(positions);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Удалить всех администраторов
        /// </summary>
        static private void ClearAdministrators()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                List<Administrator> admins = db.Administrators.ToList();
                db.Administrators.RemoveRange(admins);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Удалить все должности сотрудников в подразделениях
        /// </summary>
        static private void ClearEmployeesInDepartments()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                List<EmployeeInDepartment> empDeps = db.EmployeesInDepartments.ToList();
                db.EmployeesInDepartments.RemoveRange(empDeps);
                db.SaveChanges();
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