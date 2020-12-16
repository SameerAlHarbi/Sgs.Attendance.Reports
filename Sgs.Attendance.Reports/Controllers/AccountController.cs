using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sgs.Attendance.Reports.Controllers
{
    public class AccountController: Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
