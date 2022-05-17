using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
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
                    if (!db.RuleTypes.Select(t => t.Id).Any())
                        Debug.WriteLine("Добавление типов правил: {0}", AddRuleTypes());
                    if (!db.VacationTypes.Select(v => v.Id).Any())
                        Debug.WriteLine("Добавление типов отпусков: {0}", AddVacationTypes());
                    if (!db.VacationStatuses.Select(v => v.Id).Any())
                        Debug.WriteLine("Добавление статусов отпусков: {0}", AddVacationStatuses());
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

        /// <summary>
        /// Заполнение справочника с типами правил выбора отпусков
        /// </summary>
        /// <returns>Успешность выполнения операции</returns>
        static private bool AddRuleTypes()
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    db.RuleTypes.Add(new RuleType
                    {
                        Name = "Должны уходить в отпуск одновременно"
                    });
                    db.RuleTypes.Add(new RuleType
                    {
                        Name = "Не должны уходить в отпуск одновременно"
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

        /// <summary>
        /// Заполнение справочника с типами отпусков
        /// </summary>
        /// <returns>Успешность выполнения операции</returns>
        static private bool AddVacationTypes()
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    db.VacationTypes.Add(new VacationType
                    {
                        Name = "Основной оплачиваемый отпуск"
                    });
                    db.VacationTypes.Add(new VacationType
                    {
                        Name = "Дополнительный оплачиваемый отпуск (ненормированный рабочий день)"
                    });
                    db.VacationTypes.Add(new VacationType
                    {
                        Name = "Дополнительный оплачиваемый отпуск (вредные/опасные условия труда)"
                    });
                    db.VacationTypes.Add(new VacationType
                    {
                        Name = "Дополнительный оплачиваемый отпуск (особый характер работы)"
                    });
                    db.VacationTypes.Add(new VacationType
                    {
                        Name = "Дополнительный оплачиваемый отпуск (другое)"
                    });
                    db.VacationTypes.Add(new VacationType
                    {
                        Name = "Отпуск без сохранения заработной платы"
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

        /// <summary>
        /// Заполнение справочника со статусами отпусков
        /// </summary>
        /// <returns>Успешность выполнения операции</returns>
        static private bool AddVacationStatuses()
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    db.VacationStatuses.Add(new VacationStatus
                    {
                        Name = "Утвержден"
                    });
                    db.VacationStatuses.Add(new VacationStatus
                    {
                        Name = "В процессе"
                    });
                    db.VacationStatuses.Add(new VacationStatus
                    {
                        Name = "Завершен"
                    });
                    db.VacationStatuses.Add(new VacationStatus
                    {
                        Name = "Согласован"
                    });
                    db.VacationStatuses.Add(new VacationStatus
                    {
                        Name = "Отменен"
                    });
                    db.VacationStatuses.Add(new VacationStatus
                    {
                        Name = "Продлен"
                    });
                    db.VacationStatuses.Add(new VacationStatus
                    {
                        Name = "Разделен"
                    });
                    db.VacationStatuses.Add(new VacationStatus
                    {
                        Name = "Перенесен"
                    });
                    db.VacationStatuses.Add(new VacationStatus
                    {
                        Name = "Прерван"
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

        /// <summary>
        /// Отладочный метод для удаления всех записей из таблицы утвержденных отпусков
        /// </summary>
        /// <returns>Успешность выполнения операции</returns>
        static public bool ClearSetVacations()
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    List<SetVacation> vacations = db.SetVacations.ToList();
                    db.SetVacations.RemoveRange(vacations);
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