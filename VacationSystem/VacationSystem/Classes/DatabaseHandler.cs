using System;
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
            List<Position> positions = ModelConverter.ConvertToPositions(Connector.GetPositionsList());

            // заполнить таблицу БД должностей
            using (ApplicationContext db = new ApplicationContext())
            {
                foreach (Position pos in positions)
                    db.Positions.Add(pos);

                db.SaveChanges();
            }
        }

        /// <summary>
        /// Заполнение таблицы отделений
        /// </summary>
        static public void FillDepartments()
        {
            // получить все отделения
            List<Department> departments = ModelConverter.ConvertToDepartments(Connector.GetDepartmentsList());

            // внести в БД
            using (ApplicationContext db = new ApplicationContext())
            {
                foreach (Department dep in departments)
                    db.Departments.Add(dep);

                db.SaveChanges();
            }
        }

        /// <summary>
        /// Заполнение таблицы сотрудников
        /// </summary>
        static public void FillEmployees()
        {
            // получить идентификаторы отделений
            List<string> deps_ids = GetDepartmentsIds();

            // идентификаторы добавляемых сотрудников
            List<string> emp_ids = new List<string>();

            // получить и записать информацию о сотрудниках всех подразделений
            foreach (string depId in deps_ids)
            {
                // сотрудники очередного подразделения
                List<Employee> emps = GetEmployees(depId);

                // добавление в БД
                LoadEmployees(emps, emp_ids);
            }
        }

        /// <summary>
        /// Добавление записей о сотрудниках в БД
        /// </summary>
        /// <param name="emps">Сотрудники одного подразделения</param>
        /// <param name="emp_ids">Идентификаторы уже добавленных в БД сотрудников</param>
        static private void LoadEmployees(List<Employee> emps, List<string> emp_ids)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                foreach (Employee emp in emps)
                    // если не был добавлен в БД ранее
                    if (!emp_ids.Contains(emp.Id))
                    {
                        db.Employees.Add(emp);
                        emp_ids.Add(emp.Id);
                    }

                db.SaveChanges();
            }
        }

        /// <summary>
        /// Получение идентификаторов всех подразделений
        /// </summary>
        /// <returns>Список идентификаторов подразделений ТПУ</returns>
        static private List<string> GetDepartmentsIds()
        {
            // список идентификаторов
            List<string> deps_ids = new List<string>();

            // все отделения
            List<Department> deps = new List<Department>();

            using (ApplicationContext db = new ApplicationContext())
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
            List<EmployeeInfo> employees = Connector.GetEmployeeList(depId);

            // полная информация о сотрудниках подразделения
            List<EmployeeParsed> emps_result = new List<EmployeeParsed>();

            // получить полную информацию по идентификаторам
            foreach(EmployeeInfo emp in employees)
                emps_result.Add(Connector.GetEmployee(emp.Id));

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

            using (ApplicationContext db = new ApplicationContext())
            {
                db.Administrators.Add(adm);
                db.SaveChanges();
            }
        }

        /// <summary>
        /// Удаление начальных данных из БД
        /// </summary>
        static private void ClearData()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                // все сотрудники
                List<Employee> emps = db.Employees.ToList();

                // все отделения
                List<Department> deps = db.Departments.ToList();

                // все должности
                List<Position> positions = db.Positions.ToList();

                // удалить все строки
                foreach (Employee emp in emps)
                    db.Employees.Remove(emp);

                foreach (Department dep in deps)
                    db.Departments.Remove(dep);

                foreach (Position pos in positions)
                    db.Positions.Remove(pos);

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