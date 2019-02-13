using Sameer.Shared.Data;
using Sgs.Attendance.Reports.Models;

namespace Sgs.Attendance.Reports.Logic
{
    public class WorkShiftsManager : GeneralManager<WorkShift>
    {
        public WorkShiftsManager(IRepository repo) : base(repo)
        {
        }
    }
}