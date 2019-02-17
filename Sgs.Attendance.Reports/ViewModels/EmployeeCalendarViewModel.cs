using Sameer.Shared;
using Sgs.Attendance.Reports.Helpers;
using Sgs.Attendance.Reports.Models;
using System;

namespace Sgs.Attendance.Reports.ViewModels
{
    public class EmployeeCalendarViewModel
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public ContractWorkTime ContractWorkTime { get; set; }

        public string ContractWorkTimeText => ContractWorkTime.GetText();

        public string ContractWorkTimeString => ContractWorkTime.ToString();

        public AttendanceProof AttendanceProof { get; set; }

        public string AttendanceProofText => AttendanceProof.GetText();

        public string AttendanceProofString => AttendanceProof.ToString();

        public DateTime? StartDate { get; set; }

        public string StartDateHijri => StartDate.HasValue ? StartDate.Value.ConvertToString(true, true, true) + "هـ " : "";

        public DateTime? EndDate { get; set; }

        public string EndDateHijri => EndDate.HasValue ? EndDate.Value.ConvertToString(true, true, true) + "هـ " : "";

        public string Note { get; set; }
    }
}
