using System;
using VacationSystem.Models;

namespace VacationSystem.ViewModels
{
    /// <summary>
    /// Модель представления для указания в таблице, в какие
    /// дни у сотрудника есть отпуска
    /// </summary>
    public class EmpVacationPeriodViewModel
    {
        public EmpVacationPeriodViewModel() { }

        /// <summary>
        /// Текущая дата
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// Входит ли данная дата в отпуск сотрудника
        /// </summary>
        public bool IsTaken { get; set; }

        /// <summary>
        /// Тип отпуска: запланированный или уже утвержденный
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Если нет отпуска, то тип может быть:
        /// b - будний день;
        /// h - праздничный;
        /// v - выходной.
        /// </summary>
        public string DayType { get; set; } = "";

        /// <summary>
        /// Приоритет желаемого отпуска (если задан)
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// Статус отпуска
        /// </summary>
        public VacationStatus Status { get; set; }

        /// <summary>
        /// Прошла ли уже данная дата:
        /// true - прошла,
        /// false - ещё нет
        /// </summary>
        public bool Past { get; set; }
    }
}
