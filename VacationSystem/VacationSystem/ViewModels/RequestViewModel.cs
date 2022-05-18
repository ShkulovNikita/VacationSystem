using System.Collections.Generic;
using VacationSystem.Models;

namespace VacationSystem.ViewModels
{
    /// <summary>
    /// ViewModel для отображения страницы для написания запроса на изменение утвержденного отпуска
    /// </summary>
    public class RequestViewModel
    {
        public RequestViewModel() { }

        /// <summary>
        /// Сотрудник, пишущий заявление
        /// </summary>
        public Employee Employee { get; set; }

        /// <summary>
        /// Идентификатор отпуска, для которого составляется заявка
        /// </summary>
        public int VacationId { get; set; }

        /// <summary>
        /// Список руководителей сотрудника
        /// </summary>
        public List<Employee> Heads { get; set; } = new List<Employee>();
    }
}
