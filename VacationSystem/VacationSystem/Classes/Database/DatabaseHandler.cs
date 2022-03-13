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
                        Name = "Выбор отпусков руководителем",
                        Description = "Все отпуска расставляются руководителем или его заместителями вручную"
                    });
                    db.ManagementStyles.Add(new ManagementStyle
                    {
                        Name = "Выбор отпусков сотрудниками",
                        Description = "Отпуска выбираются сотрудниками в соответствии с имеющимися свободными днями, руководитель утверждает выбранные отпуска"
                    });
                    db.ManagementStyles.Add(new ManagementStyle
                    {
                        Name = "Выбор сотрудниками желаемых отпусков",
                        Description = "Сотрудники оставляют пожелания в виде нескольких возможных периодов отпусков, руководитель их учитывает при расстановке отпусков"
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