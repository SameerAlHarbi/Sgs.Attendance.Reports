using Sameer.Shared;
using System;
using System.Globalization;

namespace Sgs.Attendance.Reports.ViewModels
{
    public class CalendarDayReportViewModel
    {

        public DateTime DayDate { get; set; }

        public string DayDateHijri => DayDate.Date.ConvertToString(true);

        public string DayDateName => DayDate.Date.ToString("dddd",new CultureInfo("ar-SA"));

        public DateTime? CheckInDateTime { get; set; }

        public string CheckInDateHijri => CheckInDateTime.HasValue ? CheckInDateTime.Value.ConvertToString(true) : "";

        public string CheckInDayName => CheckInDateTime.HasValue ? CheckInDateTime.Value.ToString("dddd", new CultureInfo("ar-SA")):"";

        public string CheckInTime => CheckInDateTime.HasValue ? CheckInDateTime.Value.ToString("hh:mm tt", new CultureInfo("ar-SA")):"";

        public DateTime? CheckOutDateTime { get; set; }

        public string CheckOutDateHijri => CheckOutDateTime.HasValue ? CheckOutDateTime.Value.ConvertToString(true) : "";

        public string CheckOutDayName => CheckOutDateTime.HasValue ? CheckOutDateTime.Value.ToString("dddd", new CultureInfo("ar-SA")) : "";

        public string CheckOutTime => CheckOutDateTime.HasValue ? CheckOutDateTime.Value.ToString("hh:mm tt", new CultureInfo("ar-SA")) : "";

        public TimeSpan? WorkDuration { get; set; }

        public bool IsDayOff { get; set; }

        public string DayOffDescription { get; set; }

        public string WorkType => IsDayOff ? "إجازة" : "عمل";

    }
}
