using Sameer.Shared.Data;
using Sgs.Attendance.Reports.Models;

namespace Sgs.Attendance.Reports.Logic
{
    public class ProcessingsRequestsManager : GeneralManager<ProcessingRequest>
    {
        public ProcessingsRequestsManager(IRepository repo) : base(repo)
        {
        }
    }
}
