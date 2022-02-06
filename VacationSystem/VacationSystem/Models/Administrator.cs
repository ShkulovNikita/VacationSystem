using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationSystem.Models
{
    /// <summary>
    /// Администратор системы отпусков
    /// </summary>
    
    [Table("administrators")]
    public class Administrator
    {
        [Key, Required, MaxLength(50)]
        public string Id { get; set; }

        /// <summary>
        /// Логин администратора
        /// </summary>
        [Required, MaxLength(50)]
        public string Login { get; set; }

        /// <summary>
        /// Пароль администратора
        /// </summary>
        [Required, MaxLength(50)]
        public string Password { get; set; }
    }
}