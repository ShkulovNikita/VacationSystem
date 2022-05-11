﻿using System;
using System.Linq;
using System.Collections.Generic;
using VacationSystem.Models;
using VacationSystem.Classes.Database;

namespace VacationSystem.Classes
{
    /// <summary>
    /// Класс для проверок отпусков на соответствие созданным правилам
    /// </summary>
    static public class RulesChecker
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
                List<RuleWarning> ruleWarnings = CheckEmployeeRule(employees, rule);
                if (ruleWarnings != null)
                    warnings.AddRange(ruleWarnings);
            }

            return warnings;
        }

        /// <summary>
        /// Проверка соблюдения указанного правила для сотрудников
        /// </summary>
        /// <param name="employees">Список сотрудников с их отпусками</param>
        /// <param name="rule">Правило для сотрудников</param>
        /// <returns>Список выявленных нарушений правила</returns>
        static public List<RuleWarning> CheckEmployeeRule(List<Employee> employees, EmployeeRule rule)
        {
            // отфильтровать отпуска сотрудников по периоду, в который действует данное правило
            List<Employee> emps = FilterVacations(employees, rule.StartDate, rule.EndDate);


            //TODO
            

            return null;
        }

        /// <summary>
        /// Проверка, что указанные сотрудники уходят в отпуск одновременно
        /// </summary>
        /// <param name="employees">Список сотрудников с их отпусками</param>
        /// <param name="startDate">Начальная дата периода, в который работает правило</param>
        /// <param name="endDate">Конечная дата периода</param>
        /// <returns>Результат проверки: true - уходят одновременно, false - не уходит одновременно</returns>
        static public bool CheckSameVacationPeriod(List<Employee> employees, DateTime startDate, DateTime endDate)
        {
            // в первую очередь проверить, есть ли сотрудники с отсутствующими отпусками
            int empsWithoutVacations = CountAbsentVacations(employees);

            if (empsWithoutVacations > 0)
            {
                // если у всех сотрудников нет отпуска, то правило выполняется
                if (empsWithoutVacations == employees.Count)
                    return true;
                // в противном случае означает, что как минимум один сотрудник из правила не вышел в отпуск, когда должен
                else
                    return false;
            }

            // необходимо сравнивать всех сотрудников с тем,
            // у которого наименьшее количество дней отпуска
            
            // сотрудники, упорядоченные по возрастанию количества дней отпуска
            employees = employees.OrderBy(e => CountVacationDays(e.WishedVacationPeriods[0].VacationParts)).ToList();

            // сравнить отпуска сотрудников на совпадение
            return CompareEmployeeVacations(employees);
        }

        /// <summary>
        /// Сравнение отпусков сотрудников
        /// </summary>
        /// <param name="employees">Список сотрудников с их отпусками</param>
        /// <returns>true: отпуска сотрудников совпадают; false: отпуска сотрудников не совпадают</returns>
        static public bool CompareEmployeeVacations(List<Employee> employees)
        {
            for (int i = 0; i < employees.Count - 1; i++)
                for (int j = i + 1; j < employees.Count; j++)
                    // отпуска первого сотрудника меньше или равны по размеру
                    // отпускам второго сравниваемого сотрудника,
                    // поэтому они должны быть включены внутрь вторых
                    if (!CompareTwoEmployees(employees[i].WishedVacationPeriods[0].VacationParts,
                                             employees[j].WishedVacationPeriods[0].VacationParts))
                        return false;

            return true;
        }

        /// <summary>
        /// Сравнение отпусков двух сотрудников
        /// </summary>
        /// <param name="parts1">Отпуска первого сотрудника (количество дней меньше или равно второму)</param>
        /// <param name="parts2">Отпуска второго сотрудника</param>
        /// <returns>true: отпуска совпадают, false: не совпадают</returns>
        static public bool CompareTwoEmployees(List<VacationPart> parts1, List<VacationPart> parts2)
        {
            // отметка о нахождении всех "аналогов" отпусков первого сотрудника
            // в отпусках второго
            bool[] found = new bool[parts1.Count];
            for (int i = 0; i < found.Length; i++)
                found[i] = false;

            for (int i = 0; i < found.Length; i++)
                foreach (VacationPart part in parts2)
                {
                    if ((parts1[i].StartDate >= part.StartDate) && (parts1[i].EndDate <= part.EndDate))
                    {
                        found[i] = true;
                        break;
                    }
                        
                }

            // проверить, совпадают ли все периоды отпуска
            for (int i = 0; i < found.Length; i++)
                if (!found[i])
                    return false;

            return true;
        }

        /// <summary>
        /// Оставить у сотрудников только те отпуска, которые входят в период правила
        /// </summary>
        /// <param name="employees">Список сотрудников с их отпусками</param>
        /// <param name="startDate">Начальная дата периода</param>
        /// <param name="endDate">Конечная дата периода</param>
        /// <returns>Список сотрудников с отфильтрованными отпусками</returns>
        static public List<Employee> FilterVacations(List<Employee> employees, DateTime startDate, DateTime endDate)
        {
            // список сотрудников с отпусками, которые выпадают только на указанный период
            List<Employee> filtered = new List<Employee>();

            foreach (Employee emp in employees)
            {
                if (emp.WishedVacationPeriods.Count == 0)
                    filtered.Add(new Employee(emp, new List<VacationPart>()));

                // отфильтровать отпуска по периоду
                List<VacationPart> filteredParts = emp.WishedVacationPeriods[0].VacationParts
                    .Where(vp => (vp.StartDate.Month >= startDate.Month && vp.StartDate.Day >= startDate.Day)
                    && (vp.EndDate.Month <= endDate.Month && vp.EndDate.Day <= endDate.Day))
                    .ToList();

                // добавить сотрудника с отфильтрованными отпусками в список результата
                if (filteredParts != null)
                    filtered.Add(new Employee(emp, filteredParts));
                else
                    filtered.Add(new Employee(emp, new List<VacationPart>()));
            }

            return filtered;
        }

        /// <summary>
        /// Посчитать количество дней отпуска сотрудника в указанные периоды
        /// </summary>
        /// <param name="parts">Периоды отпуска</param>
        /// <returns>Количество дней отпуска</returns>
        static public int CountVacationDays(List<VacationPart> parts)
        {
            int count = 0;

            foreach (VacationPart part in parts) 
                count += Math.Abs((part.EndDate - part.StartDate).Days) + 1;

            return count;
        }

        /// <summary>
        /// Посчитать количество сотрудников, не имеющих отпусков
        /// </summary>
        /// <param name="employees">Список сотрудников с их отпусками</param>
        /// <returns>Количество сотрудников, у которых нет отпусков</returns>
        static public int CountAbsentVacations(List<Employee> employees)
        {
            int count = 0;

            foreach (Employee emp in employees)
                if (emp.WishedVacationPeriods.Count == 0)
                    count++;
                else if (emp.WishedVacationPeriods[0].VacationParts.Count == 0)
                    count++;

            return count;
        }
    }
}