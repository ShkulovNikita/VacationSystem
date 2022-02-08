using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    /// <summary>
    /// Тип уведомления
    /// </summary>

    [Table("notification_types")]
    public class NotificationType
    {
        public int Id { get; set; }

        /// <summary>
        /// Название типа уведомления
        /// </summary>
        [Required, MaxLength(50)]
        public string Name { get; set; }

        /// <summary>
        /// Уведомления, отправленные пользователям, данного типа
        /// </summary>
        public List<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
