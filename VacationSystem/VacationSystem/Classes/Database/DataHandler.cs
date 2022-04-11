using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using VacationSystem.Models;

namespace VacationSystem.Classes.Database
{
    /// <summary>
    /// Класс для работы с данными в БД
    /// </summary>
    static public class DataHandler
    {
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
                    Administrator admin = db.Administrators.FirstOrDefault(a => a.Login == login);
                    return admin;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Получение всех типов правил выбора отпусков в БД
        /// </summary>
        /// <returns>Список всех типов правил</returns>
        static public List<RuleType> GetRuleTypes()
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.RuleTypes.ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }
    }
}