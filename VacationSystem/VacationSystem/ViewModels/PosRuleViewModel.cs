using VacationSystem.Models;

namespace VacationSystem.ViewModels
{
    /// <summary>
    /// Модель представления с информацией о правиле для должности
    /// </summary>
    public class PosRuleViewModel
    {
        public PosRuleViewModel() { }

        /// <summary>
        /// Правило для должности
        /// </summary>
        public RuleForPosition Rule { get; set; }

        /// <summary>
        /// Должность, к которой применено правило
        /// </summary>
        public Position Position { get; set; }
    }
}