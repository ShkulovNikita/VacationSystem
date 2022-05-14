using VacationSystem.Models;
using VacationSystem.Classes.Database;
using VacationSystem.Classes.Helpers;
using System.Collections.Generic;
using System.Linq;
using System;

namespace VacationSystem.Classes.Rules
{
    /// <summary>
    /// Класс с методами для проверки выполнения правил отпусков, 
    /// накладываемых на должности
    /// </summary>
    static public class PositionRulesChecker
    {
        /// <summary>
        /// Проверить соблюдение правил для должностей
        /// </summary>
        /// <param name="employees">Список сотрудников с их отпусками</param>
        /// <param name="headId">Идентификатор руководителя подразделения</param>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <returns>Список предупреждений о нарушении правила</returns>
        static public List<RuleWarning> CheckPositionRules(List<Employee> employees, string headId, string depId)
        {
            // получить список правил
            List<RuleForPosition> rules = PositionRuleDataHandler.GetPositionRules(headId, depId);

            if (rules == null)
                return null;
            else
                if (rules.Count == 0)
                    return new List<RuleWarning>();

            // получить должности для сотрудников
            employees = EmployeeHelper.GetEmployeesPositions(employees, depId);

            // пройтись по всем правилам и добавить обнаруженные нарушения в список
            List<RuleWarning> warnings = new List<RuleWarning>();
            foreach (RuleForPosition rule in rules)
            {
                RuleWarning ruleWarning = CheckPositionRule(employees, rule);
                if (ruleWarning != null)
                    warnings.Add(ruleWarning);
            }

            return warnings;
        }

        /// <summary>
        /// Проверка соблюдения одного правила для должностей
        /// </summary>
        /// <param name="employees">Список сотрудников с их запланированными отпусками</param>
        /// <param name="rule">Правило для должностей</param>
        /// <returns>Предупреждение о несоблюдении правила либо null</returns>
        static public RuleWarning CheckPositionRule(List<Employee> employees, RuleForPosition rule)
        {
            // оставить только тех сотрудников, которые затронуты правилом
            employees = employees.Where(e => e.DepPositions.Any(p => p.Id == rule.Position.Id)).ToList();

            return null;

        }


    }
}
