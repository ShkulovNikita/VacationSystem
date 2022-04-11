using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using VacationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace VacationSystem.Classes.Database
{
    /// <summary>
    /// Класс с методами БД, связанными с правилами выбора
    /// отпусков для сотрудников
    /// </summary>
    static public class EmployeeRuleDataHandler
    {
        /// <summary>
        /// Получение списка правил выбора отпусков, накладываемых на сотрудников
        /// </summary>
        /// <param name="headId">Идентификатор руководителя, установившего правила</param>
        /// <returns>Список правил выбора отпусков для сотрудников</returns>
        static public List<EmployeeRule> GetEmployeeRules(string headId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.EmployeeRules
                        .Include(er => er.EmployeeInRules)
                        .Where(er => er.HeadEmployeeId == headId)
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
        /// Получение списка правил выбора отпусков, накладываемых на сотрудников
        /// </summary>
        /// <param name="headId">Идентификатор руководителя, установившего правила</param>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <returns>Список правил выбора отпусков для сотрудников</returns>
        static public List<EmployeeRule> GetEmployeeRules(string headId, string depId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.EmployeeRules
                        .Include(er => er.EmployeeInRules)
                        .Where(er => er.HeadEmployeeId == headId
                        && er.DepartmentId == depId)
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
        /// Добавление в БД нового правила выбора отпусков для сотрудников
        /// </summary>
        /// <param name="description">Описание правила</param>
        /// <param name="type">Идентификатор типа правила</param>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <param name="employees">Список задействованных правилом сотрудников</param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool AddEmployeesRule(string description, int type, string depId, string headId, List<Employee> employees)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    EmployeeRule rule = new EmployeeRule
                    {
                        Description = description,
                        RuleTypeId = type,
                        DepartmentId = depId,
                        HeadEmployeeId = headId,
                        Date = DateTime.Now
                    };

                    db.EmployeeRules.Add(rule);
                    db.SaveChanges();

                    foreach (Employee employee in employees)
                    {
                        rule.EmployeeInRules.Add(new EmployeeInRule
                        {
                            EmployeeRuleId = rule.Id,
                            EmployeeId = employee.Id,
                            Date = DateTime.Now
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
        /// Удаление из БД правила для сотрудников
        /// </summary>
        /// <param name="ruleId">Идентификатор правила</param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool DeleteEmployeesRule(int ruleId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    EmployeeRule rule = db.EmployeeRules.FirstOrDefault(r => r.Id == ruleId);
                    if (rule != null)
                    {
                        db.EmployeeRules.Remove(rule);
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
        /// Редактирование правила выбора отпусков для сотрудников
        /// </summary>
        /// <param name="ruleId">Идентификатор правила</param>
        /// <param name="description">Описание правила</param>
        /// <param name="employees">Список сотрудников, затрагиваемых правилом</param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool EditEmployeesRule(int ruleId, string description, List<Employee> employees)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    // получить редактируемое правило
                    EmployeeRule rule = db.EmployeeRules
                        .Include(r => r.EmployeeInRules)
                        .FirstOrDefault(r => r.Id == ruleId);
                    if (rule == null)
                        return false;

                    rule.Description = description;

                    foreach (Employee emp in employees)
                    {
                        // если в правиле нет текущего сотрудника, то добавить
                        if (!rule.EmployeeInRules.Any(empInRule => empInRule.EmployeeId == emp.Id))
                            db.EmployeeInRules.Add(new EmployeeInRule
                            {
                                EmployeeRuleId = rule.Id,
                                EmployeeId = emp.Id,
                                Date = DateTime.Now
                            });
                    }

                    // сотрудники, которых больше нет в правиле
                    List<EmployeeInRule> deletedEmps = rule.EmployeeInRules
                        .Where(empInRule => !employees.Any(emp => emp.Id == empInRule.EmployeeId))
                        .ToList();

                    // удалить этих сотрудников
                    db.EmployeeInRules.RemoveRange(deletedEmps);

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
        /// Получение правила для сотрудников по идентификатору
        /// </summary>
        /// <param name="ruleId">Идентификатор правила</param>
        /// <returns>Правило выбора отпусков для сотрудников</returns>
        static public EmployeeRule GetEmployeeRule(int ruleId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.EmployeeRules
                        .Include(er => er.EmployeeInRules)
                        .Include(er => er.RuleType)
                        .FirstOrDefault(er => er.Id == ruleId);
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
