using AutoMapper;
using Microsoft.Extensions.Logging;
using Sgs.Attendance.Reports.Logic;
using Sgs.Attendance.Reports.Models;
using Sgs.Attendance.Reports.ViewModels;

namespace Sgs.Attendance.Reports.Controllers
{
    public class WorkShiftsController : GeneralMvcController<WorkShift, WorkShiftViewModel>
    {
        public WorkShiftsController(WorkShiftsManager dataManager, IMapper mapper
            , ILogger<WorkShiftsController> logger) : base(dataManager, mapper, logger)
        {
        }
    }
}
