using System;
using System.Linq;
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
                    result.AddRange(ConvertWishedVacationToViewModel(vacation, empId));

            if (setVacations != null)
                foreach (SetVacation vacation in setVacations)
                    result.Add(ConvertSetVacationToViewModel(vacation, empId));

            return result;
        }

        /// <summary>
        /// Конвертировать запланированный отпуск в модель представления
        /// </summary>
        /// <param name="vacation">Запланированный отпуск из БД</param>
        /// <param name="empId">Идентификатор сотрудника</param>
        /// <returns>Периоды отпуска в формате модели представления</returns>
        static public List<VacationViewModel> ConvertWishedVacationToViewModel(WishedVacationPeriod vacation, string empId)
        {
            List<VacationViewModel> result = new List<VacationViewModel>();

            foreach (VacationPart period in vacation.VacationParts)
            {
                result.Add(new VacationViewModel
                {
                    Id = vacation.Id,
                    Type = "wished",
                    Year = vacation.Year,
                    StartDate = period.StartDate,
                    EndDate = period.EndDate,
                    Days = period.EndDate.Subtract(period.StartDate).Days + 1,
                    Status = "На утверждении",
                    EmpId = empId
                });
            }

            return result;
        }

        /// <summary>
        /// Конвертировать утвержденный отпуск в модель представления
        /// </summary>
        /// <param name="vacation">Утвержденный отпуск из БД</param>
        /// <param name="empId">Идентификатор сотрудника</param>
        /// <returns>Отпуск в формате модели представления</returns>
        static public VacationViewModel ConvertSetVacationToViewModel(SetVacation vacation, string empId)
        {
            return new VacationViewModel
            {
                Id = vacation.Id,
                Type = "set",
                Year = vacation.Date.Year,
                StartDate = vacation.StartDate,
                EndDate = vacation.EndDate,
                Days = vacation.EndDate.Subtract(vacation.StartDate).Days + 1,
                Status = vacation.VacationStatus.Name,
                EmpId = empId
            };
        }

        /// <summary>
        /// Получение данных об отпусках сотрудников
        /// в формате View Model для формирование календаря отпусков
        /// </summary>
        /// <param name="employees">Список сотрудников</param>
        /// <param name="type">Тип отпусков в календаре: запланированные или утвержденные</param>
        /// <param name="startDate">Начальная дата календаря</param>
        /// <param name="endDate">Конечная дата календаря</param>
        /// <returns>Данные об отпусках сотрудников для календаря</returns>
        static public List<EmpVacationViewModel> GetEmployeesVacationsTable(List<Employee> employees, 
            string type,
            DateTime startDate,
            DateTime endDate)
        {
            List<EmpVacationViewModel> result = new List<EmpVacationViewModel>();

            // получить все даты указанного периода
            List<DateTime> dates = DateHelper.GetDateRange(startDate, endDate);

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
            };

            if ((employee.MiddleName != null) && (employee.MiddleName != ""))
                empVacation.Name = employee.LastName + " " + employee.FirstName[0] + ". " + employee.MiddleName[0] + ".";
            else
                empVacation.Name = employee.LastName + " " + employee.FirstName[0] + ". ";

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
            else if (type == "all")
            {
                List<WishedVacationPeriod> wishedVacations = VacationDataHandler.GetWishedVacations(employee.Id);
                List<SetVacation> setVacations = VacationDataHandler.GetSetVacations(employee.Id);
                empVacation.Vacations = CheckVacationDates(dates, wishedVacations, setVacations);

                return empVacation;
            }
            else
            {
                return null;
            }
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
                period.Date = date.ToString("dd.MM");
                period.IsTaken = false;

                // если дата уже прошла, то поставить отметку
                if (date <= DateTime.Now)
                    period.Past = true;
                else
                    period.Past = false;

                if ((wishedVacations != null) && (setVacations != null))
                {
                    period = CheckSetVacations(period, setVacations, date);
                    if (period.IsTaken == false)
                        period = CheckWishedVacations(period, wishedVacations, date);
                }
                else
                {
                    // проверить, входит ли данная дата в число запланированных отпусков
                    if (wishedVacations != null)
                        period = CheckWishedVacations(period, wishedVacations, date);

                    // проверить, входит ли данная дата в число утвержденных отпусков
                    if (setVacations != null)
                        period = CheckSetVacations(period, setVacations, date);
                }

                if (period.IsTaken == false)
                    period.DayType = HolidayHelper.GetDayType(date);

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

        /// <summary>
        /// Получить запланированные периоды отпусков сотрудника на указанный год
        /// </summary>
        /// <param name="id">Идентификатор отпуска</param>
        /// <returns>Список пар из дат "начало отпуска - конец отпуска"</returns>
        static public List<VacationDatesViewModel> GetWishedVacationPeriods(int id)
        {
            List<VacationDatesViewModel> periods = new List<VacationDatesViewModel>();

            // получить указанный запланированный отпуск
            WishedVacationPeriod wishedVacation = VacationDataHandler.GetWishedVacation(id);

            // преобразовать периоды отпуска в формат View Model
            foreach (VacationPart part in wishedVacation.VacationParts)
            {
                VacationDatesViewModel period = new VacationDatesViewModel
                {
                    StartDate = part.StartDate,
                    EndDate = part.EndDate
                };

                periods.Add(period);
            }

            return periods;
        }

        /// <summary>
        /// Подготовить список утверждаемых отпусков на основе запланированных отпусков сотрудников
        /// </summary>
        /// <param name="employees">Список сотрудников</param>
        /// <returns>Список утверждаемых отпусков</returns>
        static private List<SetVacation> MakeSetVacations(List<Employee> employees, int year)
        {
            List<SetVacation> result = new List<SetVacation>();

            foreach (Employee employee in employees)
            {
                // получить желаемые отпуска сотрудника
                List<WishedVacationPeriod> periods = VacationDataHandler.GetWishedVacations(employee.Id, year);
                employee.WishedVacationPeriods = periods.OrderBy(wv => wv.Priority).ToList();

                if (employee.WishedVacationPeriods == null)
                    continue;
                if (employee.WishedVacationPeriods.Count == 0)
                    continue;

                // если у сотрудника есть запланированные отпуска, то пройтись по ним
                foreach (VacationPart part in employee.WishedVacationPeriods[0].VacationParts)
                {
                    SetVacation vacation = new SetVacation
                    {
                        StartDate = part.StartDate,
                        EndDate = part.EndDate,
                        Date = DateTime.Now,
                        VacationStatusId = VacationDataHandler.GetVacationStatus("Утвержден").Id,
                        EmployeeId = employee.Id
                    };

                    result.Add(vacation);
                }
            }

            return result;
        }

        /// <summary>
        /// Утвердить запланированные отпуска указанных сотрудников
        /// </summary>
        /// <param name="employees">Список сотрудников</param>
        /// <param name="year">Год, на который назначаются отпуска</param>
        /// <returns></returns>
        static public bool SetVacations(List<Employee> employees, int year)
        {
            List<SetVacation> vacations = MakeSetVacations(employees, year);

            bool result = VacationDataHandler.SetVacations(vacations, employees, year);

            return result;
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
                List<VacationPart> filteredParts = FilterByPeriod(emp.WishedVacationPeriods[0].VacationParts, startDate, endDate);

                // добавить сотрудника с отфильтрованными отпусками в список результата
                if (filteredParts != null)
                    filtered.Add(new Employee(emp, filteredParts));
                else
                    filtered.Add(new Employee(emp, new List<VacationPart>()));
            }

            return filtered;
        }

        /// <summary>
        /// Фильтр, отсеивающий периоды отпуска за пределами указанных границ и сокращающий отпуска в случае,
        /// если они частично входят в заданный период
        /// </summary>
        /// <param name="unfilteredParts">Все периоды отпуска сотрудника</param>
        /// <param name="startDate">Начальная дата периода-границы</param>
        /// <param name="endDate">Конечная дата периода-границы</param>
        /// <returns>Список периодов отпуска, входящих в заданные даты-границы</returns>
        static public List<VacationPart> FilterByPeriod(List<VacationPart> unfilteredParts, DateTime startDate, DateTime endDate)
        {
            List<VacationPart> filteredParts = new List<VacationPart>();

            foreach (VacationPart part in unfilteredParts)
            {
                startDate = new DateTime(part.StartDate.Year, startDate.Month, startDate.Day);
                endDate = new DateTime(part.EndDate.Year, endDate.Month, endDate.Day);

                if ((part.StartDate >= startDate) && (part.EndDate <= endDate))
                    filteredParts.Add(part);
                else if ((part.StartDate >= startDate) && (part.EndDate >= endDate) && (part.StartDate <= endDate))
                    filteredParts.Add(new VacationPart
                    {
                        StartDate = part.StartDate,
                        EndDate = endDate,
                        Part = part.Part,
                        WishedVacationPeriodId = part.WishedVacationPeriodId,
                        WishedVacationPeriod = part.WishedVacationPeriod
                    });
                else if ((part.StartDate <= startDate) && (part.EndDate <= endDate) && (part.EndDate >= startDate))
                    filteredParts.Add(new VacationPart
                    {
                        StartDate = startDate,
                        EndDate = part.EndDate,
                        Part = part.Part,
                        WishedVacationPeriod = part.WishedVacationPeriod,
                        WishedVacationPeriodId = part.WishedVacationPeriodId
                    });
                else if ((part.StartDate <= startDate) && (part.EndDate >= endDate))
                    filteredParts.Add(new VacationPart
                    {
                        StartDate = startDate,
                        EndDate = endDate,
                        Part = part.Part,
                        WishedVacationPeriod = part.WishedVacationPeriod,
                        WishedVacationPeriodId = part.WishedVacationPeriodId
                    });
            }

            return filteredParts;
        }

        /// <summary>
        /// Получить периоды уже утвержденных отпусков сотрудника
        /// </summary>
        /// <param name="empId">Идентификатор сотрудника</param>
        /// <param name="year">Год отпусков</param>
        /// <returns>Список периодов утвержденного отпуска сотрудника</returns>
        static public List<ChosenPeriod> GetSetPeriods(string empId, int year)
        {
            List<ChosenPeriod> result = new List<ChosenPeriod>();

            // получить утвержденные отпуска сотрудника на год
            List<SetVacation> vacations = VacationDataHandler.GetSetVacations(empId, year);

            if (vacations == null)
                return null;

            foreach (SetVacation vacation in vacations)
                result.Add(new ChosenPeriod
                {
                    StartDate = vacation.StartDate,
                    EndDate = vacation.EndDate,
                    Type = true
                });

            return result;
        }
    }
}