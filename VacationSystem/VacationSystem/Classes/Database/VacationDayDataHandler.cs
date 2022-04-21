using System;
using System.Diagnostics;
using System.Collections.Generic;
using VacationSystem.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace VacationSystem.Classes.Database
{
    /// <summary>
    /// Класс с методами БД для работы с днями отпуска, назначенными сотрудникам
    /// </summary>
    static public class VacationDayDataHandler
    {
        /// <summary>
        /// Получение всех дней отпуска, назначенных сотруднику
        /// </summary>
        /// <param name="id">Идентификатор сотрудника</param>
        /// <returns>Список дней отпуска сотрудника</returns>
        static public List<VacationDay> GetVacationDays(string id)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.VacationDays
                        .Include(vd => vd.VacationType)
                        .Where(vd => vd.EmployeeId == id)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Получение всех доступных отпускных дней сотрудника
        /// </summary>
        /// <param name="empId">Идентификатор сотрудника</param>
        /// <returns>Список отпускных дней сотрудника</returns>
        static public List<VacationDay> GetAvailableVacationDays(string empId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.VacationDays
                        .Where(vd => vd.EmployeeId == empId && vd.NumberOfDays - vd.UsedDays > 0)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Сохранение в БД дней отпуска сотрудников
        /// </summary>
        /// <param name="emps">Список сотрудников, которым были заданы отпускные дни</param>
        /// <param name="type">Тип отпуска</param>
        /// <param name="notes">Примечания руководителя</param>
        /// <param name="number">Количество отпускных дней</param>
        /// <param name="year">Год, на который назначены дни</param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool SetVacationDays(string[] emps, int type, string notes, int number, int year)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    foreach (string empId in emps)
                    {
                        // проверить, есть ли уже у сотрудника на заданный год
                        // дни отпуска по такой же причине
                        VacationDay sameTypeDays = GetEmployeeVacationDays(db, empId, type, year);
                        // если есть - добавить дни
                        if (sameTypeDays != null)
                            sameTypeDays.NumberOfDays += number;
                        // если нет - создать такую запись
                        else
                        {
                            // определить из типа отпуска, является ли он оплачиваемым
                            bool paid;
                            if (VacationDataHandler.GetVacationType(type).Name == "Отпуск без сохранения заработной платы")
                                paid = false;
                            else
                                paid = true;

                            db.VacationDays.Add(new VacationDay
                            {
                                EmployeeId = empId,
                                NumberOfDays = number,
                                UsedDays = 0,
                                Year = year,
                                VacationTypeId = type,
                                Notes = notes,
                                Date = DateTime.Now,
                                Paid = paid
                            });
                        }
                    }

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
        /// Получение отпускных дней сотрудника определенного типа
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="empId">Идентификатор сотрудника</param>
        /// <param name="type">Тип отпускныз дней</param>
        /// <param name="year">Год, на который назначены дни</param>
        /// <returns>Отпускные дни сотрудника заданного типа</returns>
        static private VacationDay GetEmployeeVacationDays(ApplicationContext db, string empId, int type, int year)
        {
            try
            {
                return db.VacationDays
                        .FirstOrDefault(vd => vd.Year == year
                        && vd.VacationTypeId == type
                        && vd.EmployeeId == empId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Удаление отпускных дней сотрудника из БД
        /// </summary>
        /// <param name="emps">Список сотрудников, которым нужно уменьшить количество отпускных дней</param>
        /// <param name="type">Тип отпускных дней</param>
        /// <param name="number">Количество удаляемых дней</param>
        /// <param name="year">Год, на который назначены дни</param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool RemoveVacationDays(string[] emps, int type, int number, int year)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    foreach (string empId in emps)
                    {
                        // проверить, есть ли уже у сотрудника на заданный год
                        // дни отпуска по такой же причине
                        VacationDay sameTypeDays = GetEmployeeVacationDays(db, empId, type, year);
                        // если есть - убрать дни, которые ещё не были использованы
                        if (sameTypeDays != null)
                        {
                            // количество ещё не использованных отпускных дней
                            int remainedDays = sameTypeDays.NumberOfDays - sameTypeDays.UsedDays;
                            if (number > remainedDays)
                                remainedDays = 0;
                            else
                                remainedDays = remainedDays - number;

                            sameTypeDays.NumberOfDays = sameTypeDays.UsedDays + remainedDays;
                        }
                    }

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
