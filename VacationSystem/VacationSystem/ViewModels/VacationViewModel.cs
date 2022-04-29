using System;

namespace VacationSystem.ViewModels
{
    /// <summary>
    /// Модель представления с данными об отпусках
    /// </summary>
    public class VacationViewModel
    {
        public VacationViewModel() { }

        /// <summary>
        /// Идентификатор в БД
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор сотрудника, которому принадлежит данный отпуск
        /// </summary>
        public string EmpId { get; set; }

        /// <summary>
        /// Тип отпуска: запланированный или уже утвержденный
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Год, в который отпуск был назначен
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Начальная дата отпуска
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Конечная дата отпуска
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Количество дней в отпуске
        /// </summary>
        public int Days { get; set; }

        /// <summary>
        /// Статус отпуска
        /// </summary>
        public string Status { get; set; }
    }
}