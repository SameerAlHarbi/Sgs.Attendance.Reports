using Sameer.Shared.Data;
using Sgs.Attendance.Reports.Models;

namespace Sgs.Attendance.Reports.Logic
{
    public class AbsentsNotificationsManager : GeneralManager<AbsentNotification>
    {
        public AbsentsNotificationsManager(IRepository repo) : base(repo)
        {
        }
    }
}
