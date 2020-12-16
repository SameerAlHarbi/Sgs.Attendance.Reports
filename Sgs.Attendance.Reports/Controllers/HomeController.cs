using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.DirectoryServices.AccountManagement;

namespace Sgs.Attendance.Reports.Controllers
{
    [Authorize(Roles = "Admin,AttendanceDepartment")]
    public class HomeController:Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
