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
            List<Employee> emps = employees.Where(e => e.DepPositions.Any(p => p.Id == rule.PositionId)).ToList();

            // отфильтровать отпуска сотрудников по периоду, в который действует данное правило
            emps = VacationHelper.FilterVacations(emps, rule.StartDate, rule.EndDate);

            if (CheckPositions(emps, rule))
                return null;
            else
                return new RuleWarning(rule, emps);
        }

        /// <summary>
        /// Проверка соответствия количества сотрудников должности на рабочем месте заданному правилу
        /// </summary>
        /// <param name="emps">Список сотрудников с их должностями и отпусками</param>
        /// <param name="rule">Правило для должности</param>
        /// <returns>true: правило соблюдается; false: правило не соблюдается</returns>
        static public bool CheckPositions(List<Employee> emps, RuleForPosition rule)
        {
            // словарь, содержащий пары "дата - количество сотрудников, уходящих в эту дату в отпуск"
            Dictionary<DateTime, int> vacationCounter = new Dictionary<DateTime, int>();

            // проход всех сотрудников с просмотром дат их отпусков
            foreach (Employee emp in emps)
            {
                if (emp.WishedVacationPeriods.Count == 0)
                    continue;

                // обновить счетчики сотрудников в отпуске для дат
                vacationCounter = AddVacationDays(vacationCounter, emp.WishedVacationPeriods[0].VacationParts);
            }

            // проверить, что нет дня, который бы нарушал правило
            foreach (var date in vacationCounter)
            {
                // сколько сотрудников может уйти в отпуск одновременно
                int limit = emps.Count - rule.PeopleNumber;

                // проверка превышения лимита
                if (date.Value > limit)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Обновить счетчик сотрудников, уходящих в отпуск в заданные дни
        /// </summary>
        /// <param name="counter">Словарь из пар "дата - количество сотрудников в отпуске в этот день"</param>
        /// <param name="period">Периоды, в течение которых данный сотрудник находится в отпуске</param>
        /// <returns>Словарь с обновленными счетчиками</returns>
        static public Dictionary<DateTime, int> AddVacationDays(Dictionary<DateTime, int> counter, List<VacationPart> period)
        {
            // все даты отпуска сотрудника
            List<DateTime> dates = new List<DateTime>();
            foreach (VacationPart part in period)
                dates.AddRange(DateHelper.GetDateRange(part.StartDate, part.EndDate));

            // добавить даты, которых нет в словаре, и обновить счетчики у существующих
            foreach (DateTime date in dates)
            {
                if (counter.ContainsKey(date))
                    counter[date] += 1;
                else
                    counter.Add(date, 1);
            }

            return counter;
        }
    }
}
