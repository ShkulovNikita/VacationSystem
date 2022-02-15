using System;
using System.Linq;
using System.Collections.Generic;
using VacationSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace VacationSystem.Classes
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
        static public Object GetUserByLogin(string login)
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
                if (pos != null)
                    return pos;
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
        /// Получение списка подразделений ТПУ
        /// </summary>
        /// <returns>Список подразделений</returns>
        static public List<Department> GetDepartments()
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    List<Department> departments = db.Departments.OrderBy(d => d.Name).ToList();
                    if (departments != null)
                        return departments;
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
        /// подчиненные подразделения, руководителя и старшее
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
                    if (employees != null)
                        return employees;
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

                    return query.ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
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
    }
}