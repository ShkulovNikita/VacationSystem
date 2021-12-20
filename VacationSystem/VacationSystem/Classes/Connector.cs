using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacationSystem.Models;
using System.IO;

namespace VacationSystem.Classes
{
    public class Connector
    {
        // постоянная часть ссылки на API
        private const string Url = "JSON/";

        static public List<Holiday> GetCalendar()
        {
            string path = Url + "calendar.json";
            string calendar = "";

            try
            {
                using (StreamReader sr = new StreamReader(path, System.Text.Encoding.UTF8))
                {
                    calendar = sr.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (calendar != "")
            {
                Parser.ParseHolidays(calendar);
            }

            return null;
        }
    }
}
