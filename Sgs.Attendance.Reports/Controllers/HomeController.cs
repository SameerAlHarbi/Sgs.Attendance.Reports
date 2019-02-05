using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sgs.Attendance.Reports.Controllers
{
    public class HomeController:Controller
    {
        private readonly IHostingEnvironment _env;

        public HomeController(IHostingEnvironment env)
        {
            this._env = env;
        }
        public IActionResult Index()
        {
            var path = Path.Combine(_env.ContentRootPath, "node_modules");
            var fileProvider = new PhysicalFileProvider(path);

            //ViewBag.path = path;
            return View();
        }
    }
}
