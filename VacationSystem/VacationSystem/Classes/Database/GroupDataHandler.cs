using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using VacationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace VacationSystem.Classes.Database
{
    /// <summary>
    /// Класс с методами работы с БД, связанными с группами сотрудников
    /// </summary>
    static public class GroupDataHandler
    {
        /// <summary>
        /// Получение списка групп сотрудников
        /// </summary>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <returns>Список групп сотрудников, созданных заданным руководителем</returns>
        static public List<Group> GetGroups(string headId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.Groups
                        .Include(g => g.EmployeesInGroup)
                        .Where(g => g.HeadEmployeeId == headId)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Получение списка групп сотрудников
        /// </summary>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <returns>Список групп сотрудников, созданных заданным руководителем в указанном подразделении</returns>
        static public List<Group> GetGroups(string headId, string depId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.Groups
                        .Include(g => g.EmployeesInGroup)
                        .Where(g => g.HeadEmployeeId == headId && g.DepartmentId == depId)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Получить данные о группе из БД
        /// </summary>
        /// <param name="groupId">Идентификатор группы</param>
        /// <returns>Запись о группе и связанных с ней сотрудниках и подразделениях</returns>
        static public Group GetGroup(int groupId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.Groups
                        .Include(g => g.EmployeesInGroup)
                        .FirstOrDefault(g => g.Id == groupId);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Сохранение новой группы сотрудников в БД
        /// </summary>
        /// <param name="employees">Список сотрудников в группе</param>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <param name="name">Название группы</param>
        /// <param name="description">Описание группы</param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool AddGroup(List<Employee> employees, string headId, string depId, string name, string description)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    Group group = new Group
                    {
                        Name = name,
                        Description = description,
                        Date = DateTime.Now,
                        HeadEmployeeId = headId,
                        DepartmentId = depId
                    };

                    db.Groups.Add(group);
                    db.SaveChanges();

                    foreach (Employee emp in employees)
                    {
                        db.EmployeeInGroups.Add(new EmployeeInGroup
                        {
                            EmployeeId = emp.Id,
                            GroupId = group.Id
                        });
                    }

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

        /// <summary>
        /// Удаление группы сотрудников из БД
        /// </summary>
        /// <param name="id">Идентификатор группы</param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool DeleteGroup(int id)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    db.Groups.Remove(db.Groups.FirstOrDefault(g => g.Id == id));
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

        /// <summary>
        /// Редактирование группы сотрудников
        /// </summary>
        /// <param name="groupId">Идентификатор группы</param>
        /// <param name="name">Наименование группы</param>
        /// <param name="description">Описание группы</param>
        /// <param name="employees">Список сотрудников группы</param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool EditGroup(int groupId, string name, string description, List<Employee> employees)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    // получить редактируемую группу со всеми ее сотрудниками
                    Group group = db.Groups.Include(group => group.EmployeesInGroup).FirstOrDefault(group => group.Id == groupId);
                    if (group == null)
                        return false;

                    // изменить имя и описание групппы
                    group.Name = name;
                    group.Description = description;

                    // проверить всех сотрудников
                    foreach (Employee emp in employees)
                    {
                        // если в группе нет такого сотрудника, то добавить
                        if (!group.EmployeesInGroup.Any(empInGroup => empInGroup.EmployeeId == emp.Id))
                            db.EmployeeInGroups.Add(new EmployeeInGroup
                            {
                                GroupId = group.Id,
                                EmployeeId = emp.Id
                            });
                    }

                    // сотрудники, которых больше нет в группе
                    List<EmployeeInGroup> deletedEmps = group.EmployeesInGroup
                        .Where(empInGroup => !employees.Any(emp => emp.Id == empInGroup.EmployeeId))
                        .ToList();

                    // удалить этих сотрудников
                    db.EmployeeInGroups.RemoveRange(deletedEmps);

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

        /// <summary>
        /// Получение списка групп указанного подразделения
        /// </summary>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <returns>Список групп сотрудников</returns>
        static public List<Group> GetGroupsOfDepartment(string depId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.Groups.Where(g => g.DepartmentId == depId).ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
