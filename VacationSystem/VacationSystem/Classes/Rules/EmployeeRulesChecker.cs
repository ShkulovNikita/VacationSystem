using System.Linq;
using System.Collections.Generic;
using VacationSystem.Models;
using VacationSystem.Classes.Helpers;
using VacationSystem.Classes.Database;

namespace VacationSystem.Classes.Rules
{
    /// <summary>
    /// Класс для проверок отпусков на соответствие созданным правилам
    /// </summary>
    static public class EmployeeRulesChecker
    {
        /// <summary>
        /// Проверить соблюдение правил для сотрудников
        /// </summary>
        /// <param name="employees">Список сотрудников с их запланированными отпусками</param>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <returns>Список предупреждений о нарушении правила</returns>
        static public List<RuleWarning> CheckEmployeeRules(List<Employee> employees, string headId, string depId)
        {
            // получить список правил
            List<EmployeeRule> rules = EmployeeRuleDataHandler.GetEmployeeRules(headId, depId);

            if (rules == null)
                return null;
            else
                if (rules.Count == 0)
                    return new List<RuleWarning>();

            // пройтись по всем правилам и добавить обнаруженные нарушения в список
            List<RuleWarning> warnings = new List<RuleWarning>();
            foreach (EmployeeRule rule in rules)
            {
                RuleWarning ruleWarning = CheckEmployeeRule(employees, rule);
                if (ruleWarning != null)
                    warnings.Add(ruleWarning);
            }

            return warnings;
        }

        /// <summary>
        /// Проверка соблюдения указанного правила для сотрудников
        /// </summary>
        /// <param name="employees">Список сотрудников с их отпусками</param>
        /// <param name="rule">Правило для сотрудников</param>
        /// <returns>Список выявленных нарушений правила</returns>
        static public RuleWarning CheckEmployeeRule(List<Employee> employees, EmployeeRule rule)
        {
            // оставить только тех сотрудников, которые затронуты правилом
            List<Employee> emps = employees.Where(e => rule.EmployeeInRules.Any(eir => eir.EmployeeId == e.Id)).ToList();

            // отфильтровать отпуска сотрудников по периоду, в который действует данное правило
            emps = VacationHelper.FilterVacations(emps, rule.StartDate, rule.EndDate);

            // должны уходить в отпуск одновременно
            if (rule.RuleTypeId == 1)
            {
                if (PeriodsChecker.CheckSameVacationPeriod(emps))
                    return null;
                else
                    return new RuleWarning(rule, emps, true);
            }
            // не должны уходить в отпуск одновременно
            else
            {
                if (PeriodsChecker.CheckNotSameVacationPeriod(emps))
                    return null;
                else
                    return new RuleWarning(rule, emps, false);
            }
        }
    }
}