using Sameer.Shared.Data;
using Sgs.Attendance.Reports.Models;

namespace Sgs.Attendance.Reports.Logic
{
    public class EmployeesDaysReportsManager : GeneralManager<EmployeeDayReport>
    {
        public EmployeesDaysReportsManager(IRepository repo) 
            : base(repo)
        {
        }
    }
}
