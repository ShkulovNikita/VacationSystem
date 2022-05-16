using System;
using System.Collections.Generic;
using VacationSystem.Models;

namespace VacationSystem.ViewModels
{
    /// <summary>
    /// Модель представления для списка отпусков сотрудника
    /// </summary>
    public class VacationIndexViewModel
    {
        public VacationIndexViewModel () { }

        /// <summary>
        /// Сотрудник, отпуска которого отображаются
        /// </summary>
        public Employee Employee { get; set; }

        /// <summary>
        /// Список отпусков сотрудника
        /// </summary>
        public List<VacationViewModel> Vacations { get; set; } = new List<VacationViewModel>();

        /// <summary>
        /// Количество доступных отпускных дней, которые не были включены в отпуска
        /// </summary>
        public int AvailableDays { get; set; }

        /// <summary>
        /// Года, в которые сотрудник имеет отпуска
        /// </summary>
        public List<int> Years { get; set; }

        /// <summary>
        /// Выбранный пользователем тип отпусков
        /// </summary>
        public string CurrentType { get; set; }

        /// <summary>
        /// Выбранный пользователем год
        /// </summary>
        public int CurrentYear { get; set; }

        /// <summary>
        /// Является ли авторизованный сотрудник руководителем текущего сотрудника
        /// </summary>
        public bool IsHead { get; set; }
    }
}
