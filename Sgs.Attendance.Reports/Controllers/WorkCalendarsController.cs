using AutoMapper;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sgs.Attendance.Reports.Logic;
using Sgs.Attendance.Reports.Models;
using Sgs.Attendance.Reports.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Sgs.Attendance.Reports.Controllers
{
    public class WorkCalendarsController : GeneralMvcController<WorkCalendar, WorkCalendarViewModel>
    {
        public WorkCalendarsController(WorkCalendarsManager dataManager, IMapper mapper
            , ILogger<WorkCalendarsController> logger) : base(dataManager, mapper, logger)
        {
        }

        protected override async Task<WorkCalendarViewModel> fillMissingItemData(WorkCalendarViewModel dataItem)
        {
            var result = await base.fillMissingItemData(dataItem);

            if (!dataItem.IsVacationCalendar && !dataItem.EndDate.HasValue)
            {
                result.IsOpenDuration = true;
            }

            return result;
        }

        public async Task<IActionResult> GetAllCalendarsByWorkTimeJsonForKendo([DataSourceRequest] DataSourceRequest request
           , ContractWorkTime contractWorkTime)
        {
            var manager = _dataManager as WorkCalendarsManager;

            var resultDataList = await manager.GetAllAsNoTrackingListAsync(w => w.ContractWorkTime == contractWorkTime);
            var resultDataModelList = await mapToViewModelsDataCollection(resultDataList);
            resultDataModelList = await fillMissingItemsData(resultDataModelList);
            return Json(resultDataModelList.ToDataSourceResult(request));
        }

        public async Task<IActionResult> UpdateCalendarsInfo(WorkCalendarViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var manager = _dataManager as WorkCalendarsManager;

                    if(!model.IsVacationCalendar && model.IsOpenDuration)
                    {
                        model.EndDate = null;
                    }

                    if (model.Id == 0)
                    {
                        var newCalendar = _mapper.Map<WorkCalendar>(model);

                        await manager.InsertNewAsync(newCalendar); 
                    }
                    else
                    {
                        var currentCalendar = await manager.GetByIdAsync(model.Id);

                        if(currentCalendar==null)
                        {
                            return Json(new { errors = new string[] { "لايمكن العثور على بيانات الوردية" } });
                        }

                        _mapper.Map(model, currentCalendar);

                        await manager.UpdateItemAsync(currentCalendar);
                    }
                    return Json("Ok");
                }

                var errors = new List<string>();
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        if (!string.IsNullOrWhiteSpace(error.ErrorMessage))
                        {
                            errors.Add(error.ErrorMessage);
                        }
                    }
                }

                if (errors.Count < 1)
                    errors.Add("خطأ الرجاء المحاولة لاحقاً");
                return Json(new { errors = errors.ToArray() });
                
            }
            catch(ValidationException ex)
            {
                return Json(new { errors = new string[] { ex.ValidationResult.ErrorMessage } });
            }
            catch (System.Exception)
            {
                return Json(new { errors = new string[] { "خطأ أثناء حفظ بيانات التقويم الرجاء المحاولة لاحقاً"} });
            }
        }

        public async Task<IActionResult> GetCalendarDaysForKendo([DataSourceRequest] DataSourceRequest request,int calendarId
            ,DateTime? fromDate=null,DateTime? toDate = null)
        {
            try
            {
                var manager = _dataManager as WorkCalendarsManager;
                var currentCalendar = await manager.GetByIdAsync(calendarId, c => c.WorkShifts);
                if(currentCalendar == null)
                {
                    return Json(new { errors = new string[] { "لايمكن العثور على بيانات التقويم" } });
                }

                var results = currentCalendar.GetDaysReports(fromDate, toDate);

                return Json(_mapper.Map<List<CalendarDayReportViewModel>>(results).ToDataSourceResult(request));
            }
            catch (ValidationException ex)
            {
                return Json(new { errors = new string[] { ex.ValidationResult.ErrorMessage } });
            }
            catch (System.Exception)
            {
                return Json(new { errors = new string[] { "خطأ أثناء قراءة البيانات الرجاء المحاولة لاحقاً" } });
            }
        }

        public async Task<IActionResult> GetCalendarsDaysForKendo([DataSourceRequest] DataSourceRequest request, ContractWorkTime workTime
            , DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {

                var manager = _dataManager as WorkCalendarsManager;

                var results = await manager.GetCalendarsDaysReport(workTime, fromDate, toDate);

                return Json(_mapper.Map<List<CalendarDayReportViewModel>>(results).ToDataSourceResult(request));
            }
            catch (ValidationException ex)
            {
                return Json(new { errors = new string[] { ex.ValidationResult.ErrorMessage } });
            }
            catch (System.Exception)
            {
                return Json(new { errors = new string[] { "خطأ أثناء قراءة البيانات الرجاء المحاولة لاحقاً" } });
            }
        }



    }
}
