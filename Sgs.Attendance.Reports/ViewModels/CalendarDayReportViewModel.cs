using System;

namespace Sgs.Attendance.Reports.ViewModels
{
    public class CalendarDayReportViewModel
    {

        public DateTime DayDate { get; set; }

        public DateTime? CheckInTime { get; set; }

        public DateTime? CheckOutTime { get; set; }

        public TimeSpan? WorkDuration { get; set; }

        public bool IsDayOff { get; set; }

        public string DayOffDescription { get; set; }

        public string WorkType => IsDayOff ? "إجازة" : "عمل";

    }
}
