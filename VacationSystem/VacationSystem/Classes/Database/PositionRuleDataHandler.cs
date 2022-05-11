using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using VacationSystem.Models;

namespace VacationSystem.Classes.Database
{
    /// <summary>
    /// Класс с методами БД для работы с правилами выбора отпусков для должностей сотрудников
    /// </summary>
    static public class PositionRuleDataHandler
    {
        /// <summary>
        /// Получение списка правил выбора отпусков для должностей
        /// </summary>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <returns>Список правил для должностей</returns>
        static public List<RuleForPosition> GetPositionRules(string headId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.RuleForPositions
                        .Where(pr => pr.HeadEmployeeId == headId)
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
        /// Получение списка правил выбора отпусков для должностей
        /// </summary>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <returns>Список правил для должностей</returns>
        static public List<RuleForPosition> GetPositionRules(string headId, string depId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.RuleForPositions
                        .Where(pr => pr.HeadEmployeeId == headId
                        && pr.DepartmentId == depId)
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
        /// Добавление в БД нового правила выбора отпусков для должностей
        /// </summary>
        /// <param name="number">Количество людей одной должности, которые должны быть на рабочем месте</param>
        /// <param name="description">Описание правила</param>
        /// <param name="posId">Идентификатор задействованной должности</param>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool AddPositionRule(int number, string description, string posId, string depId, string headId, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    db.RuleForPositions.Add(new RuleForPosition
                    {
                        PeopleNumber = number,
                        Description = description,
                        PositionId = posId,
                        DepartmentId = depId,
                        HeadEmployeeId = headId,
                        StartDate = startDate,
                        EndDate = endDate,
                        Date = DateTime.Now
                    });

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

        /// <summary>
        /// Удаление из БД правила для должностей
        /// </summary>
        /// <param name="ruleId">Идентификатор правила</param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool DeletePositionRule(int ruleId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    RuleForPosition rule = db.RuleForPositions.FirstOrDefault(r => r.Id == ruleId);
                    if (rule != null)
                    {
                        db.RuleForPositions.Remove(rule);
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
        /// Редактирование правила выбора отпусков для должности
        /// </summary>
        /// <param name="ruleId">Идентификатор правила</param>
        /// <param name="number">Количество сотрудников должности, которые должны быть на рабочем месте</param>
        /// <param name="description">Описание правила</param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool EditPositionRule(int ruleId, int number, string description, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    RuleForPosition rule = db.RuleForPositions.FirstOrDefault(r => r.Id == ruleId);
                    if (rule == null)
                        return false;

                    rule.Description = description;
                    rule.StartDate = startDate;
                    rule.EndDate = endDate;
                    rule.PeopleNumber = number;

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

        /// <summary>
        /// Получение правила для должности по идентификатору
        /// </summary>
        /// <param name="ruleId">Идентификатор правила</param>
        /// <returns>Правило выбора отпусков для должности</returns>
        static public RuleForPosition GetPositionRule(int ruleId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.RuleForPositions
                        .FirstOrDefault(rp => rp.Id == ruleId);
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