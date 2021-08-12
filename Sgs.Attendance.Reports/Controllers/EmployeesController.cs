using AutoMapper;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sgs.Attendance.Reports.Logic;
using Sgs.Attendance.Reports.Models;
using Sgs.Attendance.Reports.Services;
using Sgs.Attendance.Reports.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Sgs.Attendance.Reports.Controllers
{
    [Authorize(Roles = "Admin,AttendanceDepartment")]
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

        public async Task<IActionResult> employeesReports()
        {
            try
            {
                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> employeesReports(DateTime startDate, DateTime endDate
         , int[] employeesIds = null, string[] departments = null, string shiftType = "All"
         , string attendanceType = "All", int? pageNumber = null, int pageSize = 31)
        {
            startDate = startDate.Date;
            endDate = endDate.Date;

            employeesIds = employeesIds ?? new int[] { };
            departments = departments ?? new string[] { };

            var employeesIdsList = employeesIds.ToList();

            if (departments.Count() > 0)
            {
                var departmentsEmployees = new List<EmployeeInfoViewModel>();
                foreach (var department in departments)
                {
                    departmentsEmployees.AddRange(await _erpManager.GetDepartmentEmployeesInfo(department, active: null));
                }

                if (departmentsEmployees.Any())
                {
                    employeesIdsList.AddRange(departmentsEmployees.Select(d => d.EmployeeId));
                }
            }

            employeesIds = employeesIdsList.Distinct().OrderBy(e => e).ToArray();

            IEnumerable<EmployeeInfoViewModel> results = await _erpManager.GetEmployeesInfo(employeesIds, 
                fromDate: startDate.ToString("yyyy-MM-dd", new CultureInfo("en-US")),
                toDate: endDate.ToString("yyyy-MM-dd", new CultureInfo("en-US")), active: true);

            results = await setupEmployeesInfo(results.OrderBy(d => d.DepartmentName), DateTime.Now);

            if (shiftType != "All")
            {
                Enum.TryParse(shiftType, out ContractWorkTime workTime);
                results = results.Where(r => r.ContractWorkTime == workTime);
            }

            if (attendanceType != "All")
            {
                Enum.TryParse(attendanceType, out AttendanceProof attendanceProof);
                results = results.Where(r => r.AttendanceProof == attendanceProof);
            }

            return PartialView("employeesReport", results.OrderBy(r => r.EmployeeId));
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

            var allDataList = await _erpManager.GetDepartmentEmployeesInfo(deptCode, active: null);
            
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
                var resultData = await _erpManager.GetEmployeesInfo(new int[] { id}, active: null);

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
                    results = await this._erpManager.GetEmployeesInfo(new int[] { empId},active: null);
                }
                else
                {
                    results = await this._erpManager.GetEmployeesInfo(employeeName: employeeId ,active: null);
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
            IEnumerable<EmployeeInfoViewModel> results = await _erpManager.GetEmployeesInfo(active: null);
            return Json(results.Select(e => e.Name).OrderBy(e => e).Distinct());
        }

        public async Task<JsonResult> GetEmployeeInfoJson(int employeeId)
        {
            try
            {
                IEnumerable<EmployeeInfoViewModel> results = await _erpManager.GetEmployeesInfo(new int[] { employeeId }, active: null);
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

        public async Task<ActionResult> ShortEmployeesInfoJson()
        {
            IEnumerable<ShortEmployeeInfoViewModel> results = await _erpManager.GetShortEmployeesInfo(active: null);
            return Json(results.OrderBy(e => e.EmployeeId).ToList());
        }

        public async Task<ActionResult> ShortEmployeesInfoByTextJson(string text)
        {
            try
            {
                
                if (string.IsNullOrWhiteSpace(text))
                {
                    return Json(new List<EmployeeInfoViewModel>());
                }

                IEnumerable<EmployeeInfoViewModel> results = new List<EmployeeInfoViewModel>();

                if (int.TryParse(text, out int empId))
                {
                    results = await _erpManager.GetEmployeesInfo(new int[] { empId }, active: null);
                }
                else
                {
                    results = await _erpManager.GetEmployeesInfo(active: null);
                    results = results.Where(e => e.Name.Trim().ToLower().Contains(text)).ToList();
                }

                return Json(results);
            }
            catch (Exception ex)
            {
                return Json(new { Errors = new string[] { "Errors" } });
            }
        }

        public async Task<ActionResult> ShortEmployeesInfoForKendo([DataSourceRequest]DataSourceRequest request)
        {
            IEnumerable<EmployeeInfoViewModel> results = await _erpManager.GetEmployeesInfo(active: null);
            return Json(results.OrderBy(e => e.EmployeeId).ToList().ToDataSourceResult(request));
        }

    }
}
