using System;

namespace Sgs.Attendance.Reports.Models
{
    public class CalendarDayReport
    {
        public DateTime DayDate { get; set; }

        public DateTime? CheckInTime { get; set; }

        public DateTime? CheckOutTime { get; set; }

        public TimeSpan? WorkDuration { get; set; }

        public bool IsDayOff { get; set; }

        public string DayOffDescription { get; set; }
    }
}
