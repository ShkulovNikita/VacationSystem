using VacationSystem.Models;

namespace VacationSystem.ViewModels
{
    /// <summary>
    /// ViewModel для вывода возможных сотрудников для выбора заместителя руководителя
    /// </summary>
    public class DeputyEmpViewModel
    {
        public DeputyEmpViewModel() { }

        /// <summary>
        /// Уникальный идентификатор для списка всех сотрудников
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Собственный идентификатор сотрудника
        /// </summary>
        public string EmpId { get; set; }

        /// <summary>
        /// ФИО сотрудника
        /// </summary>
        public string Name { get; set; }

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
