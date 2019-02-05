using Sameer.Shared.Data;
using Sgs.Attendance.Reports.Models;

namespace Sgs.Attendance.Reports.Logic
{
    public class EmployeesInfoManager : GeneralManager<EmployeeInfo>
    {
        public EmployeesInfoManager(IRepository repo) : base(repo)
        {
        }
    }
}
