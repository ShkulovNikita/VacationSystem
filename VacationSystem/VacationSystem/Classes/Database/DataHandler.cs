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
        /// Получение списка правил выбора отпусков, накладываемых на сотрудников
        /// </summary>
        /// <param name="headId">Идентификатор руководителя, установившего правила</param>
        /// <returns>Список правил выбора отпусков для сотрудников</returns>
        static public List<EmployeeRule> GetEmployeeRules(string headId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.EmployeeRules
                        .Include(er => er.EmployeeInRules)
                        .Where(er => er.HeadEmployeeId == headId)
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
        /// Получение списка правил выбора отпусков, накладываемых на сотрудников
        /// </summary>
        /// <param name="headId">Идентификатор руководителя, установившего правила</param>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <returns>Список правил выбора отпусков для сотрудников</returns>
        static public List<EmployeeRule> GetEmployeeRules(string headId, string depId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.EmployeeRules
                        .Include(er => er.EmployeeInRules)
                        .Where(er => er.HeadEmployeeId == headId
                        && er.DepartmentId == depId)
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
        /// Получение списка правил выбора отпусков для должностей
        /// </summary>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <returns>Список правил для должностей</returns>
        static public List<RuleForPosition> GetPositionRules(string headId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.RuleForPositions
                        .Where(pr => pr.HeadEmployeeId == headId)
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
        /// Получение списка правил выбора отпусков для должностей
        /// </summary>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <returns>Список правил для должностей</returns>
        static public List<RuleForPosition> GetPositionRules(string headId, string depId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.RuleForPositions
                        .Where(pr => pr.HeadEmployeeId == headId
                        && pr.DepartmentId == depId)
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
        /// Получение списка правил выбора отпусков для групп сотрудников
        /// </summary>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <returns>Список правил выбора отпусков для групп сотрудников</returns>
        static public List<GroupRule> GetGroupRules(string headId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.GroupRules
                        .Include(gr => gr.Group)
                        .Where(gr => gr.Group.HeadEmployeeId == headId)
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
        /// Получение списка правил выбора отпусков для групп сотрудников
        /// </summary>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <returns>Список правил выбора отпусков для групп сотрудников</returns>
        static public List<GroupRule> GetGroupRules(string headId, string depId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.GroupRules
                            .Include(gr => gr.Group)
                            .Where(gr => gr.Group.HeadEmployeeId == headId
                            && gr.Group.DepartmentId == depId)
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
        /// Добавление в БД нового правила выбора отпусков для сотрудников
        /// </summary>
        /// <param name="description">Описание правила</param>
        /// <param name="type">Идентификатор типа правила</param>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <param name="employees">Список задействованных правилом сотрудников</param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool AddEmployeesRule(string description, int type, string depId, string headId, List<Employee> employees)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    EmployeeRule rule = new EmployeeRule
                    {
                        Description = description,
                        RuleTypeId = type,
                        DepartmentId = depId,
                        HeadEmployeeId = headId,
                        Date = DateTime.Now
                    };

                    db.EmployeeRules.Add(rule);
                    db.SaveChanges();

                    foreach (Employee employee in employees)
                    {
                        rule.EmployeeInRules.Add(new EmployeeInRule
                        {
                            EmployeeRuleId = rule.Id,
                            EmployeeId = employee.Id,
                            Date = DateTime.Now
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
        /// Добавление в БД нового правила выбора отпусков для должностей
        /// </summary>
        /// <param name="number">Количество людей одной должности, которые должны быть на рабочем месте</param>
        /// <param name="description">Описание правила</param>
        /// <param name="posId">Идентификатор задействованной должности</param>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool AddPositionRule(int number, string description, string posId, string depId, string headId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    db.RuleForPositions.Add(new RuleForPosition
                    {
                        PeopleNumber = number,
                        Description = description,
                        PositionId = posId,
                        DepartmentId = depId,
                        HeadEmployeeId = headId,
                        Date = DateTime.Now
                    });

                    db.SaveChanges();

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
        /// Добавление в БД нового правила выбора отпусков для групп сотрудников
        /// </summary>
        /// <param name="description">Описание правила</param>
        /// <param name="type">Идентификатор типа правила</param>
        /// <param name="groupId">Идентификатор задействованной группы</param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool AddGroupRule(string description, int type, int groupId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    db.GroupRules.Add(new GroupRule
                    {
                        Description = description,
                        RuleTypeId = type,
                        GroupId = groupId,
                        Date = DateTime.Now
                    });

                    db.SaveChanges();

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
        /// Удаление из БД правила для сотрудников
        /// </summary>
        /// <param name="ruleId">Идентификатор правила</param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool DeleteEmployeesRule(int ruleId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    EmployeeRule rule = db.EmployeeRules.FirstOrDefault(r => r.Id == ruleId);
                    if (rule != null)
                    {
                        db.EmployeeRules.Remove(rule);
                        db.SaveChanges();
                    }
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
        /// Удаление из БД правила для должностей
        /// </summary>
        /// <param name="ruleId">Идентификатор правила</param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool DeletePositionRule(int ruleId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    RuleForPosition rule = db.RuleForPositions.FirstOrDefault(r => r.Id == ruleId);
                    if (rule != null)
                    {
                        db.RuleForPositions.Remove(rule);
                        db.SaveChanges();
                    }
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
        /// Удаление из БД правила для групп
        /// </summary>
        /// <param name="ruleId">Идентификатор правила</param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool DeleteGroupRule(int ruleId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    GroupRule rule = db.GroupRules.FirstOrDefault(r => r.Id == ruleId);
                    if (rule != null)
                    {
                        db.GroupRules.Remove(rule);
                        db.SaveChanges();
                    }
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
        /// Редактирование правила выбора отпусков для сотрудников
        /// </summary>
        /// <param name="ruleId">Идентификатор правила</param>
        /// <param name="description">Описание правила</param>
        /// <param name="employees">Список сотрудников, затрагиваемых правилом</param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool EditEmployeesRule(int ruleId, string description, List<Employee> employees)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    // получить редактируемое правило
                    EmployeeRule rule = db.EmployeeRules
                        .Include(r => r.EmployeeInRules)
                        .FirstOrDefault(r => r.Id == ruleId);
                    if (rule == null)
                        return false;

                    rule.Description = description;

                    foreach (Employee emp in employees)
                    {
                        // если в правиле нет текущего сотрудника, то добавить
                        if (!rule.EmployeeInRules.Any(empInRule => empInRule.EmployeeId == emp.Id))
                            db.EmployeeInRules.Add(new EmployeeInRule
                            {
                                EmployeeRuleId = rule.Id,
                                EmployeeId = emp.Id,
                                Date = DateTime.Now
                            });
                    }

                    // сотрудники, которых больше нет в правиле
                    List<EmployeeInRule> deletedEmps = rule.EmployeeInRules
                        .Where(empInRule => !employees.Any(emp => emp.Id == empInRule.EmployeeId))
                        .ToList();

                    // удалить этих сотрудников
                    db.EmployeeInRules.RemoveRange(deletedEmps);

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
        /// Редактирование правила выбора отпусков для должности
        /// </summary>
        /// <param name="ruleId">Идентификатор правила</param>
        /// <param name="number">Количество сотрудников должности, которые должны быть на рабочем месте</param>
        /// <param name="description">Описание правила</param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool EditPositionRule(int ruleId, int number, string description)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    RuleForPosition rule = db.RuleForPositions.FirstOrDefault(r => r.Id == ruleId);
                    if (rule == null)
                        return false;

                    rule.Description = description;
                    rule.PeopleNumber = number;

                    db.SaveChanges();

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
        /// Редактирование правила выбора отпусков для группы сотрудников
        /// </summary>
        /// <param name="ruleId">Идентификатор правила</param>
        /// <param name="description">Описание правила</param>
        /// <returns>Успешность выполнения операции</returns>
        static public bool EditGroupRule(int ruleId, string description)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    GroupRule rule = db.GroupRules.FirstOrDefault(r => r.Id == ruleId);
                    if (rule == null)
                        return false;

                    rule.Description = description;

                    db.SaveChanges();

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
        /// Получение всех типов правил выбора отпусков в БД
        /// </summary>
        /// <returns>Список всех типов правил</returns>
        static public List<RuleType> GetRuleTypes()
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.RuleTypes.ToList();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Получение правила для сотрудников по идентификатору
        /// </summary>
        /// <param name="ruleId">Идентификатор правила</param>
        /// <returns>Правило выбора отпусков для сотрудников</returns>
        static public EmployeeRule GetEmployeeRule(int ruleId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.EmployeeRules
                        .Include(er => er.EmployeeInRules)
                        .Include(er => er.RuleType)
                        .FirstOrDefault(er => er.Id == ruleId);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Получение правила для должности по идентификатору
        /// </summary>
        /// <param name="ruleId">Идентификатор правила</param>
        /// <returns>Правило выбора отпусков для должности</returns>
        static public RuleForPosition GetPositionRule(int ruleId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.RuleForPositions
                        .FirstOrDefault(rp => rp.Id == ruleId);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Получение правила для группы по идентификатору
        /// </summary>
        /// <param name="ruleId">Идентификатор правила</param>
        /// <returns>Правило выбора отпусков для группы</returns>
        static public GroupRule GetGroupRule(int ruleId)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    return db.GroupRules
                        .Include(gr => gr.Group)
                        .FirstOrDefault(gr => gr.Id == ruleId);
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