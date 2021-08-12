using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sgs.Attendance.Reports.Logic;
using Sgs.Attendance.Reports.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sgs.Attendance.Reports.Controllers
{
    [AllowAnonymous]
    public class AttendanceSummaryController : BaseController
    {
        private readonly EmployeesDaysReportsManager _employeesDaysReportsManager;
        private readonly AbsentsNotificationsManager _absentsNotificationsManager;
        private readonly RequestsReportsManager _requestsReportsManager;
        private readonly IErpManager _erpManager;
        private readonly IHostingEnvironment _env;

        public AttendanceSummaryController(IHostingEnvironment environment, EmployeesDaysReportsManager employeesDaysReportsManager,
            IErpManager erpManager, IMapper mapper, ILogger<EmployeesDaysReportsController> logger) : base(mapper, logger)
        {
            _employeesDaysReportsManager = employeesDaysReportsManager;
            _erpManager = erpManager;
            _env = environment;
        }

        [AllowAnonymous]
        public async Task<IActionResult> GetAllDayesReports()
        {
            return Json("Ok");
        }
    }
}
