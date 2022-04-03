using System.Collections.Generic;

namespace VacationSystem.ViewModels.ListItems
{
    public class DepListItem
    {
        public DepListItem() { }

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
        public List<EmpListItem> Employees { get; set; } = new List<EmpListItem>();
    }
}
