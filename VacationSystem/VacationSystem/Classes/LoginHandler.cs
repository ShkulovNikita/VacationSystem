using System;
using System.Diagnostics;
using VacationSystem.Models;
using VacationSystem.Classes.Database;

namespace VacationSystem.Classes
{
    /// <summary>
    /// Класс для работы с авторизацией пользователей
    /// </summary>
    static public class LoginHandler
    {
        /// <summary>
        /// Получение пользователя системы по его логину
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        static public object GetUser(string login)
        {
            // проверить, есть ли такой пользователь-администратор
            Administrator admin = DataHandler.GetAdminByLogin(login);

            // если найден такой администратор - вернуть его
            if (admin != null)
                return admin;
            else
            {
                // попробовать получить пользователя с таким логином
                Employee emp = Connector.GetEmployee(login);
                return null;
            }
        }
    }
}
