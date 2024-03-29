﻿using System;
using System.Collections.Generic;
using System.Linq;
using VacationSystem.Models;

namespace VacationSystem.Classes.Rules
{
    /// <summary>
    /// Класс с методами для проверки пересечений и наложений периодов отпусков сотрудников
    /// </summary>
    static public class PeriodsChecker
    {
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

            // убрать тех сотрудников, у которых нет отпусков
            employees = employees.Where(e => e.WishedVacationPeriods.Count > 0).ToList();

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
