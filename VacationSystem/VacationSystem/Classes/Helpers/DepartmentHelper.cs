using System.Collections.Generic;
using System.Linq;
using VacationSystem.Models;
using VacationSystem.ViewModels;
using VacationSystem.ViewModels.ListItems;

namespace VacationSystem.Classes.Helpers
{
    /// <summary>
    /// Класс для различных операций с подразделениями
    /// </summary>
    static public class DepartmentHelper
    {
        /// <summary>
        /// Конвертация объекта подразделения в объект ViewModel
        /// для отображения на странице с информацией о подразделении
        /// </summary>
        /// <param name="dep">Подразделение</param>
        /// <returns>Подразделение в формате ViewModel</returns>
        static public DepartmentViewModel ConvertDepartmentToViewModel(Department dep)
        {
            // получение старшего подразделения
            Department headDep = Connector.GetHeadDepartment(dep.HeadDepartment);

            // получение руководителя подразделения
            Employee headEmp = Connector.GetHeadOfDepartment(dep.Head);

            // получение младших подразделений
            List<Department> lowerDeps = Connector.GetLowerDepartments(dep.Id)
                .OrderBy(d => d.Name)
                .ToList();

            // передать данные о подразделении во ViewModel
            DepartmentViewModel department = new DepartmentViewModel()
            {
                Id = dep.Id,
                Name = dep.Name,
                ChildDepartments = lowerDeps
            };

            if (headDep != null)
                department.HeadDepartment = headDep;
            if (headEmp != null)
                department.Head = headEmp;

            return department;
        }

        /// <summary>
        /// Получение списка подразделений для выпадающего списка 
        /// при выборе заместителя для руководителя
        /// </summary>
        /// <param name="departments">Список подразделений, которыми управляет руководитель</param>
        /// <returns>Список подразделений в формате ViewModel для выпадающего списка</returns>
        static public List<DepListItem> GetDepartmentsList(List<Department> departments)
        {
            List<DepListItem> deps = new List<DepListItem>();

            foreach (Department dep in departments)
            {
                // добавить подразделение в общий список подразделений
                deps.Add(new DepListItem
                {
                    Id = dep.Id,
                    Name = dep.Name
                });
            }

            // отсортировать подразделения и сотрудников по алфавиту
            deps = deps.OrderBy(d => d.Name).ToList();

            return deps;
        }
    }
}
