using System.Collections.Generic;
using VacationSystem.Models;
using VacationSystem.ViewModels;
using VacationSystem.Classes.Database;

namespace VacationSystem.Classes.Helpers
{
    /// <summary>
    /// Класс с различными методами для работы 
    /// с правилами выбора отпусков
    /// </summary>
    static public class RuleHelper
    {
        /// <summary>
        /// Получить список всех правил указанного руководителя
        /// </summary>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <returns>Список всех правил, установленных указанным руководителем 
        /// в его подразделении/подразделениях</returns>
        static public List<RuleViewModel> GetRulesList(string headId, string depId)
        {
            List<RuleViewModel> result = new List<RuleViewModel>();

            // добавить все правила в список
            result.AddRange(ConvertEmployeeRulesToVmList(headId, depId));
            result.AddRange(ConvertPositionRulesToVmList(headId, depId));
            result.AddRange(ConvertGroupRulesToVmList(headId, depId));

            return result;
        }

        /// <summary>
        /// Получение списка правил для сотрудников в формате списка View Model
        /// </summary>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <returns>Список правил для сотрудников в формате View Model списка</returns>
        static public List<RuleViewModel> ConvertEmployeeRulesToVmList(string headId, string depId)
        {
            List<EmployeeRule> rules;

            if (depId == null)
                rules = DataHandler.GetEmployeeRules(headId);
            else
                rules = DataHandler.GetEmployeeRules(headId, depId);

            List<RuleViewModel> result = new List<RuleViewModel>();
            foreach(EmployeeRule rule in rules)
            {
                Department dep = Connector.GetDepartment(rule.DepartmentId);

                result.Add(new RuleViewModel
                {
                    Id = rule.Id,
                    Description = rule.Description,
                    Department = dep,
                    Date = rule.Date,
                    RuleType = "Emp",
                    SystemDescription = "Правило для " + rule.EmployeeInRules.Count.ToString() + " сотрудников подразделения"
                });
            }

            return result;
        }

        /// <summary>
        /// Получение списка правил для должностей в формате списка ViewModel
        /// </summary>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <returns>Список правил для должностей в формате View Model списка</returns>
        static public List<RuleViewModel> ConvertPositionRulesToVmList(string headId, string depId)
        {
            List<RuleForPosition> rules;

            if (depId == null)
                rules = DataHandler.GetPositionRules(headId);
            else
                rules = DataHandler.GetPositionRules(headId, depId);

            List<RuleViewModel> result = new List<RuleViewModel>();
            foreach (RuleForPosition rule in rules) 
            {
                Department dep = Connector.GetDepartment(rule.DepartmentId);
                Position pos = Connector.GetPosition(rule.PositionId);

                if ((dep != null) && (pos != null))
                {
                    result.Add(new RuleViewModel
                    {
                        Id = rule.Id,
                        Description = rule.Description,
                        Department = dep,
                        Date = rule.Date,
                        RuleType = "Pos",
                        SystemDescription = "Правило для " + rule.PeopleNumber.ToString() + " человек должности \"" + pos.Name + "\""
                    });
                }
            }

            return result;
        }

        /// <summary>
        /// Получение списка правил для групп в формате списка ViewModel
        /// </summary>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <returns>Список правил для групп в формате View Model списка</returns>
        static public List<RuleViewModel> ConvertGroupRulesToVmList(string headId, string depId)
        {
            List<GroupRule> rules;

            if (depId == null)
                rules = DataHandler.GetGroupRules(headId);
            else
                rules = DataHandler.GetGroupRules(headId, depId);

            List<RuleViewModel> result = new List<RuleViewModel>();
            foreach (GroupRule rule in rules)
            {
                Department dep = Connector.GetDepartment(rule.Group.DepartmentId);
                
                if (dep != null)
                {
                    result.Add(new RuleViewModel
                    {
                        Id = rule.Id,
                        Description = rule.Description,
                        Department = dep,
                        Date = rule.Date,
                        RuleType = "Group",
                        SystemDescription = "Правило для группы " + rule.Group.Name + " в подразделении " + dep.Name
                    });
                }
            }

            return result;
        }
    }
}
