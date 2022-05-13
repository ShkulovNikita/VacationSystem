using System;
using System.Linq;
using System.Collections.Generic;
using VacationSystem.Models;
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
            emps = FilterVacations(employees, rule.StartDate, rule.EndDate);

            // должны уходить в отпуск одновременно
            if (rule.RuleTypeId == 1)
            {
                if (CheckSameVacationPeriod(emps))
                    return null;
                else
                {
                    return new RuleWarning
                    {
                        RuleId = rule.Id,
                        Type = "emp",
                        Description = "Сотрудники данного правила должны уходить в отпуск одновременно",
                        RuleDescription = rule.Description,
                        Employees = employees
                    };
                }
            }
            // не должны уходить в отпуск одновременно
            else
            {
                if (CheckNotSameVacationPeriod(emps))
                    return null;
                else
                    return new RuleWarning
                    {
                        RuleId = rule.Id,
                        Type = "emp",
                        Description = "В отпусках сотрудников данного правила не должно быть пересечений",
                        RuleDescription = rule.Description,
                        Employees = employees
                    };
            }
        }

        /// <summary>
        /// Проверка, что указанные сотрудники уходят в отпуск одновременно
        /// </summary>
        /// <param name="employees">Список сотрудников с их отпусками</param>
        /// <returns>Результат проверки: true - уходят одновременно, false - не уходит одновременно</returns>
        static public bool CheckSameVacationPeriod(List<Employee> employees)
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
        /// Проверка того, что отпуска указанных сотрудников не пересекаются
        /// </summary>
        /// <param name="employees">Список сотрудников с их отпусками</param>
        /// <returns>true: отпуска не пересекаются: false: пересекаются</returns>
        static public bool CheckNotSameVacationPeriod(List<Employee> employees)
        {
            // проверка количество сотрудников с отсутствующими отпусками
            int empsWithoutVacations = CountAbsentVacations(employees);

            // если у всех сотрудников не отпусков в данный период, то правило выполняется
            if (empsWithoutVacations == employees.Count)
                return true;
            // аналогично, если только у одного сотрудника есть отпуск
            if (empsWithoutVacations == employees.Count - 1)
                return true;

            // необходимо попарно сравнить всех сотрудников со всеми их отпусками,
            // чтобы найти пересечения
            return (!FindIntersections(employees));

        }

        /// <summary>
        /// Поиск пересечений в отпусках сотрудников
        /// </summary>
        /// <param name="employees">Список сотрудников с их отпусками</param>
        /// <returns>true: есть пересечения; false: нет пересечений</returns>
        static public bool FindIntersections(List<Employee> employees)
        {
            // попарный перебор всех сотрудников
            for (int i = 0; i < employees.Count - 1; i++)
                for (int j = i + 1; j < employees.Count; j++)
                    // сравнение всех отпусков заданных двух сотрудников
                    // если найдено хотя бы одно пересечение, то правило нарушается
                    if (FindIntersectionsForEmployees(employees[i].WishedVacationPeriods[0].VacationParts,
                                                      employees[j].WishedVacationPeriods[0].VacationParts))
                        return true;


            return false;
        }

        /// <summary>
        /// Поиск пересечений в отпусках двух сотрудников
        /// </summary>
        /// <param name="parts1">Периоды отпуска одного сотрудника</param>
        /// <param name="parts2">Периоды отпуска другого сотрудника</param>
        /// <returns>true: есть пересечения; false: нет пересечений</returns>
        static public bool FindIntersectionsForEmployees(List<VacationPart> parts1, List<VacationPart> parts2)
        {
            foreach (VacationPart part1 in parts1) 
                foreach (VacationPart part2 in parts2)
                {
                    // проверить включение одного отпуска внутрь другого
                    if (FindInclusions(part1, part2))
                        return true;

                    // пересечения - частичные накладывания
                    if (FindCrossIntersections(part1, part2))
                        return true;
                }

            return false;
        }

        /// <summary>
        /// Сравнить пару отпусков и определить, включен ли один внутрь другого
        /// </summary>
        /// <param name="part1">Первый период отпуска</param>
        /// <param name="part2">Второй период отпуска</param>
        /// <returns>true: один отпуск включен внутрь другого; false: отпуска не включены друг в друга</returns>
        static public bool FindInclusions(VacationPart part1, VacationPart part2)
        {
            if ((part1.StartDate >= part2.StartDate) && (part1.EndDate <= part2.EndDate))
                return true;
            if ((part2.StartDate >= part1.StartDate) && (part2.EndDate <= part1.EndDate))
                return true;
            return false;
        }

        /// <summary>
        /// Определить, накладываются ли заданные периоды отпусков друг на друга
        /// </summary>
        /// <param name="part1">Первый период отпуска</param>
        /// <param name="part2">Второй период отпуска</param>
        /// <returns>true: есть наложение; false: нет наложений</returns>
        static public bool FindCrossIntersections(VacationPart part1, VacationPart part2)
        {
            if ((part1.StartDate <= part2.StartDate) && (part1.EndDate <= part2.EndDate) && (part1.EndDate >= part2.StartDate))
                return true;
            if ((part2.StartDate <= part1.StartDate) && (part2.EndDate <= part1.EndDate) && (part2.EndDate >= part1.StartDate))
                return true;
            return false;
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
                    if (parts1[i].StartDate >= part.StartDate && parts1[i].EndDate <= part.EndDate)
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
                {
                    filtered.Add(new Employee(emp, null));
                    continue;
                }
                    
                // отфильтровать отпуска по периоду
                List<VacationPart> filteredParts = emp.WishedVacationPeriods[0].VacationParts
                    .Where(vp => vp.StartDate.Month >= startDate.Month && vp.StartDate.Day >= startDate.Day
                    && vp.EndDate.Month <= endDate.Month && vp.EndDate.Day <= endDate.Day)
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