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

        public async Task<IActionResult> DeleteEmployeeCalendar(int calendarId)
        {
            try
            {

                var saveResult = await _employeesCalendarsManager.DeleteDataItem(calendarId);

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

        public virtual async Task<IActionResult> GetDataByIdJson(int calendarId)
        {
            try
            {
                var dataItem = await _employeesCalendarsManager.GetDataById(calendarId);

                if (dataItem == null)
                {
                    return Json(new { errors = new string[] { "NotFound" } });
                }

                var dataItemVM = _mapper.Map<EmployeeCalendarViewModel>(dataItem);
                return Json(dataItemVM);
            }
            catch (Exception)
            {
                return Json(new { errors = new string[] { "خطأ أثناء قراءة البيانات" } });
            }
        }

        public async Task<IActionResult> AddOrUpdateEmployeeCalendar(int employeeId, int calendarId,bool isOpenCalendar ,DateTime startDate,DateTime endDate, ContractWorkTime workTime, AttendanceProof attendanceProof, string note)
        {
            try
            {

                var newData = new EmployeeCalendar
                {
                    Id = calendarId,
                    StartDate = startDate,
                    EndDate = isOpenCalendar? default(DateTime?) : endDate,
                    ContractWorkTime = workTime,
                    AttendanceProof = attendanceProof,
                    EmployeeId = employeeId,
                    Note = note,
                };

                if (User.Identity.IsAuthenticated)
                {
                    newData.UserId = User.Identity.Name.ConvertToInteger();
                }

                if(newData.Id == 0)
                {
                    var saveResult = await _employeesCalendarsManager.InsertNewAsync(newData);
                    if (saveResult.Status == RepositoryActionStatus.Created)
                    {
                        return Json("ok");
                    }
                    else
                    {
                        return Json(new { errors = "Error" });
                    }
                }
                else
                {
                    var currentData = await _employeesCalendarsManager.GetByIdAsync(newData.Id);

                    if(currentData  == null)
                    {
                        return Json(new { errors = "NotFound" });
                    }

                    currentData.StartDate = newData.StartDate;
                    currentData.EndDate = newData.EndDate;
                    currentData.ContractWorkTime = newData.ContractWorkTime;
                    currentData.AttendanceProof = newData.AttendanceProof;
                    currentData.Note = newData.Note;
                    currentData.UserId = newData.UserId;

                    var updateResult = await _employeesCalendarsManager.UpdateDataItem(currentData);
                    if (updateResult.Status == RepositoryActionStatus.Updated)
                    {
                        return Json("ok");
                    }
                    else
                    {
                        return Json(new { errors = "Error" });
                    }
                }
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrWhiteSpace(ex.Message))
                {
                    if (ex.Message.Contains("before"))
                    {
                        return Json(new { errors = "enddate" });
                    }
                }
                return Json(new { errors = "Error" });
            }
        }
    }
}
