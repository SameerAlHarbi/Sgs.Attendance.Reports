using System;

namespace Sgs.Attendance.Reports.ViewModels
{
    public class VacationViewModel
    {
        public int EmployeeId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string VacationTypeName { get; set; }

        public DateTime RegisterDate { get; set; }
    }
}
