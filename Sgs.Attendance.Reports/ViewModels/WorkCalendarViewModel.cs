using Sameer.Shared;
using Sgs.Attendance.Reports.Helpers;
using Sgs.Attendance.Reports.Models;
using System;

namespace Sgs.Attendance.Reports.ViewModels
{
    public class WorkCalendarViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public string StartDateHijri => StartDate.ConvertToString(true, true, true) + "هـ ";

        public DateTime? EndDate { get; set; }

        public string EndDateHijri => EndDate.HasValue ? EndDate.Value.ConvertToString(true, true, true) + "هـ " : string.Empty;

        public ContractWorkTime ContractWorkTime { get; set; }

        public string ContractWorkTimeText => ContractWorkTime.GetText();

        public string ContractWorkTimeString => ContractWorkTime.ToString();

        public bool IsVacationCalendar { get; set; }

        public string WorkCalendarType => !IsVacationCalendar ? "تقويم إجازة" : "تقويم عمل";

        public string Note { get; set; }
    }
}
