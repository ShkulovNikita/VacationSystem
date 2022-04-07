namespace VacationSystem.ViewModels.ListItems
{
    public class GroupListItem
    {
        public GroupListItem() { }

        /// <summary>
        /// Уникальный идентификатор группы в списке
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор группы в БД
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        /// Наименование группы
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Идентификатор подразделения группы
        /// </summary>
        public string DepartmentId { get; set; }

        /// <summary>
        /// Ссылка на подразделение группы
        /// </summary>
        public DepListItem Department { get; set; }
    }
}
