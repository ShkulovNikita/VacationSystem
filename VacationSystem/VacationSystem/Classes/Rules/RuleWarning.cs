using VacationSystem.Models;
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

        public RuleWarning (EmployeeRule rule, List<Employee> employees, bool type)
        {
            RuleId = rule.Id;
            Type = "emp";
            if (type)
                Description = "Сотрудники данного правила должны уходить в отпуск одновременно";
            else
                Description = "В отпусках сотрудников данного правила не должно быть пересечений";
            Employees = employees;
            if (rule.Description == null)
                RuleDescription = "";
            else
                RuleDescription = rule.Description;
        }

        public RuleWarning (RuleForPosition rule)
        {
            RuleId = rule.Id;
            Type = "pos";
            Description = "На рабочем месте меньшее количество сотрудников должности, чем должно быть";
            Position = rule.Position;
            if (rule.Description == null)
                RuleDescription = "";
            else
                RuleDescription = rule.Description;
        }

        public RuleWarning (GroupRule rule, List<Employee> employees, bool type)
        {
            RuleId = rule.Id;
            Type = "group";
            if (type)
                Description = "Сотрудники из группы данного правила должны уходить в отпуск одновременно";
            else
                Description = "В отпусках сотрудников из группы данного правила не должно быть пересечений";
            Employees = employees;
            if (rule.Description == null)
                RuleDescription = "";
            else
                RuleDescription = rule.Description;
        }

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
