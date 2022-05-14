﻿using VacationSystem.Models;
using System.Collections.Generic;

namespace VacationSystem.Classes.Rules
{
    /// <summary>
    /// Класс для объектов, указывающих, какие правила не были соблюдены
    /// при попытке утверждения отпусков
    /// </summary>
    public class RuleWarning
    {
        public RuleWarning () { }

        /// <summary>
        /// Идентификатор правила, которое было нарушено
        /// </summary>
        public int RuleId { get; set; }

        /// <summary>
        /// Тип правила:
        /// emp - для сотрудников;
        /// pos - для должностей;
        /// group - для групп
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Описание нарушения правила
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Собственное описание правила, которое было нарушено
        /// </summary>
        public string RuleDescription { get; set; }

        /// <summary>
        /// Список сотрудников, нарушающих правило
        /// </summary>
        public List<Employee> Employees { get; set; }

        /// <summary>
        /// Должность, у которой было нарушено правило
        /// </summary>
        public Position Position { get; set; }
    }
}
