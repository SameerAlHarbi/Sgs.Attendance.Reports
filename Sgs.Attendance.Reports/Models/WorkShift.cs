using Sameer.Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace Sgs.Attendance.Reports.Models
{
    public class WorkShift : ISameerObject
    {

        public int Id { get; set; }

        [Range(1,31,ErrorMessage ="Shift order must be between 1 and 31")]
        [Unique(nameof(WorkCalendarId),ErrorMessage ="Shift order already taken !")]
        public int ShiftOrder { get; set; }

        [Range(1, 31, ErrorMessage = "Shift repeat count must be between 1 and 31")]
        public bool ShiftRepeat { get; set; }

        public TimeSpan? ShiftStartTime { get; set; }

        public TimeSpan? ShiftEndTime { get; set; }

        public TimeSpan? ShiftStartTimeInRamadan { get; set; }

        public TimeSpan? ShiftEndTimeInRamadan { get; set; }

        public bool IsDayOff { get; set; }

        public bool? IsDayOffInRamadan { get; set; }

        public int WorkCalendarId { get; set; }

        public WorkCalendar WorkCalendar { get; set; }

    }
}
