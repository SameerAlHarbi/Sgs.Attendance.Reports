using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sgs.Attendance.Reports.ViewModels;

namespace Sgs.Attendance.Reports.Controllers
{
    public class EmployeesDaysReportsController : BaseController
    {
        public EmployeesDaysReportsController(IMapper mapper, ILogger<EmployeesDaysReportsController> logger) : base(mapper, logger)
        {
        }

        public IActionResult Absents()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AbsentsReport(DateTime startDate,DateTime endDate,int[] employeesIds = null)
        {
            try
            {
                var results = new List<EmployeeDayReportViewModel>();

                return PartialView(results);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
