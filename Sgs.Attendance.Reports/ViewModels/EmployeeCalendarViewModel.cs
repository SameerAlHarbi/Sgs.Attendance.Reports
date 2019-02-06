using Sgs.Attendance.Reports.Models;
using System;

namespace Sgs.Attendance.Reports.ViewModels
{
    public class EmployeeCalendarViewModel
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public ContractWorkTime ContractWorkTime { get; set; }

        public AttendanceProof AttendanceProof { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Note { get; set; }
    }
}
