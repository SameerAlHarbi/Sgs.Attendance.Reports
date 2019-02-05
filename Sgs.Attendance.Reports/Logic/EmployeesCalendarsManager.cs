using Sameer.Shared.Data;
using Sgs.Attendance.Reports.Models;

namespace Sgs.Attendance.Reports.Logic
{
    public class EmployeesCalendarsManager : GeneralManager<EmployeeCalendar>
    {
        public EmployeesCalendarsManager(IRepository repo) : base(repo)
        {
        }
    }
}
