using Sgs.Attendance.Reports.Helpers;
using Sgs.Attendance.Reports.Models;
using System;

namespace Sgs.Attendance.Reports.ViewModels
{
    public class EmployeeInfoViewModel
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public string Name { get; set; }

        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; }

        public int? ReportToId { get; set; }

        public DepartmentInfoViewModel DepartmentInfo { get; set; }

        public EmployeeInfoViewModel ReportTo { get; set; }

        public ContractWorkTime ContractWorkTime { get; set; }

        public string ContractWorkTimeText => ContractWorkTime.GetText();

        public string ContractWorkTimeString => ContractWorkTime.ToString();

        public AttendanceProof AttendanceProof { get; set; }

        public string AttendanceProofText => AttendanceProof.GetText();

        public string AttendanceProofString => AttendanceProof.ToString();

        public DateTime DefaultCalendarStartDate { get; set; }

        public string Note { get; set; }
    }
}
