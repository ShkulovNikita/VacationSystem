using Microsoft.AspNetCore.Mvc;

namespace VacationSystem.Controllers
{
    /// <summary>
    /// Контроллер, отвечающий за назначение дней отпусков сотрудникам
    /// </summary>
    public class VacationDayController : Controller
    {
        /// <summary>
        /// Страница для назначения дней отпусков сотрудникам,
        /// подчиненным авторизованному руководителю
        /// </summary>
        public IActionResult Index()
        {
            // получить идентификатор руководителя

            // получить подразделения руководителя

            // получить сотрудников подразделений руководителя

            // получить уже назначенные дни отпуска сотрудников

            // собрать все во ViewModel

            return View();
        }

        /// <summary>
        /// Сохранение назначенных дней отпуска в БД
        /// </summary>
        /// <param name="department">Выбранное подразделение руководителя</param>
        /// <param name="employees">Сотрудники, которым добавляются дни отпуска</param>
        /// <param name="type">Выбранный тип отпуска</param>
        /// <param name="number">Количество дней отпуска</param>
        /// <param name="notes">Пояснения</param>
        public IActionResult SetDays(string department, string[] employees, int type, int number, string notes)
        {
            return View();
        }


    }
}
