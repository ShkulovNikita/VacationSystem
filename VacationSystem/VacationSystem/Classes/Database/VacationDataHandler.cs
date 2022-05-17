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

                    if (vacation.Periods.Length > 0) 
                        wishedVacation.Year = vacation.Periods[0].StartDate.Year;

                    db.WishedVacationPeriods.Add(wishedVacation);
                    db.SaveChanges();

                    // добавить периоды, составляющие этот отпуск
                    db.VacationParts.AddRange(MakeVacationParts(vacation, wishedVacation.Id));

                    db.SaveChanges();

                    if (!VacationDayDataHandler.TakeVacationDays(db, wishedVacation))
                        return false;
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
        /// Метод для сохранения изменений, внесенных в желаемый отпуск сотрудника
        /// </summary>
        /// <param name="id">Идентификатор отпуска</param>
        /// <param name="vacation">Выбранный период отпуска</param>
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

        /// <summary>
        /// Получить статус отпуска по его имени
        /// </summary>
        /// <param name="status">Наименование статуса отпуска</param>
        /// <returns>Статус отпуска</returns>
        static public VacationStatus GetVacationStatus(string status)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.VacationStatuses.FirstOrDefault(vs => vs.Name == status);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Получить статус отпуска по его имени
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="status">Наименование статуса отпуска</param>
        /// <returns>Статус отпуска</returns>
        static public VacationStatus GetVacationStatus(ApplicationContext db, string status)
        {
            try
            {
                return db.VacationStatuses.FirstOrDefault(vs => vs.Name == status);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Утверждение запланированных отпусков в БД
        /// </summary>
        /// <param name="vacations">Список утверждаемых отпусков</param>
        /// <param name="employees">Список сотрудников с их запланированными отпусками</param>
        /// <returns>Результат выполнения операции</returns>
        static public bool SetVacations(List<SetVacation> vacations, List<Employee> employees, int year)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    // начать транзакцию, чтобы добавить утвержденные отпуска
                    // и удалить запланированные
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            // добавить в БД утверждаемые отпуска
                            db.SetVacations.AddRange(vacations);

                            List<WishedVacationPeriod> periods = new List<WishedVacationPeriod>();
                            // найти все желаемые отпуска текущих сотрудников
                            foreach (Employee employee in employees)
                            {
                                List<WishedVacationPeriod> wishedVacations = GetWishedVacations(employee.Id, year);
                                periods.AddRange(wishedVacations);
                            }

                            /* ------------------------------- */
                            /* РАСКОММЕНТИРОВАТЬ ПОСЛЕ ОТЛАДКИ */
                            /* ------------------------------- */
                            // удалить эти отпуска из БД
                            db.WishedVacationPeriods.RemoveRange(periods);

                            db.SaveChanges();

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex.Message);
                            transaction.Rollback();
                            return false;
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }

        /// <summary>
        /// Удаление запланированного отпуска из БД
        /// </summary>
        /// <param name="vacationId">Идентификатор отпуска</param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool DeleteWishedVacation(int vacationId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    WishedVacationPeriod wishedVacation = GetWishedVacation(vacationId);
                    if (wishedVacation != null)
                    {
                        if (!VacationDayDataHandler.FreeVacationDays(db, wishedVacation))
                            return false;

                        db.WishedVacationPeriods.Remove(wishedVacation);
                        db.SaveChanges();
                    }
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
        /// Получение утвержденного отпуска
        /// </summary>
        /// <param name="vacationId">Идентификатор отпуска</param>
        /// <returns>Утвержденный отпуск</returns>
        static public SetVacation GetSetVacation(int vacationId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.SetVacations.FirstOrDefault(sv => sv.Id == vacationId);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Получение утвержденного отпуска
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="vacationId">Идентификатор отпуска</param>
        /// <returns>Утвержденный отпуск</returns>
        static public SetVacation GetSetVacation(ApplicationContext db, int vacationId)
        {
            try
            {
                return db.SetVacations.FirstOrDefault(sv => sv.Id == vacationId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Прерывание утвержденного отпуска
        /// </summary>
        /// <param name="vacationId">Идентификатор отпуска</param>
        /// <param name="date">Дата прерывания</param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool InterruptVacation(int vacationId, DateTime date)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    SetVacation vacation = GetSetVacation(db, vacationId);

                    // разница между изначальной конечной датой отпуска и датой прерывания
                    int difference = (vacation.EndDate - date).Days;

                    // попробовать освободить занятые отпуском отпускные дни
                    bool result = VacationDayDataHandler.FreeVacationDays(db, vacation, difference);

                    if (!result)
                        return false;

                    // если получилось освободить дни, то поменять статус отпуска
                    VacationStatus status = GetVacationStatus(db, "Прерван");

                    vacation.VacationStatus = status;
                    vacation.VacationStatusId = status.Id;

                    db.SaveChanges();

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