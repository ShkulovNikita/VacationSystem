﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;

using VacationSystem.Models;
using VacationSystem.ViewModels;
using VacationSystem.Classes;
using VacationSystem.Classes.Database;
using VacationSystem.Classes.Helpers;
using VacationSystem.ViewModels.ListItems;

namespace VacationSystem.Controllers
{
    public class VacationController : Controller
    {
        /// <summary>
        /// Отображение списка отпусков пользователя
        /// </summary>
        public IActionResult Index()
        {
            // получить идентификатор пользователя
            string id = HttpContext.Session.GetString("id");
            if (id == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        /// <summary>
        /// Отображение страницы с выбором периодов отпусков
        /// </summary>
        [HttpGet]
        public IActionResult AddVacation()
        {
            // получить идентификатор пользователя
            string id = HttpContext.Session.GetString("id");
            if (id == null)
            {
                TempData["Error"] = "Не удалось загрузить данные пользователя";
                return RedirectToAction("Index");
            }

            // получить количество доступных отпускных дней пользователя
            List<VacationDay> days = VacationDayDataHandler.GetAvailableVacationDays(id);
            int availableDays = VacationDayHelper.CountAvailableDays(days);

            HttpContext.Session.SetInt32("available_days", availableDays);

            return View();
        }

        /// <summary>
        /// Сохранение в БД выбранных сотрудником периодов отпуска
        /// </summary>
        /// <param name="startDates">Начальные даты отпуска</param>
        /// <param name="endDates">Конечные даты отпуска</param>
        [HttpPost]
        public IActionResult AddVacation(DateTime[] startDates, DateTime[] endDates)
        {
            return View();
        }
    }
}
