using System.Collections.Generic;
using VacationSystem.Models;

namespace VacationSystem.ViewModels
{
    public class HomeViewModel
    {
        /// <summary>
        /// ФИО сотрудника
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Список уведомлений пользователя
        /// </summary>
        public List<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
