using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sgs.Attendance.Reports.Logic;
using Sgs.Attendance.Reports.Models;
using Sgs.Attendance.Reports.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Sgs.Attendance.Reports.Controllers
{
    public class WorkShiftsController : GeneralMvcController<WorkShift, WorkShiftViewModel>
    {

        public WorkShiftsController(WorkShiftsManager dataManager, IMapper mapper
            , ILogger<WorkShiftsController> logger) : base(dataManager, mapper, logger)
        {
        }

        public async Task<IActionResult> UpdateWorkShiftInfo(WorkShiftViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var manager = _dataManager as WorkShiftsManager;

                    if (model.IsDayOff)
                    {
                        model.ShiftStartTime = null;
                        model.ShiftEndTime = null;
                    }

                    if (model.dayOffInRamadan)
                    {
                        model.ShiftStartTimeInRamadan = null;
                        model.ShiftEndTimeInRamadan = null;
                    }

                    if (model.Id == 0)
                    {
                        var newShift = _mapper.Map<WorkShift>(model);
                        await manager.InsertNewAsync(newShift);
                    }
                    else
                    {
                        var currentShift = await manager.GetByIdAsync(model.Id);

                        if (currentShift == null)
                        {
                            return Json(new { errors = new string[] { "لايمكن العثور على بيانات الوردية" } });
                        }

                        _mapper.Map(model, currentShift);

                        await manager.UpdateItemAsync(currentShift);
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
            catch (ValidationException ex)
            {
                return Json(new { errors = new string[] { ex.ValidationResult.ErrorMessage } });
            }
            catch (Exception)
            {
                return Json(new { errors = new string[] { "خطأ أثناء حفظ بيانات الوردية الرجاء المحاولة لاحقاً" } });
            }

        }

    }
}
