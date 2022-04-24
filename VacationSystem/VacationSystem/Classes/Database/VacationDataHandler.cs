using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using VacationSystem.Models;
using VacationSystem.Classes.Data;

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
        /// Сохранение в БД выбранного желаемого периода отпуска
        /// </summary>
        /// <param name="id">Идентификатор сотрудника</param>
        /// <param name="vacation">Выбранный период отпуска</param>
        /// <returns></returns>
        static public bool AddWishedVacation(string id, ChosenVacation vacation)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    // добавить новый желаемый отпуск в БД
                    WishedVacationPeriod wishedVacation = new WishedVacationPeriod
                    {
                        Priority = 1,
                        Date = DateTime.Now,
                        EmployeeId = id
                    };
                    db.WishedVacationPeriods.Add(wishedVacation);
                    db.SaveChanges();

                    // добавить периоды, составляющие этот отпуск
                    for (int i = 0; i < vacation.Periods.Length; i++)
                    {
                        db.VacationParts.Add(new VacationPart
                        {
                            Part = i,
                            StartDate = vacation.Periods[i].StartDate,
                            EndDate = vacation.Periods[i].EndDate,
                            WishedVacationPeriodId = wishedVacation.Id
                        });
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
        /// Получение запланированных отпусков сотрудника
        /// </summary>
        /// <param name="empId">Идентификатор сотрудника</param>
        /// <returns>Список запланированных отпусков сотрудника</returns>
        static public List<WishedVacationPeriod> GetWishedVacations(string empId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.WishedVacationPeriods
                        .Include(wv => wv.VacationParts)
                        .Where(wv => wv.EmployeeId == empId)
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
        /// Получение утвержденных отпусков сотрудника
        /// </summary>
        /// <param name="empId">Идентификатор сотрудника</param>
        /// <returns>Список утвержденных отпусков сотрудника</returns>
        static public List<SetVacation> GetSetVacations(string empId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.SetVacations
                        .Include(sv => sv.VacationStatus)
                        .Where(sv => sv.EmployeeId == empId)
                        .ToList();
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