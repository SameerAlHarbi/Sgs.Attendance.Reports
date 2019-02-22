using Sameer.Shared;
using System;
using System.Globalization;

namespace Sgs.Attendance.Reports.ViewModels
{
    public class CalendarDayReportViewModel
    {

        public DateTime DayDate { get; set; }

        public string DayDateHijriString => DayDate.ConvertToString(true);

        public string DayDateName => DayDate.ToString("dddd",new CultureInfo("ar-SA"));

        public DateTime? CheckInTime { get; set; }

        public string CheckInDayName => CheckInTime.HasValue ? CheckInTime.Value.ToString("dddd", new CultureInfo("ar-SA")):"";

        public string CheckInTimeHijri => CheckInTime.HasValue ? CheckInTime.Value.ConvertToString(true, false, false, "yyyy/MM/dd hh:mm tt") : "";

        public DateTime? CheckOutTime { get; set; }

        public string CheckOutDayName => CheckOutTime.HasValue ? CheckOutTime.Value.ToString("dddd", new CultureInfo("ar-SA")) : "";

        public string CheckOutTimeHijri => CheckOutTime.HasValue ? CheckOutTime.Value.ConvertToString(true, false, false, "yyyy/MM/dd hh:mm tt") : "";

        public TimeSpan? WorkDuration { get; set; }

        public bool IsDayOff { get; set; }

        public string DayOffDescription { get; set; }

        public string WorkType => IsDayOff ? "إجازة" : "عمل";

    }
}
