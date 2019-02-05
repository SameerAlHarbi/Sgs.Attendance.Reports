using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sgs.Attendance.Reports.Services;
using System;
using System.Threading.Tasks;

namespace Sgs.Attendance.Reports.Controllers
{
    public class DepartmentsController : BaseController
    {
        private readonly IErpManager _erpManager;

        public DepartmentsController(IErpManager erpManager,
            IMapper mapper, ILogger<DepartmentsController> logger)
            : base(mapper, logger)
        {
            _erpManager = erpManager;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var allDataList = await _erpManager.GetAllDepartmentsInfo();
                return View(allDataList);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
