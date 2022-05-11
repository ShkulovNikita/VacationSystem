using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using VacationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace VacationSystem.Classes.Database
{
    /// <summary>
    /// Класс с методами БД для работы с правилами выбора отпусков для групп сотрудников
    /// </summary>
    static public class GroupRuleDataHandler
    {
        /// <summary>
        /// Получение списка правил выбора отпусков для групп сотрудников
        /// </summary>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <returns>Список правил выбора отпусков для групп сотрудников</returns>
        static public List<GroupRule> GetGroupRules(string headId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.GroupRules
                        .Include(gr => gr.Group)
                        .Where(gr => gr.Group.HeadEmployeeId == headId)
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
        /// Получение списка правил выбора отпусков для групп сотрудников
        /// </summary>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <returns>Список правил выбора отпусков для групп сотрудников</returns>
        static public List<GroupRule> GetGroupRules(string headId, string depId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.GroupRules
                            .Include(gr => gr.Group)
                            .Where(gr => gr.Group.HeadEmployeeId == headId
                            && gr.Group.DepartmentId == depId)
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
        /// Добавление в БД нового правила выбора отпусков для групп сотрудников
        /// </summary>
        /// <param name="description">Описание правила</param>
        /// <param name="type">Идентификатор типа правила</param>
        /// <param name="groupId">Идентификатор задействованной группы</param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool AddGroupRule(string description, int type, int groupId, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    db.GroupRules.Add(new GroupRule
                    {
                        Description = description,
                        RuleTypeId = type,
                        GroupId = groupId,
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
        /// Удаление из БД правила для групп
        /// </summary>
        /// <param name="ruleId">Идентификатор правила</param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool DeleteGroupRule(int ruleId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    GroupRule rule = db.GroupRules.FirstOrDefault(r => r.Id == ruleId);
                    if (rule != null)
                    {
                        db.GroupRules.Remove(rule);
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
        /// Редактирование правила выбора отпусков для группы сотрудников
        /// </summary>
        /// <param name="ruleId">Идентификатор правила</param>
        /// <param name="description">Описание правила</param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool EditGroupRule(int ruleId, string description, DateTime startDate, DateTime endDate)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    GroupRule rule = db.GroupRules.FirstOrDefault(r => r.Id == ruleId);
                    if (rule == null)
                        return false;

                    rule.Description = description;
                    rule.StartDate = startDate;
                    rule.EndDate = endDate;

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
        /// Получение правила для группы по идентификатору
        /// </summary>
        /// <param name="ruleId">Идентификатор правила</param>
        /// <returns>Правило выбора отпусков для группы</returns>
        static public GroupRule GetGroupRule(int ruleId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.GroupRules
                        .Include(gr => gr.Group)
                        .Include(gr => gr.RuleType)
                        .FirstOrDefault(gr => gr.Id == ruleId);
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
