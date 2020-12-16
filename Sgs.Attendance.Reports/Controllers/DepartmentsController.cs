﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sgs.Attendance.Reports.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sgs.Attendance.Reports.Controllers
{
    [Authorize(Roles = "Admin,AttendanceDepartment")]
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

        public async Task<JsonResult> GetAllDepartmentsJson(string id)
        {
            try
            {
                var allDataList = await _erpManager.GetChildsDepartmentsInfo(id);
                return Json(allDataList
                        .Select(d => new {
                            id = d.Code,
                            name = d.Name,
                            hasChildren = d.ChildsCount
                        }));
            }
            catch (Exception)
            {
                return Json(new { errors = "sorry" });
            }
        }
    }
}
