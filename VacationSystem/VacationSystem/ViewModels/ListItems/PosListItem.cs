namespace VacationSystem.ViewModels.ListItems
{
    public class PosListItem
    {
        public PosListItem() { }

        /// <summary>
        /// Уникальный идентификатор для списка всех должностей
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Идентификатор должности
        /// </summary>
        public string PosId { get; set; }

        /// <summary>
        /// Наименование должности
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Идентификатор подразделения
        /// </summary>
        public string DepartmentId { get; set; }

        /// <summary>
        /// Ссылка на подразделение
        /// </summary>
        public DepListItem Department { get; set; }
    }
}