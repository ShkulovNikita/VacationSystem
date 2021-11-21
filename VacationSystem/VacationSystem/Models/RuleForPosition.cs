using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace VacationSystem.Models
{
    /// <summary>
    /// Правила, устанавливаемые на должности
    /// сотрудников в отделениях
    /// </summary>
    public class RuleForPosition
    {
        public int Id { get; set; }

        // количество сотрудников определенной должности,
        // которые должны находиться одновременно на рабочем месте
        public int PeopleNumber { get; set; }
    }
}
