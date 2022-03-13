using VacationSystem.Models;

namespace VacationSystem.ViewModels
{
    /// <summary>
    /// Модель представления для отображения 
    /// выбранных руководителем стилей управления
    /// отпусками
    /// </summary>
    public class HeadStyleViewModel
    {
        public HeadStyleViewModel() { }

        public int Id { get; set; }

        public ManagementStyle Style { get; set; }

        public Department Department { get; set; }
    }
}