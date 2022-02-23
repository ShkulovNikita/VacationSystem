using System;
using System.Linq;
using System.Collections.Generic;
using VacationSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace VacationSystem.Classes.Database
{
    /// <summary>
    /// Операции с БД по манипуляции данными
    /// </summary>
    static public class DataHandler
    {
        /// <summary>
        /// Найти пользователя системы по его логину
        /// </summary>
        /// <param name="login">Логин пользователя</param>
        /// <returns>Администратор или сотрудник с указанным логином</returns>
        static public object GetUserByLogin(string login)
        {
            Administrator admin = GetAdminByLogin(login);
            if (admin != null)
                return admin;
            else
            {
                Employee emp = GetEmployeeById(login);
                if (emp != null)
                    return emp;
                else
                    return null;
            }
        }

        /// <summary>
        /// Получение администратора по его логину
        /// </summary>
        /// <param name="login">Логин администратора</param>
        /// <returns>Администратор с указанным логином</returns>
        static public Administrator GetAdminByLogin(string login)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    // попытка найти администратора
                    Administrator admin = db.Administrators.FirstOrDefault(adm => adm.Login == login);
                    return admin;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Получение сотрудника ТПУ по его идентификатору
        /// </summary>
        /// <param name="id">Идентификатор сотрудника</param>
        /// <returns>Сотрудник с указанным идентификатором</returns>
        static public Employee GetEmployeeById(string id)
        {
            try
            {
                // попытка найти сотрудника
                using (ApplicationContext db = new ApplicationContext())
                {
                    Employee emp = db.Employees.SingleOrDefault(e => e.Id == id);
                    return emp;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Получение сотрудника ТПУ по его идентификатору
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="id">Идентификатор сотрудника</param>
        /// <returns>Сотрудник с указанным идентификатором</returns>
        static public Employee GetEmployeeById(ApplicationContext db, string id)
        {
            try
            {
                // попытка найти сотрудника
                Employee emp = db.Employees.SingleOrDefault(e => e.Id == id);
                return emp;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Получение должности сотрудника ТПУ по её идентификатору
        /// </summary>
        /// <param name="id">Идентификатор должности</param>
        /// <returns>Должность сотрудника</returns>
        static public Position GetPositionById(string id)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    Position pos = db.Positions.FirstOrDefault(p => p.Id == id);
                    if (pos != null)
                        return pos;
                    else
                        return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Получение должности сотрудника ТПУ по её идентификатору
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="id">Идентификатор должности</param>
        /// <returns>Должность сотрудника</returns>
        static public Position GetPositionById(ApplicationContext db, string id)
        {
            try
            {
                Position pos = db.Positions.FirstOrDefault(p => p.Id == id);
                return pos;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Получить все должности в БД
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <returns>Список должностей</returns>
        static public List<Position> GetPositions(ApplicationContext db)
        {
            try
            {
                List<Position> positions = db.Positions.OrderBy(p => p.Name).ToList();
                return positions;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Получение списка подразделений ТПУ
        /// </summary>
        /// <returns>Список подразделений</returns>
        static public List<Department> GetDepartments()
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.Departments.OrderBy(d => d.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Получение списка подразделений ТПУ
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <returns>Список подразделений</returns>
        static public List<Department> GetDepartments(ApplicationContext db)
        {
            try
            {
                return db.Departments.OrderBy(d => d.Name).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Получение подразделения ТПУ по его идентификатору
        /// </summary>
        /// <param name="id">Идентификатор подразделения</param>
        /// <returns>Подразделение ТПУ</returns>
        static public Department GetDepartmentById(string id)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    Department department = db.Departments.FirstOrDefault(p => p.Id == id);
                    return department;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Получение данных о подразделении ТПУ, включая
        /// подчиненные подразделения, сотрудников, руководителя и старшее
        /// подразделение
        /// </summary>
        /// <param name="id">Идентификатор подразделения</param>
        /// <returns>Подразделение с полными данными</returns>
        static public Department GetFullDepartmentById(string id)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    Department department = db.Departments
                        .Include(d => d.HeadDepartment)
                        .Include(d => d.HeadEmployee)
                        .Include(d => d.ChildDepartments)
                        .FirstOrDefault(d => d.Id == id);
                    return department;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Получение подразделения ТПУ по его идентификатору
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="id">Идентификатор подразделения</param>
        /// <returns>Подразделение ТПУ</returns>
        static public Department GetDepartmentById(ApplicationContext db, string id)
        {
            try
            {
                Department department = db.Departments.FirstOrDefault(p => p.Id == id);
                if (department != null)
                    return department;
                else
                    return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Получение списка сотрудников ТПУ
        /// </summary>
        /// <returns>Список сотрудников ТПУ</returns>
        static public List<Employee> GetEmployees()
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    List<Employee> employees = db.Employees.OrderBy(e => e.LastName)
                                                            .ThenBy(e => e.FirstName)
                                                            .ThenBy(e => e.MiddleName)
                                                            .ToList();
                    return employees;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Получение списка сотрудников подразделения
        /// </summary>
        /// <param name="id">Идентификатор подразделения</param>
        /// <returns>Список сотрудников указанного подразделения</returns>
        static public List<Employee> GetEmployees(string id)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    // получение сотрудников из БД
                    var employees = from dep in db.Departments
                                    where dep.Id == id
                                    join empDep in db.EmployeesInDepartments
                                    on dep.Id equals empDep.DepartmentId
                                    join emp in db.Employees
                                    on empDep.EmployeeId equals emp.Id
                                    select emp;

                    // отсортировать по алфавиту
                    employees = employees.OrderBy(e => e.LastName)
                                         .ThenBy(e => e.FirstName)
                                         .ThenBy(e => e.MiddleName);

                    // удалить дубликаты и преобразовать в список
                    List<Employee> emps_result = DeleteEmployeeDuplicates(employees.ToList());

                    return emps_result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Удалить повторяющихся сотрудников из списка
        /// </summary>
        /// <param name="emps">Оригинальный список сотрудников</param>
        /// <returns>Список сотрудников без дубликатов</returns>
        static public List<Employee> DeleteEmployeeDuplicates(List<Employee> emps)
        {
            // отсортировать список по идентификаторам
            emps = emps.OrderBy(e => e.Id).ToList();

            // список без дубликатов
            List<Employee> result = new List<Employee>();

            // проход по всем сотрудникам из оригинального списка
            for (int i = 0; i < emps.Count; i++)
            {
                // первый сотрудник в любом случае будет добавлен в список без дубликатов
                if (i == 0)
                {
                    result.Add(emps[i]);
                    continue;
                }
                
                // сравнение текущего сотрудника с последним добавленным
                // в список без дубликатов
                if (emps[i].Id != result[result.Count - 1].Id)
                    result.Add(emps[i]);
            }

            if (result.Count > 0)
                return result;
            else
                return null;
        }

        /// <summary>
        /// Получение списка сотрудников ТПУ
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <returns>Список сотрудников ТПУ</returns>
        static public List<Employee> GetEmployees(ApplicationContext db)
        {
            try
            {
                List<Employee> employees = db.Employees.OrderBy(e => e.LastName)
                                                        .ThenBy(e => e.FirstName)
                                                        .ThenBy(e => e.MiddleName)
                                                        .ToList();
                return employees;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Получение списка подразделений сотрудника с его должностями
        /// </summary>
        /// <param name="id">Идентификатор сотрудника</param>
        /// <returns>Список подразделений сотрудника с его должностями в них</returns>
        static public List<EmployeeInDepartment> GetEmployeeDepartments(string id)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    var query = from emp in db.Employees
                                join empDep in db.EmployeesInDepartments
                                on emp.Id equals empDep.EmployeeId
                                where emp.Id == id
                                select empDep;

                    return RemoveEmpInDepDuplicates(query
                        .Include(e => e.Department)
                        .Include(e => e.Position)
                        .OrderBy(e => e.Department.Name)
                        .ToList());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Получить все записи о должностях сотрудников в подразделениях
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <returns>Список должностей сотрудников в подразделениях</returns>
        static public List<EmployeeInDepartment> GetEmployeesInDepartments(ApplicationContext db)
        {
            try
            {
                return db.EmployeesInDepartments.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Получить запись о должности сотрудника в подразделении
        /// </summary>
        /// <param name="empId">Идентификатор сотрудника</param>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <param name="posId">Идентификатор должности</param>
        /// <returns>Должность сотрудника в подразделении</returns>
        static public EmployeeInDepartment GetEmpInDep(ApplicationContext db, string empId, string depId, string posId)
        {
            try
            {
                return db.EmployeesInDepartments
                        .Where(e => e.EmployeeId == empId
                        && e.DepartmentId == depId
                        && e.PositionId == posId)
                        .FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Удалить дубликаты из списка должностей сотрудников в подразделениях
        /// </summary>
        /// <param name="empInDeps">Оригинальный список должностей сотрудников в подразделениях</param>
        /// <returns>Список должностей сотрудников в подразделениях без дубликатов</returns>
        static public List<EmployeeInDepartment> RemoveEmpInDepDuplicates(List<EmployeeInDepartment> empInDeps)
        {
            // список без дубликатов
            List<EmployeeInDepartment> result = new List<EmployeeInDepartment>();

            foreach(EmployeeInDepartment empInDep in empInDeps)
            {
                // поиск, есть ли уже такой элемент в списке без дубликатов
                if (result.Find(r => r.EmployeeId == empInDep.EmployeeId
                                && r.PositionId == empInDep.PositionId
                                && r.DepartmentId == empInDep.DepartmentId) == null)
                    // если нет - добавить новый элемент
                    result.Add(empInDep);
            }

            if (result.Count > 0)
                return result;
            else
                return null;
        }

        /// <summary>
        /// Получение подразделения ТПУ
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="id">Идентификатор подразделения</param>
        /// <returns>Подразделение ТПУ</returns>
        static public async Task<Department> GetDepartment(ApplicationContext db, string id)
        {
            Department department = await db.Departments.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
            return department;
        }

        /// <summary>
        /// Получение списка должностей сотрудника в подразделении
        /// </summary>
        /// <param name="empId">Идентификатор сотрудника</param>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <returns></returns>
        static public List<Position> GetPositionsOfEmployee(string empId, string depId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    var positions = from emp in db.Employees
                                    where emp.Id == empId
                                    join empDep in db.EmployeesInDepartments
                                    on emp.Id equals empDep.EmployeeId
                                    where empDep.DepartmentId == depId
                                    join position in db.Positions
                                    on empDep.PositionId equals position.Id
                                    select position;
                    return positions.OrderBy(p => p.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Получение списка подразделений, которыми 
        /// руководит заданный сотрудник
        /// </summary>
        /// <param name="id">Идентификатор сотрудника (руководителя)</param>
        /// <returns>Список руководимых подразделений</returns>
        static public List<Department> GetSubordinateDepartments(string id)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    var query = from emp in db.Employees
                                where emp.Id == id
                                join dep in db.Departments
                                on emp.Id equals dep.HeadEmployeeId
                                select dep;
                    return query.OrderBy(d => d.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Получение количества записей в БД о выходных/праздничных днях
        /// </summary>
        /// <returns>Количество периодов выходных дней в БД</returns>
        static public int GetHolidaysCount()
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.Holidays.Count();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// Получение количества записей в БД о сотрудниках
        /// </summary>
        /// <returns>Количество сотрудников в БД</returns>
        static public int GetEmployeesCount()
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.Employees.Count();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// Получение количества записей в БД о подразделениях
        /// </summary>
        /// <returns>Количество подразделений в БД</returns>
        static public int GetDepartmentsCount()
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.Departments.Count();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// Получение количества записей в БД о должностях
        /// </summary>
        /// <returns>Количество должностей в БД</returns>
        static public int GetPositionsCount()
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.Positions.Count();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// Получение количества записей в БД о должностях
        /// сотрудников в подразделениях
        /// </summary>
        /// <returns>Количество записей о должностях 
        /// сотрудников в подразделениях</returns>
        static public int GetEmployeesInDepartmentsCount()
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.EmployeesInDepartments.Count();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }
    }
}