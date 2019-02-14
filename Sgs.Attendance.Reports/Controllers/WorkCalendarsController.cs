using AutoMapper;
using Microsoft.Extensions.Logging;
using Sameer.Shared;
using Sgs.Attendance.Reports.Models;
using Sgs.Attendance.Reports.ViewModels;

namespace Sgs.Attendance.Reports.Controllers
{
    public class WorkCalendarsController : GeneralMvcController<WorkCalendar, WorkCalendarViewModel>
    {
        public WorkCalendarsController(IDataManager<WorkCalendar> dataManager, IMapper mapper
            , ILogger<GeneralMvcController<WorkCalendar, WorkCalendarViewModel>> logger) : base(dataManager, mapper, logger)
        {
        }
    }
}
