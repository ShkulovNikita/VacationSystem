using System;
using System.Diagnostics;
using System.Collections.Generic;
using VacationSystem.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VacationSystem.Classes.Helpers;

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
        /// Получение всех дней отпуска, назначенных сотруднику
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="id">Идентификатор сотрудника</param>
        /// <returns>Список дней отпуска сотрудника</returns>
        static public List<VacationDay> GetVacationDays(ApplicationContext db, string id)
        {
            try
            {
                return db.VacationDays
                    .Include(vd => vd.VacationType)
                    .Where(vd => vd.EmployeeId == id)
                    .ToList();
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
        /// Получение всех доступных отпускных дней сотрудника
        /// </summary>
        /// <param name="empId">Идентификатор сотрудника</param>
        /// <param name="year">Год, на который назначены отпускные дни</param>
        /// <returns>Список отпускных дней сотрудника</returns>
        static public List<VacationDay> GetAvailableVacationDays(string empId, int year)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.VacationDays
                        .Where(vd => vd.EmployeeId == empId 
                        && vd.NumberOfDays > vd.TakenDays 
                        && ((vd.Year == year) || (vd.Year == year - 1)))
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
        /// <param name="db">Контекст БД</param>
        /// <param name="empId">Идентификатор сотрудника</param>
        /// <param name="year">Год, на который назначены отпускные дни</param>
        /// <returns>Список отпускных дней сотрудника</returns>
        static public List<VacationDay> GetAvailableVacationDays(ApplicationContext db, string empId, int year)
        {
            try
            {
                return db.VacationDays
                    .Where(vd => vd.EmployeeId == empId
                    && vd.NumberOfDays > vd.TakenDays
                    && ((vd.Year == year) || (vd.Year == year - 1)))
                    .ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Сохранение в БД дней отпуска одного сотрудника
        /// </summary>
        /// <param name="empId">Идентификатор сотрудника</param>
        /// <param name="type">Тип отпуска</param>
        /// <param name="notes">Примечания руководителя</param>
        /// <param name="number">Количество отпускных дней</param>
        /// <param name="year">Год, на который назначены дни</param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool SetVacationDays(string empId, int type, string notes, int number, int year)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
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
        /// <param name="empId">Идентификатор сотрудника</param>
        /// <param name="type">Тип отпускныз дней</param>
        /// <param name="year">Год, на который назначены дни</param>
        /// <returns>Отпускные дни сотрудника заданного типа</returns>
        static public VacationDay GetEmployeeVacationDays(string empId, int type, int year)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.VacationDays
                            .FirstOrDefault(vd => vd.Year == year
                            && vd.VacationTypeId == type
                            && vd.EmployeeId == empId);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
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

        /// <summary>
        /// Указать, что дни отпуска были взяты для какого-то периода
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="wishedVacation">Запланированный период отпуска</param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool TakeVacationDays(ApplicationContext db, WishedVacationPeriod wishedVacation)
        {
            try
            {
                // список отпускных дней сотрудника
                List<VacationDay> days = GetAvailableVacationDays(db, wishedVacation.EmployeeId, wishedVacation.Year)
                    .OrderByDescending(vd => vd.Year)
                    .ToList();

                // количество дней в отпуске
                int count = 0;
                foreach (VacationPart part in wishedVacation.VacationParts)
                {
                    int holidays = HolidayHelper.CountHolidays(part.StartDate, part.EndDate);
                    count += (part.EndDate - part.StartDate).Days + 1 - holidays;
                }

                // проход по имеюшимся у сотрудника дням отпуска с уменьшением их доступного количества
                foreach (VacationDay day in days)
                {
                    // если уже все дни отпусков учтены, то прекратить проход
                    if (count == 0)
                        break;

                    // иначе указать, что дни отпуска были заняты некоторым отпуском

                    // если count превышает доступное количество дней, то отрезать только то количество, которое возможно
                    if (count >= (day.NumberOfDays - day.TakenDays))
                    {
                        day.TakenDays = day.NumberOfDays;
                        count = count - (day.NumberOfDays - day.TakenDays);
                    }
                    else
                        day.TakenDays += count;
                }

                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Освободить отпускные дни, ранее взятые для утвержденного отпуска
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="vacation">Утвержденный отпуск</param>
        /// <param name="number">Количество освобождаемых дней</param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool FreeVacationDays(ApplicationContext db, SetVacation vacation, int number)
        {
            try
            {
                // получить дни отпуска, назначенные сотруднику
                List<VacationDay> days = GetVacationDays(db, vacation.EmployeeId)
                    .Where(vd => vd.Year == vacation.EndDate.Year || vd.Year == vacation.EndDate.Year - 1)
                    .OrderBy(vd => vd.Year)
                    .ToList();


                // проход по имеющимся у сотрудника дням отпуска с их высвобождением
                bool result = FreeTakenDays(days, number);
                if (!result)
                    return false;

                db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Освобождение занятых отпускных дней
        /// </summary>
        /// <param name="days">Отпускные дни</param>
        /// <param name="count">Количество освобождаемых дней</param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool FreeTakenDays(List<VacationDay> days, int count)
        {
            try
            {
                // проход по имеющимся у сотрудника дням отпуска с их высвобождением
                foreach (VacationDay day in days)
                {
                    // если уже было высвобождено 
                    if (count == 0)
                        break;

                    if (count >= day.TakenDays)
                    {
                        count = count - day.TakenDays;
                        day.TakenDays = 0;
                    }
                    else
                    {
                        day.TakenDays = day.TakenDays - count;
                        count = 0;
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
        /// Освободить отпускные дни, ранее взятые для запланированного отпуска
        /// </summary>
        /// <param name="db">Контекст БД</param>
        /// <param name="wishedVacation">Запланированный отпуск, удаляемый из БД</param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool FreeVacationDays(ApplicationContext db, WishedVacationPeriod wishedVacation)
        {
            try
            {
                // получить дни отпуска, назначенные сотруднику
                List<VacationDay> days = GetVacationDays(db, wishedVacation.EmployeeId)
                    .Where(vd => vd.Year == wishedVacation.Year || vd.Year == wishedVacation.Year - 1)
                    .OrderBy(vd => vd.Year)
                    .ToList();

                // количество дней в отпуске
                int count = 0;
                foreach (VacationPart part in wishedVacation.VacationParts)
                {
                    int holidays = HolidayHelper.CountHolidays(part.StartDate, part.EndDate);
                    count += (part.EndDate - part.StartDate).Days + 1;
                }

                // проход по имеющимся у сотрудника дням отпуска с их высвобождением
                bool result = FreeTakenDays(days, count);
                if (!result)
                    return false;

                db.SaveChanges();

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
