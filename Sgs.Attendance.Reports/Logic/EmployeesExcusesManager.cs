using Sameer.Shared.Data;
using Sgs.Attendance.Reports.Models;

namespace Sgs.Attendance.Reports.Logic
{
    public class EmployeesExcusesManager : GeneralManager<EmployeeExcuse>
    {
        public EmployeesExcusesManager(IRepository repo) : base(repo)
        {
        }
    }
}
