using VacationSystem.Models;
using System;

namespace VacationSystem.ViewModels
{
    /// <summary>
    /// Модель представления для отображения списка правил всех типов
    /// </summary>
    public class RuleViewModel
    {
        public RuleViewModel() { }

        /// <summary>
        /// Идентификатор правила в БД
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Наименование правила
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Описание правила
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Тип правила
        /// </summary>
        public string RuleType { get; set; }

        /// <summary>
        /// Дата создания правила
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Период года, в который действует правило
        /// </summary>
        public string Period { get; set; }

        /// <summary>
        /// Подразделение, в котором действует правило
        /// </summary>
        public Department Department { get; set; }

        /// <summary>
        /// Описание правила на основе его типа и содержания
        /// </summary>
        public string SystemDescription { get; set; }
    }
}
