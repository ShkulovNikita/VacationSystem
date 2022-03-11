using System;
using System.Linq;
using System.Diagnostics;
using VacationSystem.Models;

namespace VacationSystem.Classes.Database
{
    /// <summary>
    /// Класс для общих операций с БД
    /// </summary>
    static public class DatabaseHandler
    {
        /// <summary>
        /// Пересоздание БД
        /// </summary>
        static public void RecreateDB()
        {
            using (ApplicationContext db = new ApplicationContext())
                db.RecreateDatabase();
        }

        /// <summary>
        /// Заполнение БД начальными данными справочников
        /// </summary>
        static public void FillInitialData()
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    if (!db.Administrators.Select(a => a.Id).Any())
                        Debug.WriteLine("Добавление профиля администратора: {0}", AddAdmin());
                    if (!db.ManagementStyles.Select(s => s.Id).Any())
                        Debug.WriteLine("Добавление стилей руководства: {0}", AddManagementStyles());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Создание профиля администратора по умолчанию
        /// </summary>
        /// <returns>Успешность выполнения операции</returns>
        static private bool AddAdmin()
        {
            Administrator admin = new Administrator
            {
                Id = "0",
                Login = "admin",
                Password = "admin"
            };

            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    db.Administrators.Add(admin);
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
        /// Заполнение справочника со стилями 
        /// руководства отпусками в подразделениях
        /// </summary>
        /// <returns>Успешность выполнения операции</returns>
        static private bool AddManagementStyles()
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    db.ManagementStyles.Add(new ManagementStyle
                    {
                        Name = "Выбор отпусков руководителем"
                    });
                    db.ManagementStyles.Add(new ManagementStyle
                    {
                        Name = "Выбор отпусков сотрудниками"
                    });
                    db.ManagementStyles.Add(new ManagementStyle
                    {
                        Name = "Выбор сотрудниками желаемых отпусков"
                    });
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