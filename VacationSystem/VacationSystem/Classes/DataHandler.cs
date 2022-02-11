using System;
using System.Linq;
using VacationSystem.Models;

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
    }
}