using System;

namespace Sgs.Attendance.Reports.Models
{
    public class CalendarDayReport
    {

        public DateTime DayDate { get; set; }

        public DateTime? CheckInDateTime { get; set; }

        public DateTime? CheckOutDateTime { get; set; }

        public bool IsDayOff { get; set; }

        public string DayOffDescription { get; set; }

    }
}
