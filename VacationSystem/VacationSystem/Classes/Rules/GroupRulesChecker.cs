using System.Linq;
using System.Collections.Generic;
using VacationSystem.Models;
using VacationSystem.Classes.Helpers;
using VacationSystem.Classes.Database;

namespace VacationSystem.Classes.Rules
{
    /// <summary>
    /// Класс с методами для проверки выполнения правил для групп сотрудников
    /// </summary>
    static public class GroupRulesChecker
    {
        /// <summary>
        /// Проверить соблюдение правил для групп
        /// </summary>
        /// <param name="employees">Список сотрудников с их отпусками</param>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <returns>Список предупреждений о нарушениях правил</returns>
        static public List<RuleWarning> CheckGroupRules(List<Employee> employees, string headId, string depId)
        {
            // получить список правил
            List<GroupRule> rules = GroupRuleDataHandler.GetGroupRules(headId, depId);

            if (rules == null)
                return null;
            else
                if (rules.Count == 0)
                    return new List<RuleWarning>();

            // проход по всем правилам с обнаружением нарушений
            List<RuleWarning> warnings = new List<RuleWarning>();
            foreach (GroupRule rule in rules)
            {
                RuleWarning ruleWarning = CheckGroupRule(employees, rule);
                if (ruleWarning != null)
                    warnings.Add(ruleWarning);
            }

            return warnings;
        }

        /// <summary>
        /// Проверка соблюдения указанного правила для группы сотрудников
        /// </summary>
        /// <param name="employees">Список сотрудников с их отпусками</param>
        /// <param name="rule">Правило для группы сотрудников</param>
        /// <returns>Предупреждение о нарушении правила либо null</returns>
        static public RuleWarning CheckGroupRule(List<Employee> employees, GroupRule rule)
        {
            // оставить только тех сотрудников, которые затронуты правилом
            List<Employee> emps = employees.Where(e => rule.Group.EmployeesInGroup.Any(eir => eir.EmployeeId == e.Id)).ToList();

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