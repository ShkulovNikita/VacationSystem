using Microsoft.AspNetCore.Mvc;

namespace VacationSystem.Controllers
{
    public class VacationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
