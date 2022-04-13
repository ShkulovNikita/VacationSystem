using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using VacationSystem.Models;

namespace VacationSystem.Classes.Database
{
    /// <summary>
    /// Класс с методами для работы с отпусками в БД
    /// </summary>
    static public class VacationDataHandler
    {
        /// <summary>
        /// Получение всех типов отпусков из БД
        /// </summary>
        /// <returns>Список типов отпусков</returns>
        static public List<VacationType> GetVacationTypes()
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.VacationTypes.ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Получение типа отпуска по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор типа отпуска</param>
        /// <returns>Тип отпуска</returns>
        static public VacationType GetVacationType(int id)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.VacationTypes.FirstOrDefault(vt => vt.Id == id);
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
                        VacationDay sameTypeDays = db.VacationDays
                            .FirstOrDefault(vd => vd.Year == year 
                            && vd.VacationTypeId == type
                            && vd.EmployeeId == empId);
                        // если есть - добавить дни
                        if (sameTypeDays != null)
                            sameTypeDays.NumberOfDays += number;
                        // если нет - создать такую запись
                        else
                        {
                            // определить из типа отпуска, является ли он оплачиваемым
                            bool paid;
                            if (GetVacationType(type).Name == "Отпуск без сохранения заработной платы")
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
    }
}