using System;
using System.Collections.Generic;
using System.Diagnostics;
using VacationSystem.Classes.Data;
using VacationSystem.ViewModels;
using VacationSystem.Classes.Database;
using VacationSystem.Models;

namespace VacationSystem.Classes.Helpers
{
    /// <summary>
    /// Класс с различными методами для работы с отпусками сотрудников
    /// </summary>
    static public class VacationHelper
    {
        /// <summary>
        /// Создать объект отпуска из выбранных периодов внутри этого отпуска
        /// </summary>
        /// <param name="startDates">Начальные даты периодов</param>
        /// <param name="endDates">Конечные даты периодов</param>
        /// <returns>Объект с данными об отпуске</returns>
        static public ChosenVacation MakeVacation(DateTime[] startDates, DateTime[] endDates)
        {
            try
            {
                ChosenVacation choosenVacation = new ChosenVacation();

                choosenVacation.Periods = new ChosenPeriod[startDates.Length];

                for (int i = 0; i < startDates.Length; i++)
                {
                    choosenVacation.Periods[i] = new ChosenPeriod
                    {
                        StartDate = startDates[i],
                        EndDate = endDates[i]
                    };
                }

                return choosenVacation;
            }
            catch (IndexOutOfRangeException ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Создать список моделей представления с данными об отпусках сотрудника
        /// </summary>
        /// <param name="empId">Идентификатор сотрудника</param>
        /// <returns>Список отпусков сотрудника в формате ViewModel</returns>
        static public List<VacationViewModel> MakeVacationsList(string empId)
        {
            // получить запланированные отпуска сотрудника
            List<WishedVacationPeriod> wishedVacations = VacationDataHandler.GetWishedVacations(empId);
            // получить утвержденные отпуска
            List<SetVacation> setVacations = VacationDataHandler.GetSetVacations(empId);

            // конвертировать списки в формат ViewModel
            List<VacationViewModel> result = new List<VacationViewModel>();

            if (wishedVacations != null)
                foreach (WishedVacationPeriod vacation in wishedVacations)
                    result.AddRange(ConvertWishedVacationToViewModel(vacation));

            if (setVacations != null)
                foreach (SetVacation vacation in setVacations)
                    result.Add(ConvertSetVacationToViewModel(vacation));

            return result;
        }

        /// <summary>
        /// Конвертировать запланированный отпуск в модель представления
        /// </summary>
        /// <param name="vacation">Запланированный отпуск из БД</param>
        /// <returns>Периоды отпуска в формате модели представления</returns>
        static public List<VacationViewModel> ConvertWishedVacationToViewModel(WishedVacationPeriod vacation)
        {
            List<VacationViewModel> result = new List<VacationViewModel>();

            foreach (VacationPart period in vacation.VacationParts)
            {
                result.Add(new VacationViewModel
                {
                    Id = vacation.Id,
                    Type = "wished",
                    Year = vacation.Date.Year,
                    StartDate = period.StartDate,
                    EndDate = period.EndDate,
                    Days = period.EndDate.Subtract(period.StartDate).Days + 1,
                    Status = "На утверждении"
                });
            }

            return result;
        }

        /// <summary>
        /// Конвертировать утвержденный отпуск в модель представления
        /// </summary>
        /// <param name="vacation">Утвержденный отпуск из БД</param>
        /// <returns>Отпуск в формате модели представления</returns>
        static public VacationViewModel ConvertSetVacationToViewModel(SetVacation vacation)
        {
            return new VacationViewModel
            {
                Id = vacation.Id,
                Type = "set",
                Year = vacation.Date.Year,
                StartDate = vacation.StartDate,
                EndDate = vacation.EndDate,
                Days = vacation.EndDate.Subtract(vacation.StartDate).Days + 1,
                Status = vacation.VacationStatus.Name
            };
        }

        /// <summary>
        /// Получение данных об отпусках сотрудников
        /// в формате View Model для формирование календаря отпусков
        /// </summary>
        /// <param name="employees">Список сотрудников</param>
        /// <param name="year">Год отпусков</param>
        /// <param name="type">Тип отпусков в календаре: запланированные или утвержденные</param>
        /// <returns>Данные об отпусках сотрудников для календаря</returns>
        static public List<EmpVacationViewModel> GetEmployeesVacationsTable(List<Employee> employees, int year, string type)
        {
            List<EmpVacationViewModel> result = new List<EmpVacationViewModel>();

            // получить все даты указанного года
            List<DateTime> dates = DateHelper.GetYearDates(year);

            foreach (Employee emp in employees)
            {
                EmpVacationViewModel empVacation = GetEmployeeVacations(emp, dates, type);
                if (empVacation != null)
                    result.Add(empVacation);
                else
                    return null;
            }

            return result;
        }

        /// <summary>
        /// Получение данных об отпусках одного сотрудника 
        /// для календаря отпусков
        /// </summary>
        /// <param name="employee">Сотрудник</param>
        /// <param name="dates">Даты, для которых необходимо получить данные об отпусках</param>
        /// <param name="type">Тип отпусков в календаре: запланированный или утвержденный</param>
        /// <returns>Модель представления с данными об отпусках одного сотрудника</returns>
        static public EmpVacationViewModel GetEmployeeVacations(Employee employee, List<DateTime> dates, string type)
        {
            // заполнить ViewModel данными о сотруднике
            EmpVacationViewModel empVacation = new EmpVacationViewModel
            {
                EmployeeId = employee.Id,
                Name = employee.LastName + " " + employee.FirstName + " " + employee.MiddleName
            };

            // получить отпуска сотрудника из БД и заполнить соответствующие данные
            if (type == "wished")
            {
                List<WishedVacationPeriod> wishedVacations = VacationDataHandler.GetWishedVacations(employee.Id);
                empVacation.Vacations = CheckVacationDates(dates, wishedVacations, null);
                return empVacation;
            }
            else if (type == "set")
            {
                List<SetVacation> setVacations = VacationDataHandler.GetSetVacations(employee.Id);
                empVacation.Vacations = CheckVacationDates(dates, null, setVacations);
                return empVacation;
            }
            else
                return null;
        }

        /// <summary>
        /// Получение заполненной строки календаря отпусков для одного сотрудника
        /// </summary>
        /// <param name="dates">Список всех дат периода календаря</param>
        /// <param name="wishedVacations">Запланированные отпуска</param>
        /// <param name="setVacations">Утвержденные отпуска</param>
        /// <returns>Строка календаря отпусков для одного сотрудника</returns>
        static public List<EmpVacationPeriodViewModel> CheckVacationDates(List<DateTime> dates, 
            List<WishedVacationPeriod> wishedVacations, 
            List<SetVacation> setVacations)
        {
            List<EmpVacationPeriodViewModel> calendar = new List<EmpVacationPeriodViewModel>();

            // проверить каждую дату из выбранного периода
            foreach(DateTime date in dates)
            {
                EmpVacationPeriodViewModel period = new EmpVacationPeriodViewModel();
                period.Date = date;
                period.IsTaken = false;

                // если дата уже прошла, то поставить отметку
                if (date <= DateTime.Now)
                    period.Past = true;
                else
                    period.Past = false;

                // проверить, входит ли данная дата в число запланированных отпусков
                if (wishedVacations != null)
                    period = CheckWishedVacations(period, wishedVacations, date);

                // проверить, входит ли данная дата в число утвержденных отпусков
                if (setVacations != null)
                    period = CheckSetVacations(period, setVacations, date);

                calendar.Add(period);
            }

            return calendar;
        }

        /// <summary>
        /// Проверить, входит ли указанная дата в периоды
        /// желаемых отпусков сотрудника
        /// </summary>
        /// <param name="period">Модель представления для календаря отпусков</param>
        /// <param name="vacations">Список желаемых отпусков сотрудника</param>
        /// <param name="date">Проверяемая дата</param>
        /// <returns>Модель представления, в которую внесены данные о проверяемом дне</returns>
        static public EmpVacationPeriodViewModel CheckWishedVacations(EmpVacationPeriodViewModel period,
            List<WishedVacationPeriod> vacations,
            DateTime date)
        {
            foreach (WishedVacationPeriod vacation in vacations)
                foreach (VacationPart part in vacation.VacationParts)
                    if ((date >= part.StartDate) && (date <= part.EndDate))
                    {
                        period.IsTaken = true;
                        period.Type = "wished";
                        period.Priority = vacation.Priority;
                    }
                    else
                        period.IsTaken = false;
            return period;
        }

        /// <summary>
        /// Проверить, входит ли указанная дата в периоды утвержденных
        /// отпусков сотрудника
        /// </summary>
        /// <param name="period">Модель представления для календаря отпусков</param>
        /// <param name="vacations">Список утвержденных отпусков сотрудника</param>
        /// <param name="date">Проверяемая дата</param>
        /// <returns>Модель представления, в которую внесены данные о проверяемом дне</returns>
        static public EmpVacationPeriodViewModel CheckSetVacations(EmpVacationPeriodViewModel period,
            List<SetVacation> vacations,
            DateTime date)
        {
            foreach (SetVacation vacation in vacations)
            {
                if ((date >= vacation.StartDate) && (date <= vacation.EndDate))
                {
                    period.IsTaken = true;
                    period.Type = "set";
                    period.Status = vacation.VacationStatus;
                }
            }
            return period;
        }
    }
}