using AutoMapper;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sgs.Attendance.Reports.Logic;
using Sgs.Attendance.Reports.Models;
using Sgs.Attendance.Reports.Services;
using Sgs.Attendance.Reports.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sgs.Attendance.Reports.Controllers
{
    public class EmployeesController : BaseController
    {
        private readonly EmployeesCalendarsManager _employeesCalendarsManager;
        private readonly EmployeesExcusesManager _employeesExcusesManager;
        private readonly IErpManager _erpManager;

        public EmployeesController(EmployeesCalendarsManager employeesCalendarsManager
            , EmployeesExcusesManager employeesExcusesManager
            ,IErpManager erpManager,IMapper mapper, ILogger<EmployeesController> logger)
            : base(mapper, logger)
        {
            _employeesCalendarsManager = employeesCalendarsManager;
            _employeesExcusesManager = employeesExcusesManager;
            _erpManager = erpManager;
        }

        private async Task<List<EmployeeInfoViewModel>> setupEmployeesInfo(IEnumerable<EmployeeInfoViewModel> employeesInfoList,DateTime setupDate)
        {
            try
            {
                var ids = employeesInfoList.Select(e => e.EmployeeId).ToList();

                var employeesCalenars = await _employeesCalendarsManager.GetAll(c => ids.Contains(c.EmployeeId) && setupDate >= c.StartDate
                    && ( !c.EndDate.HasValue || setupDate <= c.EndDate.Value)).OrderByDescending(c => c.StartDate).AsNoTracking().ToListAsync();

                foreach (var empInfo in employeesInfoList)
                {
                    var defaultCalendar = employeesCalenars.FirstOrDefault(c => c.EmployeeId == empInfo.EmployeeId && !c.EndDate.HasValue);
                    var employeeCalendar = employeesCalenars.FirstOrDefault(c => c.EmployeeId == empInfo.EmployeeId && c.EndDate.HasValue);

                    defaultCalendar = defaultCalendar ?? new EmployeeCalendar
                    {
                        AttendanceProof = AttendanceProof.RequiredInOut,
                        ContractWorkTime = ContractWorkTime.Default,
                        StartDate = new DateTime(2018, 12, 1)
                    };

                    empInfo.AttendanceProof = employeeCalendar?.AttendanceProof ?? defaultCalendar.AttendanceProof;
                    empInfo.ContractWorkTime = employeeCalendar?.ContractWorkTime ?? defaultCalendar.ContractWorkTime;
                    empInfo.Note = employeeCalendar?.Note ?? defaultCalendar.Note;
                    empInfo.DefaultCalendarStartDate = defaultCalendar.StartDate;
                }

                return employeesInfoList.ToList() ;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<JsonResult> GetDepartmentEmployees([DataSourceRequest] DataSourceRequest request, string deptCode)
        {
            if (string.IsNullOrWhiteSpace(deptCode))
            {
                return Json(new List<object>() { });
            }

            var allDataList = await _erpManager.GetDepartmentEmployeesInfo(deptCode);
            
            allDataList = await setupEmployeesInfo(allDataList.OrderBy(d => d.DepartmentName),DateTime.Now);

            var result = allDataList.ToTreeDataSourceResult(request,
                e => e.EmployeeId,
                e => e.ReportToId,
                e => e);

            return Json(result);
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var resultData = await _erpManager.GetEmployeesInfo(new int[] { id});

                if(resultData == null || resultData.Count <1)
                {
                    return NotFound();
                }

                resultData = await setupEmployeesInfo(resultData,DateTime.Today);

                return View(resultData.First());

            }
            catch (Exception)
            {
                throw;
            }
        }

        public IActionResult EmployeeDetails()
        {
            return View();
        }

        public async Task<JsonResult> GetAllEmployeesForAutoCompleteKendoWedget([DataSourceRequest]DataSourceRequest request
            , string employeeId = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(employeeId))
                {
                    return Json(new List<EmployeeInfoViewModel>());
                }
                IEnumerable<EmployeeInfoViewModel> results = new List<EmployeeInfoViewModel>();

                if (int.TryParse(employeeId, out int empId))
                {
                    results = await this._erpManager.GetEmployeesInfo(new int[] { empId});
                }
                else
                {
                    results = await this._erpManager.GetEmployeesInfo(employeeName: employeeId);
                }

                return this.Json(results);
            }
            catch (Exception ex)
            {
                return Json(new DataSourceResult() { Errors = ex.Message });
            }
        }

        public async Task<ActionResult> EmployeesNamesJson()
        {
            IEnumerable<EmployeeInfoViewModel> results = await _erpManager.GetEmployeesInfo();
            return Json(results.Select(e => e.Name).OrderBy(e => e).Distinct());
        }

        public async Task<JsonResult> GetEmployeeInfoJson(int employeeId)
        {
            try
            {
                IEnumerable<EmployeeInfoViewModel> results = await _erpManager.GetEmployeesInfo(new int[] { employeeId });
                if(results == null || results.Count() <1 )
                {
                    return Json(new { errors = "لايمكن العثور على بيانات الموظف" });
                }
                return Json(results.FirstOrDefault());
            }
            catch (Exception)
            {
                return Json(new { errors="خطأ اثناء قراءة البيانات"});
            }
        }
    }
}
