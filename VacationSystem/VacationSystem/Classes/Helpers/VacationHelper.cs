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
                    Days = period.EndDate.Subtract(period.StartDate).Days,
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
                Days = vacation.EndDate.Subtract(vacation.StartDate).Days,
                Status = vacation.VacationStatus.Name
            };
        }
    }
}