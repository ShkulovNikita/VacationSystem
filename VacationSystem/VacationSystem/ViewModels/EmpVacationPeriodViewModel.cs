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
        public DateTime Date { get; set; }

        /// <summary>
        /// Входит ли данная дата в отпуск сотрудника
        /// </summary>
        public bool IsTaken { get; set; }

        /// <summary>
        /// Тип отпуска: запланированный или уже утвержденный
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Статус отпуска
        /// </summary>
        public VacationStatus Status { get; set; }
    }
}
