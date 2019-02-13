using System;

namespace Sgs.Attendance.Reports.ViewModels
{
    public class VacationRequestViewModel
    {
        public int EmployeeId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime RequestDate { get; set; }
    }
}
