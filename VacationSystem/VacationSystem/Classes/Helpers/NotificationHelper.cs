﻿using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using VacationSystem.Models;
using VacationSystem.Classes.Database;
using VacationSystem.Classes.Data;

namespace VacationSystem.Classes.Helpers
{
    /// <summary>
    /// Класс с методами для работы с уведомлениями пользователям системы
    /// </summary>
    static public class NotificationHelper
    {
        /// <summary>
        /// Добавление уведомления о скором уходе сотрудника в отпуск
        /// </summary>
        /// <param name="empId">Идентификатор сотрудника</param>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <param name="vacationId">Идентификатор отпуска</param>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <param name="number">Количество дней, через которые сотрудник уйдет в отпуск</param>
        static public void AddEmployeeLeave(string empId, string headId, string depId, int vacationId, int number)
        {
            try
            {
                // получить объекты для уведомления
                Employee employee = Connector.GetEmployee(empId);
                Department dep = Connector.GetDepartment(depId);
                SetVacation vacation = VacationDataHandler.GetSetVacation(vacationId);

                if ((employee == null) || (vacation == null) || (dep == null))
                    return;

                // если все объекты успешно получены, то создать уведомление
                string text = "Сотрудник " + EmployeeHelper.GetFullName(employee)
                    + " из подразделения \"" + dep.Name + "\""
                    + " уходит в отпуск (" + vacation.StartDate.ToString("yyyy-MM-dd") + " - " + vacation.EndDate.ToString("yyyy-MM-dd")
                    + ") через " + number + " дней";

                NotificationDataHandler.AddNotification(text, headId, "Уход в отпуск");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Добавление уведомления о скором выходе сотрудника из отпуска
        /// </summary>
        /// <param name="empId">Идентификатор сотрудника</param>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <param name="vacationId">Идентификатор отпуска</param>
        /// <param name="number">Количество дней, через которые сотрудник выйдет из отпуска</param>
        static public void AddEmployeeeReturn(string empId, string headId, string depId, int vacationId, int number)
        {
            try
            {
                // получить объекты для уведомления
                Employee employee = Connector.GetEmployee(empId);
                Department dep = Connector.GetDepartment(depId);
                SetVacation vacation = VacationDataHandler.GetSetVacation(vacationId);

                if ((employee == null) || (vacation == null) || (dep == null))
                    return;

                // если все объекты успешно получены, то создать уведомление
                string text = "Сотрудник " + EmployeeHelper.GetFullName(employee)
                    + " из подразделения \"" + dep.Name + "\""
                    + " выходит из отпуска (" + vacation.StartDate.ToString("yyyy-MM-dd") + " - " + vacation.EndDate.ToString("yyyy-MM-dd")
                    + ") через " + number + " дней";

                NotificationDataHandler.AddNotification(text, headId, "Выход из отпуска");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Добавление уведомления о выборе сотрудником 
        /// </summary>
        /// <param name="empId">Идентификатор сотрудника</param>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <param name="depId">Идентификатор подразделения</param>
        static public void AddChoosingPeriods(string empId, string headId, string depId)
        {
            try
            {
                // получить объекты для уведомления
                Employee employee = Connector.GetEmployee(empId);
                Department dep = Connector.GetDepartment(depId);

                if ((employee == null) || (dep == null))
                    return;

                // если все объекты успешно получены, то создать уведомление
                string text = "Сотрудник " + EmployeeHelper.GetFullName(employee)
                    + " из подразделения \"" + dep.Name + "\""
                    + " выбрал желаемые периоды для отпуска";

                NotificationDataHandler.AddNotification(text, headId, "Выбор периода отпуска");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Добавление уведомления об утверждении отпуска сотрудника
        /// </summary>
        /// <param name="empId">Идентификатор сотрудника</param>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <param name="parts">Утвержденные периоды отпуска сотрудника</param>
        static public void AddSetVacation(string empId, string headId, List<VacationPart> parts)
        {
            try
            {
                Employee head = Connector.GetEmployee(headId);

                if (head == null)
                    return;
                if (parts == null)
                    return;
                if (parts.Count == 0)
                    return;

                string text = "Ваши отпуска:\n";
                foreach (VacationPart part in parts)
                {
                    text += "• " + part.StartDate.ToString("yyyy-MM-dd") + " - " + part.EndDate.ToString("yyyy-MM-dd") + "\n";
                }
                text += "были утверждены руководителем " + EmployeeHelper.GetFullName(head);

                NotificationDataHandler.AddNotification(text, empId, "Утверждение отпуска");

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Добавление уведомления о прерывании отпуска сотрудника
        /// </summary>
        /// <param name="empId">Идентификатор сотрудника</param>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <param name="vacationId">Идентификатор отпуска</param>
        static public void AddVacationInterrupt(string empId, string headId, int vacationId)
        {
            try
            {
                Employee head = Connector.GetEmployee(headId);
                SetVacation vacation = VacationDataHandler.GetSetVacation(vacationId);

                if ((head == null) || (vacation == null))
                    return;

                string text = "Ваш отпуск был прерван руководителем " + EmployeeHelper.GetFullName(head)
                    + ", новые даты отпуска: " + vacation.StartDate.ToString("yyyy-MM-dd") + " - " + vacation.EndDate.ToString("yyyy-MM-dd");

                NotificationDataHandler.AddNotification(text, empId, "Прерывание отпуска");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Добавление уведомления об отмене отпуска
        /// </summary>
        /// <param name="empId">Идентификатор сотрудника</param>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <param name="startDate">Начальная дата отпуска</param>
        /// <param name="endDate">Конечная дата отпуска</param>
        static public void AddVacationCancel(string empId, string headId, DateTime startDate, DateTime endDate)
        {
            try
            {
                Employee head = Connector.GetEmployee(headId);

                if (head == null)
                    return;

                string text = "Ваш отпуск (" + startDate.ToString("yyyy-MM-dd") + " - " + endDate.ToString("yyyy-MM-dd")
                    + ") был отменен руководителем " + EmployeeHelper.GetFullName(head);

                NotificationDataHandler.AddNotification(text, empId, "Отмена отпуска");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Добавление уведомления об изменении утвержденного отпуска
        /// </summary>
        /// <param name="empId">Идентификатор сотрудника</param>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <param name="vacationId">Идентификатор изменяемого отпуска</param>
        /// <param name="vacation">Новые периоды отпуска</param>
        static public void AddVacationChange(string empId, string headId, int vacationId, ChosenVacation newVacation)
        {
            try
            {
                Employee head = Connector.GetEmployee(headId);
                SetVacation vacation = VacationDataHandler.GetSetVacation(vacationId);

                if ((head == null) || (vacation == null))
                    return;
                if (newVacation.Periods == null)
                    return;
                if (newVacation.Periods.Length == 0)
                    return;

                string text = "Ваш отпуск (" + vacation.StartDate.ToString("yyyy-MM-dd") + " - " + vacation.EndDate.ToString("yyyy-MM-dd")
                    + ") был изменен руководителем " + EmployeeHelper.GetFullName(head)
                    + ". Новые периоды отпуска:\n";
                foreach (ChosenPeriod period in newVacation.Periods)
                    text += "• " + period.StartDate.ToString("yyyy-MM-dd") + " - " + period.EndDate.ToString("yyyy-MM-dd") + "\n";

                NotificationDataHandler.AddNotification(text, empId, "Перемещение отпуска");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Добавление уведомления с сообщением от подчиненного сотрудника
        /// </summary>
        /// <param name="empId">Идентификатор сотрудника</param>
        /// <param name="headId">Идентификатор руководителя</param>
        /// <param name="depId">Идентификатор подразделения</param>
        /// <param name="message">Сообщение от сотрудника</param>
        static public void AddMessage(string empId, string headId, string depId, string message)
        {
            try
            {
                Employee emp = Connector.GetEmployee(empId);
                Department dep = Connector.GetDepartment(depId);

                if ((emp == null) || (dep == null))
                    return;

                string text = "Сообщение от сотрудника " + EmployeeHelper.GetFullName(emp)
                    + " (подразделение \"" + dep.Name + "\"):\n"
                    + message;

                NotificationDataHandler.AddNotification(text, headId, "Заявка на изменение отпуска");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
