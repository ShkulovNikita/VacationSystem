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
                    db.VacationParts.AddRange(MakeVacationParts(vacation, wishedVacation.Id));

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
        /// Генерация периодов отпуска в необходимом для БД формате
        /// </summary>
        /// <param name="id">Идентификатор отпуска</param>
        /// <returns>Список периодов отпусков в формате модели</returns>
        static private List<VacationPart> MakeVacationParts(ChosenVacation vacation, int id)
        {
            List<VacationPart> result = new List<VacationPart>();

            // добавить периоды, составляющие этот отпуск
            for (int i = 0; i < vacation.Periods.Length; i++)
            {
                result.Add(new VacationPart
                {
                    Part = i,
                    StartDate = vacation.Periods[i].StartDate,
                    EndDate = vacation.Periods[i].EndDate,
                    WishedVacationPeriodId = id
                });
            }

            return result;
        }

        /// <summary>
        /// Метод для сохранения изменений, внесенных в 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="vacation"></param>
        /// <returns></returns>
        static public bool EditWishedVacation(int id, ChosenVacation vacation)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    // очистить периоды, связанные с редактируемым отпуском
                    List<VacationPart> oldParts = db.VacationParts.Where(vp => vp.WishedVacationPeriodId == id).ToList();
                    db.VacationParts.RemoveRange(oldParts);
                    db.SaveChanges();

                    // создать новые периоды
                    db.VacationParts.AddRange(MakeVacationParts(vacation, id));

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
        /// Получение запланированных отпусков сотрудника
        /// </summary>
        /// <param name="empId">Идентификатор сотрудника</param>
        /// <param name="year">Год, на который назначены отпуска</param>
        /// <returns>Список запланированных отпусков сотрудника</returns>
        static public List<WishedVacationPeriod> GetWishedVacations(string empId, int year)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.WishedVacationPeriods
                        .Include(wv => wv.VacationParts)
                        .Where(wv => wv.EmployeeId == empId
                        && wv.Year == year)
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

        /// <summary>
        /// Получение утвержденных отпусков сотрудника
        /// </summary>
        /// <param name="empId">Идентификатор сотрудника</param>
        /// <param name="year">Год, на который назначен отпуск</param>
        /// <returns>Список утвержденных отпусков сотрудника</returns>
        static public List<SetVacation> GetSetVacations(string empId, int year)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.SetVacations
                        .Include(sv => sv.VacationStatus)
                        .Where(sv => sv.EmployeeId == empId
                        && sv.EndDate.Year == year)
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
        /// Получение статуса отпуска из справочника
        /// </summary>
        /// <param name="id">Идентификатор статуса</param>
        /// <returns>Статус отпуска</returns>
        static public VacationStatus GetVacationStatus(int id)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.VacationStatuses.FirstOrDefault(vs => vs.Id == id);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Получение из БД заданного запланированного отпуска
        /// </summary>
        /// <param name="id">Идентификатор отпуска</param>
        /// <returns>Запланированный отпуск с его частями</returns>
        static public WishedVacationPeriod GetWishedVacation(int id)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.WishedVacationPeriods
                        .Include(wv => wv.VacationParts)
                        .FirstOrDefault(wv => wv.Id == id);
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