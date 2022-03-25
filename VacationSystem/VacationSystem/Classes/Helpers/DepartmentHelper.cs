using System.Collections.Generic;
using System.Linq;
using VacationSystem.Models;
using VacationSystem.ViewModels;

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
    }
}
