using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using VacationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace VacationSystem.Classes.Database
{
    /// <summary>
    /// Класс для работы с данными в БД
    /// </summary>
    static public class DataHandler
    {
        /// <summary>
        /// Получение администратора по его логину
        /// </summary>
        /// <param name="login">Логин администратора</param>
        /// <returns>Администратор с указанным логином</returns>
        static public Administrator GetAdminByLogin(string login)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    Administrator admin = db.Administrators.FirstOrDefault(a => a.Login == login);
                    return admin;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

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

        /// <summary>
        /// Получение списка заместителей руководителя
        /// </summary>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <returns>Список заместителей указанного руководителя</returns>
        static public List<Deputy> GetDeputies(string headId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.Deputies.Where(d => d.HeadEmployeeId == headId).ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Получение списка заместителей руководителя
        /// </summary>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <returns>Список заместителей руководителя в указанном подразделении</returns>
        static public List<Deputy> GetDeputies(string headId, string depId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.Deputies.Where(d => d.HeadEmployeeId == headId && d.DepartmentId == depId).ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Добавление заместителя руководителя в подразделении
        /// </summary>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <param name="empId">Идентификатор сотрудника-заместителя</param>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool AddDeputy(string headId, string empId, string depId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    Deputy deputy = new Deputy
                    {
                        HeadEmployeeId = headId,
                        DeputyEmployeeId = empId,
                        DepartmentId = depId,
                        Date = DateTime.Now
                    };

                    db.Deputies.Add(deputy);
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
        /// Проверка существования в БД заместителя
        /// </summary>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <param name="empId">Идентификатор сотрудника-заместителя</param>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <returns>true - такой заместитель уже есть; false - такого заместителя в БД нет</returns>
        static public bool CheckDeputy(string headId, string empId, string depId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    if (db.Deputies.Any(d => d.DepartmentId == depId 
                                        && d.DeputyEmployeeId == empId 
                                        && d.HeadEmployeeId == headId))
                        return true;
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Проверка существования в БД заместителя
        /// </summary>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <param name="empId">Идентификатор сотрудника-заместителя</param>
        /// <returns>true - такой заместитель уже есть; false - такого заместителя в БД нет</returns>
        static public bool CheckDeputy(string headId, string empId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    if (db.Deputies.Any(d => d.DeputyEmployeeId == empId
                                          && d.HeadEmployeeId == headId))
                        return true;
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Удаление заместителя руководителя
        /// </summary>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <param name="empId">Идентификатор сотрудника-заместителя</param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool DeleteDeputy(string headId, string empId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    if (CheckDeputy(headId, empId))
                    {
                        List<Deputy> deputyForDelete = db.Deputies.Where(d => d.HeadEmployeeId == headId
                                                                          && d.DeputyEmployeeId == empId)
                                                                  .ToList();
                        db.Deputies.RemoveRange(deputyForDelete);
                        db.SaveChanges();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

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
        /// <returns>Успешность выполнения операции</returns>
        static public bool AddGroup(List<Employee> employees, string headId, string depId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    Group group = new Group
                    {
                        Name = "",
                        Description = "",
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
    }
}