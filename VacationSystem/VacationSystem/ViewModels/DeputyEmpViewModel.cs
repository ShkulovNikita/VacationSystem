using VacationSystem.Models;

namespace VacationSystem.ViewModels
{
    /// <summary>
    /// ViewModel для вывода возможных сотрудников для выбора заместителя руководителя
    /// </summary>
    public class DeputyEmpViewModel
    {
        public DeputyEmpViewModel() { }

        // уникальный Id для списка
        public string Id { get; set; }

        /// <summary>
        /// Объект сотрудника
        /// </summary>
        public Employee Employee { get; set; }

        /// <summary>
        /// Ссылка на подразделение, в котором работает сотрудник
        /// </summary>
        public DeputyDepViewModel Department { get; set; }
        /// <summary>
        /// Идентификатор подразделения, в котором работает сотрудник
        /// </summary>
        public string DepartmentId { get; set; }
    }
}
