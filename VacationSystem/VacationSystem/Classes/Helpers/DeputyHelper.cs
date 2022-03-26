using VacationSystem.Models;
using VacationSystem.Classes.Database;
using VacationSystem.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace VacationSystem.Classes.Helpers
{
    /// <summary>
    /// Класс с различными методами для работы с заместителями
    /// </summary>
    static public class DeputyHelper
    {
        /// <summary>
        /// Получение заместителей руководителя (всех или в указанном подразделении)
        /// </summary>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <param name="depId">Идентификатор подразделения (может быть равен null)</param>
        /// <param name="departments">Список подразделений, которыми в данный момент управляет руководитель</param>
        /// <returns>Список заместителей руководителя</returns>
        static public List<Deputy> GetDeputies(string headId, string depId, List<Department> departments)
        {
            List<Deputy> deputies = new List<Deputy>();

            // получить данные о заместителях из БД
            if (depId != null)
                deputies = DataHandler.GetDeputies(headId, depId);
            else
                deputies = DataHandler.GetDeputies(headId);

            if (deputies == null)
                return null;

            // получить заместителей только в тех подразделениях, где руководитель
            // управляет в данный момент
            deputies = deputies
                .Where(deputy => departments.Any(depart => deputy.DepartmentId == depart.Id))
                .ToList();

            return deputies;
        }

        /// <summary>
        /// Преобразование списка заместителей в список в формате ViewModel
        /// для соответствующего представления
        /// </summary>
        /// <param name="deputies">Список заместителей руководителя</param>
        /// <param name="departments">Список подразделений, которыми управляет руководитель</param>
        /// <returns>Список заместителей в формате ViewModel</returns>
        static public List<DeputyViewModel> ConvertDeputiesToViewModel(List<Deputy> deputies, List<Department> departments)
        {
            // список заместителей в формате ViewModel
            List<DeputyViewModel> deputiesList = new List<DeputyViewModel>();

            foreach (Deputy dep in deputies)
            {
                // данные о заместителе как о сотруднике
                Employee depEmp = Connector.GetEmployee(dep.DeputyEmployeeId);

                if (depEmp != null)
                {
                    // получить очередного заместителя в формате ViewModel для добавления в список
                    DeputyViewModel deputy = GetDeputyViewModel(deputies, departments, depEmp);
                    if (deputy == null)
                        continue;
                    else
                        deputiesList.Add(deputy);
                }
            }

            return deputiesList;
        }

        /// <summary>
        /// Преобразование данных о заместителе как о сотруднике в формат ViewModel
        /// </summary>
        /// <param name="deputies">Список заместителей руководителя</param>
        /// <param name="departments">Список подразделений, которыми управляет руководитель</param>
        /// <param name="depEmp">Данные о заместителе как о сотруднике</param>
        /// <returns>Данные о заместителе в формате ViewModel</returns>
        static private DeputyViewModel GetDeputyViewModel(List<Deputy> deputies, List<Department> departments, Employee depEmp)
        {
            // список подразделений, на которые назначен данный заместитель
            List<Department> depsOfDeputy = new List<Department>();
            depsOfDeputy = departments
                .Where(depart => deputies.Any(deputy => deputy.DepartmentId == depart.Id
                                                && deputy.DeputyEmployeeId == depEmp.Id))
                .ToList();

            if (depsOfDeputy.Count == 0)
                return null;

            // сохранить информацию о заместителе во ViewModel
            return new DeputyViewModel
            {
                Id = depEmp.Id,
                FirstName = depEmp.FirstName,
                MiddleName = depEmp.MiddleName,
                LastName = depEmp.LastName,
                Departments = depsOfDeputy
            };
        }

        /// <summary>
        /// Получение списка подразделений для выпадающего списка 
        /// при выборе заместителя для руководителя
        /// </summary>
        /// <param name="departments">Список подразделений, которыми управляет руководитель</param>
        /// <returns>Список подразделений в формате ViewModel для выпадающего списка</returns>
        static public List<DeputyDepViewModel> GetDepartmentsList(List<Department> departments)
        {
            List<DeputyDepViewModel> deps = new List<DeputyDepViewModel>();

            foreach (Department dep in departments)
            {
                // добавить подразделение в общий список подразделений
                deps.Add(new DeputyDepViewModel
                {
                    Id = dep.Id,
                    Name = dep.Name
                });
            }

            // отсортировать подразделения и сотрудников по алфавиту
            deps = deps.OrderBy(d => d.Name).ToList();

            return deps;
        }

        /// <summary>
        /// Получение списка сотрудников для выпадающего списка
        /// при выборе заместителя для руководителя
        /// </summary>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <param name="departments">Список подразделений руководителя в формате ViewModel</param>
        /// <returns>Список сотрудников в формате ViewModel для выпадающего списка</returns>
        static public List<DeputyEmpViewModel> GetEmployeesList(string headId, List<DeputyDepViewModel> departments)
        {
            // получить список сотрудников в формате ViewModel
            List<DeputyEmpViewModel> emps = ConvertEmployeesToViewModel(headId, departments);

            // удалить из списка сотрудников самого руководителя
            emps = emps.Where(e => e.EmpId != headId).ToList();

            // отсортировать сотрудников по алфавиту
            emps = emps.OrderBy(e => e.Name).ToList();

            // присвоить сотрудникам в списке уникальные идентификаторы
            for (int i = 0; i < emps.Count; i++)
                emps[i].Id = i.ToString();

            return emps;
        }

        /// <summary>
        /// Преобразование объекта сотрудника в формат ViewModel
        /// </summary>
        /// <param name="headId">Идентификатор руководителя подразделения</param>
        /// <param name="emp">Сотрудник как объект класса модели</param>
        /// <param name="dep">Подразделение сотрудника</param>
        /// <returns>Объект сотрудника в формате ViewModel для выпадающего списка сотрудников</returns>
        static private DeputyEmpViewModel GetEmployeeViewModel(string headId, Employee emp, DeputyDepViewModel dep)
        {
            // проверить наличие такого заместителя в БД
            if (DataHandler.CheckDeputy(headId, emp.Id, dep.Id))
                return null;

            // новый сотрудник в формате ViewModel
            return new DeputyEmpViewModel
            {
                EmpId = emp.Id,
                Name = emp.LastName + " " + emp.FirstName + " " + emp.MiddleName,
                Department = dep,
                DepartmentId = dep.Id
            };
        }

        /// <summary>
        /// Преобразование список сотрудников в формат ViewModel для вывода 
        /// выпадающего списка сотрудников
        /// </summary>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <param name="departments">Список подразделений руководителя</param>
        /// <returns>Список сотрудников в формате ViewModel</returns>
        static private List<DeputyEmpViewModel> ConvertEmployeesToViewModel(string headId, List<DeputyDepViewModel> departments)
        {
            List<DeputyEmpViewModel> emps = new List<DeputyEmpViewModel>();

            // получить сотрудников всех подразделений
            foreach (DeputyDepViewModel dep in departments)
            {
                // получить всех сотрудников данного подразделения
                List<Employee> employees = Connector.GetEmployeesOfDepartment(dep.Id);
                if (employees == null)
                    continue;

                // добавить сотрудников в списки
                foreach (Employee emp in employees)
                {
                    DeputyEmpViewModel newEmp = GetEmployeeViewModel(headId, emp, dep);
                    if (newEmp == null)
                        continue;

                    // добавить в список сотрудников подразделения
                    dep.Employees.Add(newEmp);

                    // добавить в список всех сотрудников
                    emps.Add(newEmp);
                }
            }

            return emps;
        }
    }
}