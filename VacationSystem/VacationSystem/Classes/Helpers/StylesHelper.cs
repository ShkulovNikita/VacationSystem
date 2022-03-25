using System.Collections.Generic;
using VacationSystem.Models;
using VacationSystem.ViewModels;
using System.Linq;
using System.Diagnostics;
using System;
using VacationSystem.Classes.Database;

namespace VacationSystem.Classes.Helpers
{
    /// <summary>
    /// Класс с различными методами для работы со стилями руководства
    /// </summary>
    static public class StylesHelper
    {
        /// <summary>
        /// Получение списка стилей руководства в формате ViewModel
        /// для отображения на странице с этим списком
        /// </summary>
        /// <param name="deps">Подразделения, которыми управляет руководитель</param>
        /// <param name="styles">Стили руководства, уже примененные руководителем</param>
        /// <returns>Список стилей руководства указанного руководителя в формате ViewModel</returns>
        static public List<HeadStyleViewModel> FindHeadStyles(List<Department> deps, List<HeadStyle> styles)
        {
            try
            {
                // список со стилями руководителя для его подразделений
                List<HeadStyleViewModel> depStyles = new List<HeadStyleViewModel>();

                foreach (Department dep in deps)
                {
                    // найти среди стилей руководителя тот,
                    // который применен к данному подразделению
                    HeadStyle styleDep = styles
                        .Where(s => s.DepartmentId == dep.Id)
                        .OrderByDescending(s => s.Date)
                        .FirstOrDefault();

                    // ничего не найдено - стиль по умолчанию
                    depStyles.Add(GetProperStyle(depStyles.Count, styleDep, dep));
                }

                return depStyles;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return new List<HeadStyleViewModel>();
            }
        }

        /// <summary>
        /// Получение подходящего стиля руководства в формате ViewModel
        /// </summary>
        /// <param name="index">Текущий размер списка со стилями руководителя</param>
        /// <param name="styleDep">Стиль руководства в формате модели</param>
        /// <param name="dep">Подразделение, к которому применен стиль</param>
        /// <returns>Стиль руководства в формате ViewModel</returns>
        static private HeadStyleViewModel GetProperStyle(int index, HeadStyle styleDep, Department dep)
        {
            if (styleDep == null)
            {
                ManagementStyle defaultStyle = DataHandler.GetManagementStyle(3);

                return new HeadStyleViewModel
                {
                    Id = index,
                    Style = defaultStyle,
                    Department = dep
                };
            }
            else
            {
                return new HeadStyleViewModel
                {
                    Id = index,
                    Style = styleDep.ManagementStyle,
                    Department = dep
                };
            }
        }
    }
}
