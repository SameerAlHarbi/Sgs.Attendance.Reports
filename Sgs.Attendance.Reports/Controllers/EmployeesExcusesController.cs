using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sgs.Attendance.Reports.Logic;
using Sgs.Attendance.Reports.Models;
using Sgs.Attendance.Reports.Services;
using Sgs.Attendance.Reports.ViewModels;
using Sameer.Shared;
using Sgs.Attendance.Reports.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace Sgs.Attendance.Reports.Controllers
{
    [Authorize(Roles = "Admin,AttendanceDepartment")]
    public class EmployeesExcusesController : BaseController
    {
        private readonly EmployeesExcusesManager _employeesExcusesManager;
        private readonly IErpManager _erpManager;

        public EmployeesExcusesController(EmployeesExcusesManager employeesExcusesManager
            ,IErpManager erpManager,IMapper mapper, ILogger<EmployeesExcusesController> logger) 
            : base(mapper, logger)
        {
            _employeesExcusesManager = employeesExcusesManager;
            _erpManager = erpManager;
        }

        public virtual async Task<IActionResult> GetAllEmployeeExcusesJsonForKendo([DataSourceRequest] DataSourceRequest request
            , int employeeId)
        {
            var allExcusesDataList = await _employeesExcusesManager.GetAllAsNoTrackingListAsync(e => e.EmployeeId == employeeId);

            var resultData = _mapper.Map<List<EmployeeExcuseViewModel>>(allExcusesDataList);

            if (resultData != null && resultData.Count > 0)
            {

                try
                {
                    var erpData = await _erpManager.GetEmployeesInfo(new int[] { employeeId });
                    var empInfo = erpData?.FirstOrDefault();
                    if (empInfo != null)
                    {
                        foreach (var empExcuse in resultData)
                        {
                            empExcuse.EmployeeName = empInfo.Name;
                        }
                    }

                }
                catch (Exception)
                {
                }
            }

            return Json(resultData.ToDataSourceResult(request));
        }

        public async Task<IActionResult> AddEmployeeExcuse(int employeeId, DateTime excuseDate, ExcuseType excuseType, TimeSpan excuseHours, string note)
        {
            try
            {
                var exHours = excuseHours.ConvertToDouble();

                var newData = new EmployeeExcuse { EmployeeId = employeeId,
                    ExcuseType = excuseType, ExcueseDate = excuseDate.Date,
                    ExcuseHours = exHours, Note = note, RegisterDate = DateTime.Now };

                if (User.Identity.IsAuthenticated)
                {
                    newData.UserId = User.Identity.Name.ConvertToInteger();
                }

                var saveResult = await _employeesExcusesManager.InsertNewDataItem(newData);

                if (saveResult.Status == RepositoryActionStatus.Created)
                {
                    return Json("Ok");
                }
                else
                {
                    return Json(new { errors = "Error" });
                }

            }
            catch (Exception)
            {
                return Json(new { errors = "Error" });
            }
        }

        public async Task<IActionResult> DeleteEmployeeExcuse(int excuseId)
        {
            try
            {

                var saveResult = await _employeesExcusesManager.DeleteDataItem(excuseId);

                if (saveResult.Status == RepositoryActionStatus.Deleted)
                {
                    return Json("Ok");
                }
                else
                {
                    return Json(new { errors = "Error" });
                }

            }
            catch (Exception)
            {
                return Json(new { errors = "Error" });
            }
        }
    }
}
