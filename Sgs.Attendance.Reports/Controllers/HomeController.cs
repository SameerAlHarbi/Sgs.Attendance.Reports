using Microsoft.AspNetCore.Mvc;

namespace Sgs.Attendance.Reports.Controllers
{
    public class HomeController:Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
