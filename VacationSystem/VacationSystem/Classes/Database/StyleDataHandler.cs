using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using VacationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace VacationSystem.Classes.Database
{
    /// <summary>
    /// Класс с методами работы с БД, связанными со стилями управления
    /// и назначения отпусков руководителя
    /// </summary>
    static public class StyleDataHandler
    {
        /// <summary>
        /// Получение выбранных руководителем стилей руководства
        /// его подразделениями
        /// </summary>
        /// <param name="id">Идентификатор руководителя</param>
        /// <returns>Список назначенных стилей руководства указанного руководителя</returns>
        static public List<HeadStyle> GetHeadStyles(string id)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    List<HeadStyle> styles = db.HeadStyles
                        .Include(style => style.ManagementStyle)
                        .Where(style => style.HeadEmployeeId == id)
                        .ToList();

                    return styles;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Получение стиля руководства указанного руководителя
        /// в выбранном подразделении
        /// </summary>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <returns>Стиль руководства в указанном подразделении</returns>
        static public HeadStyle GetHeadStyle(string headId, string depId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    HeadStyle style = db.HeadStyles
                        .OrderByDescending(s => s.Date)
                        .Include(s => s.ManagementStyle)
                        .FirstOrDefault(s => s.HeadEmployeeId == headId && s.DepartmentId == depId);
                    return style;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Получение стиля руководства по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор стиля руководства</param>
        /// <returns>Стиль руководства с указанным идентификатором</returns>
        static public ManagementStyle GetManagementStyle(int id)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.ManagementStyles.FirstOrDefault(s => s.Id == id);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Получение всех стилей руководства из БД
        /// </summary>
        /// <returns>Список возможных стилей руководства</returns>
        static public List<ManagementStyle> GetManagementStyles()
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.ManagementStyles.ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Применение стиля управления к подразделению
        /// </summary>
        /// <param name="headId">Идентификатор руководителя подразделения</param>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <param name="styleId">Идентификатор стиля управления</param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool AddHeadStyle(string headId, string depId, int styleId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    HeadStyle style = new HeadStyle
                    {
                        HeadEmployeeId = headId,
                        DepartmentId = depId,
                        ManagementStyleId = styleId,
                        Date = DateTime.Now
                    };

                    db.HeadStyles.Add(style);
                    db.SaveChanges();
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
