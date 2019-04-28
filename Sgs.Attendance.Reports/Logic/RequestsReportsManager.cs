using Sameer.Shared.Data;
using Sgs.Attendance.Reports.Models;

namespace Sgs.Attendance.Reports.Logic
{
    public class RequestsReportsManager : GeneralManager<RequestReport>
    {
        public RequestsReportsManager(IRepository repo) : base(repo)
        {
        }
    }
}
