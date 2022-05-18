using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    /// <summary>
    /// Уведомления, отправляемые пользователям
    /// системы
    /// </summary>

    [Table("notifications")]
    public class Notification
    {
        public int Id { get; set; }

        /// <summary>
        /// Текст уведомления, отображаемый
        /// пользователю
        /// </summary>
        [Required, MaxLength(255)]
        public string Text { get; set; }

        /// <summary>
        /// Дата отправки уведомления
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Пользователь системы, которому отправлено
        /// уведомление
        /// </summary>
        [Required, MaxLength(50)]
        public string EmployeeId { get; set; }
        public Employee Employee { get; set; }

        [Required]
        public int NotificationTypeId { get; set; }
        public NotificationType NotificationType { get; set; }
    }
}
