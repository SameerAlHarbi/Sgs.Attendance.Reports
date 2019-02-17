using AutoMapper;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sgs.Attendance.Reports.Logic;
using Sgs.Attendance.Reports.Models;
using Sgs.Attendance.Reports.ViewModels;
using System.Threading.Tasks;

namespace Sgs.Attendance.Reports.Controllers
{
    public class WorkCalendarsController : GeneralMvcController<WorkCalendar, WorkCalendarViewModel>
    {
        public WorkCalendarsController(WorkCalendarsManager dataManager, IMapper mapper
            , ILogger<WorkCalendarsController> logger) : base(dataManager, mapper, logger)
        {
        }

        public virtual async Task<IActionResult> GetAllCalendarsByWorkTimeJsonForKendo([DataSourceRequest] DataSourceRequest request
           , ContractWorkTime contractWorkTime)
        {
            var manager = _dataManager as WorkCalendarsManager;

            var resultDataList = await manager.GetAllAsNoTrackingListAsync(w => w.ContractWorkTime == contractWorkTime);
            var resultDataModelList = await mapToViewModelsDataCollection(resultDataList);
            resultDataModelList = await fillMissingItemsData(resultDataModelList);
            return Json(resultDataModelList.ToDataSourceResult(request));
        }

    }
}
