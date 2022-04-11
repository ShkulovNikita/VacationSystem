using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using VacationSystem.Models;

namespace VacationSystem.Classes.Database
{
    /// <summary>
    /// Класс с методами работы с БД, связанными с 
    /// заместителями/секретарями руководителя подразделения
    /// </summary>
    static public class DeputyDataHandler
    {
        /// <summary>
        /// Получение списка заместителей руководителя
        /// </summary>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <returns>Список заместителей указанного руководителя</returns>
        static public List<Deputy> GetDeputies(string headId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.Deputies.Where(d => d.HeadEmployeeId == headId).ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Получение списка заместителей руководителя
        /// </summary>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <returns>Список заместителей руководителя в указанном подразделении</returns>
        static public List<Deputy> GetDeputies(string headId, string depId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.Deputies.Where(d => d.HeadEmployeeId == headId && d.DepartmentId == depId).ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Добавление заместителя руководителя в подразделении
        /// </summary>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <param name="empId">Идентификатор сотрудника-заместителя</param>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool AddDeputy(string headId, string empId, string depId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    Deputy deputy = new Deputy
                    {
                        HeadEmployeeId = headId,
                        DeputyEmployeeId = empId,
                        DepartmentId = depId,
                        Date = DateTime.Now
                    };

                    db.Deputies.Add(deputy);
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
        /// Проверка существования в БД заместителя
        /// </summary>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <param name="empId">Идентификатор сотрудника-заместителя</param>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <returns>true - такой заместитель уже есть; false - такого заместителя в БД нет</returns>
        static public bool CheckDeputy(string headId, string empId, string depId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    if (db.Deputies.Any(d => d.DepartmentId == depId
                                        && d.DeputyEmployeeId == empId
                                        && d.HeadEmployeeId == headId))
                        return true;
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Проверка существования в БД заместителя
        /// </summary>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <param name="empId">Идентификатор сотрудника-заместителя</param>
        /// <returns>true - такой заместитель уже есть; false - такого заместителя в БД нет</returns>
        static public bool CheckDeputy(string headId, string empId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    if (db.Deputies.Any(d => d.DeputyEmployeeId == empId
                                          && d.HeadEmployeeId == headId))
                        return true;
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Удаление заместителя руководителя
        /// </summary>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <param name="empId">Идентификатор сотрудника-заместителя</param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool DeleteDeputy(string headId, string empId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    if (CheckDeputy(headId, empId))
                    {
                        List<Deputy> deputyForDelete = db.Deputies.Where(d => d.HeadEmployeeId == headId
                                                                          && d.DeputyEmployeeId == empId)
                                                                  .ToList();
                        db.Deputies.RemoveRange(deputyForDelete);
                        db.SaveChanges();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
