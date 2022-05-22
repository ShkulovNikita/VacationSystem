using VacationSystem.Classes.Data;

namespace VacationSystem.Classes
{
    /// <summary>
    /// Класс с методами, отвечающими за проверку отпусков сотрудников
    /// </summary>
    static public class VacationChecker
    {
        /// <summary>
        /// Проверка корректности выбранного отпуска
        /// </summary>
        /// <param name="vacation">Отпуск с его периодами</param>
        /// <returns>Сообщение об ошибке либо успешности операции</returns>
        static public string CheckVacationPeriods(ChosenVacation vacation)
        {
            string errors = "";
            bool isValid = true;

            // проверить пустоту значений
            if (!CheckEmptiness(vacation))
            {
                return "Не выбраны даты отпуска";
            }
            // проверить порядок начальных и конечных дат периодов
            if (!CheckOrder(vacation.Periods))
            {
                errors += "• Неправильный порядок дат начала и конца периодов отпуска\n";
                isValid = false;
            }
            // проверить, не включены ли периоды внутрь друг друга
            if (!CheckInclusions(vacation.Periods))
            {
                errors += "• Выбраны периоды, включенные внутрь других периодов\n";
                isValid = false;
            }
            // проверить пересечения периодов
            if (!CheckIntersections(vacation.Periods))
            {
                errors += "• Выбраны пересекающиеся периоды\n";
                isValid = false;
            }

            if (isValid)
                return "success";
            else
                return errors;
        }

        /// <summary>
        /// Проверить пустоту значений в выбранном отпуске
        /// </summary>
        /// <param name="vacation">Отпуск с его периодами</param>
        /// <returns>Результат проверки</returns>
        static private bool CheckEmptiness(ChosenVacation vacation)
        {
            if (vacation == null)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Проверить порядок дат в периодах
        /// </summary>
        /// <param name="periods">Периоды внутри отпуска</param>
        /// <returns>Результат проверки</returns>
        static private bool CheckOrder(ChosenPeriod[] periods)
        {
            for (int i = 0; i < periods.Length; i++)
            {
                if (periods[i].StartDate > periods[i].EndDate)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Проверить, что даты не включены внутрь друг друга
        /// </summary>
        /// <param name="periods">Периоды внутри отпуска</param>
        /// <returns>Результат проверки</returns>
        static private bool CheckInclusions(ChosenPeriod[] periods)
        {
            // счетчик первой пары для сравнения
            for (int i = 0; i < periods.Length - 1; i++)
                // счетчик второй пары для сравнения
                for (int j = i + 1; j < periods.Length; j++)
                {
                    // если первый период включен во второй
                    if ((periods[i].StartDate > periods[j].StartDate)
                        && (periods[i].EndDate < periods[j].EndDate))
                        return false;
                    // если второй период включен в первый
                    if ((periods[i].StartDate < periods[j].StartDate)
                        && (periods[i].EndDate > periods[j].EndDate))
                        return false;
                }

            return true;
        }

        /// <summary>
        /// Проверить, что периоды не пересекаются
        /// </summary>
        /// <param name="periods">Периоды внутри отпуска</param>
        /// <returns>Результат проверки</returns>
        static private bool CheckIntersections(ChosenPeriod[] periods)
        {
            for (int i = 0; i < periods.Length - 1; i++)
                for (int j = i + 1; j < periods.Length; j++)
                {
                    // находится ли какая-то дата одного периода внутри другого периода
                    if (
                        ((periods[i].StartDate > periods[j].StartDate)
                        &&
                        (periods[i].StartDate < periods[j].EndDate))
                        ||
                        ((periods[i].EndDate > periods[j].StartDate)
                        &&
                        (periods[i].EndDate < periods[j].EndDate))
                        ||
                        ((periods[j].StartDate > periods[i].StartDate)
                        &&
                        (periods[j].StartDate < periods[i].EndDate))
                        ||
                        ((periods[j].EndDate > periods[i].StartDate)
                        &&
                        (periods[j].EndDate < periods[i].EndDate))
                    )
                    // если да, то искать пересечения
                    {
                        // пересечения с одной стороны
                        if ((periods[i].StartDate > periods[j].StartDate)
                        && (periods[i].EndDate > periods[j].EndDate))
                            return false;
                        // пересечения с другой стороны
                        if ((periods[i].StartDate < periods[j].StartDate)
                        && (periods[i].EndDate < periods[j].EndDate))
                            return false;
                    }
                }

            return true;
        }

        /// <summary>
        /// Проверить на соответствие ТК РФ
        /// </summary>
        /// <param name="vacation">Отпуск с его периодами</param>
        /// <returns>Сообщение об ошибке либо успешности операции</returns>
        static public string CheckLawRules(string empId, ChosenVacation vacation, string errors)
        {
            if (errors == null)
                errors = "";
            bool isValid = true;

            if (!Check14Days(empId, vacation.Periods))
            {
                errors += "• Хотя бы одна из частей отпуска должна быть не менее 14 дней (ТК РФ, Ст. 125)";
                isValid = false;
            }

            if (isValid)
                return "success";
            else
                return errors;
        }

        /// <summary>
        /// Проверить, есть ли хотя бы один период отпуска, включающий в себя минимум 14 дней
        /// </summary>
        /// <param name="periods">Периоды внутри отпуска</param>
        /// <returns>Результат проверки</returns>
        static private bool Check14Days(string empId, ChosenPeriod[] periods)
        {
            foreach(ChosenPeriod period in periods)
            {
                // получить количество дней в периоде
                int numberOfDays = period.EndDate.Subtract(period.StartDate).Days + 1;
                if (numberOfDays >= 14)
                    return true;
            }

            return false;
        }
    }
}