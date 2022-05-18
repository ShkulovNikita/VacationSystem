using System;
using System.Linq;
using System.Diagnostics;
using VacationSystem.Models;

namespace VacationSystem.Classes.Database
{
    static public class UpdateDataHandler
    {
        static public bool HasDate(DateTime date)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    Update upd = db.Updates.FirstOrDefault(u => u.Date.Day == date.Day 
                    && u.Date.Month == date.Month 
                    && u.Date.Year == date.Year);

                    if (upd == null)
                        return false;
                    else
                        return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        static public bool AddUpdate(DateTime date)
        {
            try
            {
                using (ApplicationContext db = new ApplicationContext())
                {
                    if (HasDate(date))
                        return true;
                    else
                    {
                        Update update = new Update { Date = date };
                        db.Updates.Add(update);
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
    }
}
