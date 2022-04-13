using System.Collections.Generic;

namespace VacationSystem.ViewModels.ListItems
{
    public class EmpListItem
    {
        public EmpListItem() { }

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
        public DepListItem Department { get; set; }
        /// <summary>
        /// Идентификатор подразделения, в котором работает сотрудник
        /// </summary>
        public string DepartmentId { get; set; }
    }
}
