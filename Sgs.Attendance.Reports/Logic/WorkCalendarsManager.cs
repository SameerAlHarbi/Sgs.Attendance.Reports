using Sameer.Shared.Data;
using Sgs.Attendance.Reports.Models;

namespace Sgs.Attendance.Reports.Logic
{
    public class WorkCalendarsManager : GeneralManager<WorkCalendar>
    {
        public WorkCalendarsManager(IRepository repo)
            : base(repo)
        {
        }
    }
}
