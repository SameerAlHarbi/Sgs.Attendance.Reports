using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sameer.Shared;
using Sgs.Attendance.Reports.Logic;
using Sgs.Attendance.Reports.Models;
using Sgs.Attendance.Reports.Services;
using Sgs.Attendance.Reports.ViewModels;
using System;
using System.Threading.Tasks;
using Sgs.Attendance.Reports.Helpers;
using Kendo.Mvc.UI;
using System.Collections.Generic;
using System.Linq;
using Kendo.Mvc.Extensions;

namespace Sgs.Attendance.Reports.Controllers
{
    public class EmployeesCalendarsController : BaseController
    {
        private readonly EmployeesCalendarsManager _employeesCalendarsManager;
        private readonly IErpManager _erpManager;

        public EmployeesCalendarsController(EmployeesCalendarsManager employeesCalendarsManager
                , IErpManager erpManager, IMapper mapper, ILogger<EmployeesCalendarsController> logger)
            : base(mapper, logger)
        {
            _employeesCalendarsManager = employeesCalendarsManager;
            _erpManager = erpManager;
        }

        public async Task<JsonResult> UpdateDefaultCalendar(EmployeeCalendarViewModel model)
        {
            try
            {
                var currentData = await _employeesCalendarsManager.GetSingleItemAsync(c => c.EmployeeId == model.EmployeeId && c.StartDate == model.StartDate && !c.EndDate.HasValue);

                currentData = currentData ?? new EmployeeCalendar
                {
                    EmployeeId = model.EmployeeId
                };

                currentData.ContractWorkTime = model.ContractWorkTime;
                currentData.AttendanceProof = model.AttendanceProof;
                currentData.StartDate = model.StartDate.Value.Date;
                currentData.Note = model.Note;

                if (User.Identity.IsAuthenticated)
                {
                    currentData.UserId = User.Identity.Name.ConvertToInteger();
                }

                if (currentData.Id == 0)
                {
                    var updateResult = await _employeesCalendarsManager.InsertNewAsync(currentData);
                    if (updateResult.Status == RepositoryActionStatus.Created)
                    {
                        return Json("ok");
                    }
                    else
                    {
                        return Json("error");
                    }
                }
                else
                {
                    var updateResult = await _employeesCalendarsManager.UpdateDataItem(currentData);
                    if (updateResult.Status == RepositoryActionStatus.Updated)
                    {
                        return Json("ok");
                    }
                    else
                    {
                        return Json("error");
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public virtual async Task<IActionResult> GetAllEmployeeCalendarsJsonForKendo([DataSourceRequest] DataSourceRequest request
            , int employeeId)
        {
            try
            {
                var allDataList = await _employeesCalendarsManager.GetAllAsNoTrackingListAsync(e => e.EmployeeId == employeeId);

                var resultData = _mapper.Map<List<EmployeeCalendarViewModel>>(allDataList);

                if (resultData != null && resultData.Count > 0)
                {

                    try
                    {
                        var erpData = await _erpManager.GetEmployeesInfo(new int[] { employeeId });
                        var empInfo = erpData?.FirstOrDefault();
                        if (empInfo != null)
                        {
                            foreach (var empCalendar in resultData)
                            {
                                empCalendar.EmployeeName = empInfo.Name;
                            }
                        }

                    }
                    catch (Exception)
                    {
                    }
                }

                return Json(resultData.OrderBy(c => c.StartDate).ToDataSourceResult(request));
            }
            catch (Exception ex)
            {
                return Json(new DataSourceResult() { Errors = ex.Message });
            }
        }
    }
}
