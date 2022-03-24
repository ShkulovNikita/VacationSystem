using System.Collections.Generic;

namespace VacationSystem.ViewModels
{
    /// <summary>
    /// ViewModel для вывода списка подразделений при выборе заместителя руководителя
    /// </summary>
    public class DeputyDepViewModel
    {
        public DeputyDepViewModel() { }

        /// <summary>
        /// Идентификатор подразделения
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Наименование подразделения
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Список сотрудников в подразделении
        /// </summary>
        public List<DeputyEmpViewModel> Employees { get; set; } = new List<DeputyEmpViewModel>();
    }
}