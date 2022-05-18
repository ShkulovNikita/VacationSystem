using System;
using System.Linq;
using System.Diagnostics;
using VacationSystem.Models;

namespace VacationSystem.Classes.Database
{
    /// <summary>
    /// Статический класс для работы в БД с уведомлениями
    /// </summary>
    static public class NotificationDataHandler
    {
        /// <summary>
        /// Получение уведомления
        /// </summary>
        /// <param name="id">Идентификатор уведомления</param>
        /// <returns>Уведомление для пользователя</returns>
        static public Notification GetNotification(int id)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.Notifications.FirstOrDefault(x => x.Id == id);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Получение типа уведомления по его наименованию
        /// </summary>
        /// <param name="text">Наименование типа уведомления</param>
        /// <returns>Тип уведомления с заданным именем</returns>
        static public NotificationType GetNotificationType(string text)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.NotificationTypes.FirstOrDefault(nt => nt.Name == text);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Получение типа уведомления по его наименованию
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="text">Наименование типа уведомления</param>
        /// <returns>Тип уведомления с заданным именем</returns>
        static public NotificationType GetNotificationType(ApplicationContext db, string text)
        {
            try
            {
                return db.NotificationTypes.FirstOrDefault(nt => nt.Name == text);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Сохранить в БД уведомление
        /// </summary>
        /// <param name="text">Текст уведомления</param>
        /// <param name="empId">Идентификатор сотрудника, которому должно прийти уведомление</param>
        /// <param name="type">Тип уведомления</param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool AddNotification(string text, string empId, string type)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    NotificationType notifType = GetNotificationType(db, type);

                    Notification notification = new Notification
                    {
                        Text = text,
                        EmployeeId = empId,
                        NotificationType = notifType,
                        NotificationTypeId = notifType.Id,
                        Date = DateTime.Now
                    };

                    db.Notifications.Add(notification);

                    db.SaveChanges();
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Удалить уведомление
        /// </summary>
        /// <param name="id">Идентификатор уведомления</param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool DeleteNotification(int id)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    Notification notification = db.Notifications.FirstOrDefault(n => n.Id == id);
                    if (notification == null)
                        return false;
                    db.Notifications.Remove(notification);
                    db.SaveChanges();
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
