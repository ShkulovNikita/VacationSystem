using System.Collections.Generic;
using VacationSystem.ViewModels.ListItems;

namespace VacationSystem.ViewModels
{
    /// <summary>
    /// Модель представления, отображающая информацию по назначенным
    /// дням отпуска сотрудника
    /// </summary>
    public class VacationDaysViewModel
    {
        public VacationDaysViewModel() { }

        /// <summary>
        /// Общее количество назначенных дней
        /// </summary>
        public int TotalDays { get; set; }

        /// <summary>
        /// Словарь: тип отпуска - выделенное количество дней этого типа
        /// </summary>
        public Dictionary<string, int> SetDays { get; set; } 

        /// <summary>
        /// Количество дней, которые ещё не были потрачены
        /// </summary>
        public int AvailableDays { get; set; }

        /// <summary>
        /// Ссылка на сотрудника
        /// </summary>
        public string EmployeeId { get; set; }

        /// <summary>
        /// Сотрудник, которому заданы данные отпускные дни
        /// </summary>
        public EmpListItem Employee { get; set; }

        /// <summary>
        /// Год, на который выданы данные дни отпуска
        /// </summary>
        public int Year { get; set; }
    }
}
