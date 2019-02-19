using Sameer.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sgs.Attendance.Reports.Models
{
    public class WorkShift : ISameerObject,IValidatableObject
    {
        public int Id { get; set; }

        [Range(1,31,ErrorMessage ="Shift order must be between 1 and 31")]
        [Unique(nameof(WorkCalendarId),ErrorMessage ="Shift order already taken !")]
        public int ShiftOrder { get; set; }

        [Range(1, 31, ErrorMessage = "Shift repeat count must be between 1 and 31")]
        public bool ShiftRepeat { get; set; }

        [Range(0, 23.99, ErrorMessage = "{0} must be between {2} And {1}")]
        public double? ShiftStart { get; set; }

        public TimeSpan? ShiftStartTime => ShiftStart.HasValue ? ShiftStart.Value.ConvertToTime() : default(TimeSpan);

        [Range(0, 23.99, ErrorMessage = "{0} must be between {2} And {1}")]
        public double? ShiftEnd { get; set; }

        public TimeSpan? ShiftEndTime => ShiftEnd.HasValue ? ShiftEnd.Value.ConvertToTime() : default(TimeSpan);

        public double? ShiftDuration
        {
            get
            {
                if (!ShiftStart.HasValue || !ShiftEnd.HasValue)
                {
                    return null;
                }

                if (ShiftStart.Value < ShiftEnd.Value)
                {
                    return ShiftEnd.Value - ShiftStart.Value;
                }
                else if (ShiftStart.Value > ShiftEnd.Value)
                {
                    return ShiftEnd + (24d - ShiftStart);
                }
                else
                {
                    return 24;
                }
            }
        }

        public TimeSpan? ShiftDurationTime => ShiftDuration.HasValue ?
            ShiftDuration.Value.ConvertToTime() : default(TimeSpan?);

        [Range(0, 23.99, ErrorMessage = "{0} must be between {2} And {1}")]
        public double? ShiftStartInRamadan { get; set; }

        public TimeSpan? ShiftStartTimeInRamadan => ShiftStartInRamadan.HasValue ? 
            ShiftStartInRamadan.Value.ConvertToTime() : default(TimeSpan);

        [Range(0, 23.99, ErrorMessage = "{0} must be between {2} And {1}")]
        public double? ShiftEndInRamadan { get; set; }

        public TimeSpan? ShiftEndTimeInRamadan => ShiftEndInRamadan.HasValue ? 
            ShiftEndInRamadan.Value.ConvertToTime() : default(TimeSpan);

        public double? ShiftDurationInRamadan
        {
            get
            {
                double? start = ShiftStartInRamadan ?? ShiftStart;

                double? end = ShiftEndInRamadan ?? ShiftEnd;

                if (!start.HasValue || !end.HasValue)
                {
                    return null;
                }

                if (start.Value < end.Value)
                {
                    return end.Value - start.Value;
                }
                else if (start.Value > end.Value)
                {
                    return end + (24d - start);
                }
                else
                {
                    return 24;
                }
            }
        }

        public TimeSpan? ShiftDurationTimeInRamadan => ShiftDurationInRamadan.HasValue ? 
            ShiftDurationInRamadan.Value.ConvertToTime() : default(TimeSpan?);

        public bool IsDayOff { get; set; }

        public string DayOffDescription { get; set; }

        public bool? IsDayOffInRamadan { get; set; }

        public string DayOffDescriptionInRamadan { get; set; }

        public int WorkCalendarId { get; set; }

        public WorkCalendar WorkCalendar { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (!IsDayOff)
            {
                if (!ShiftStart.HasValue)
                {
                    results.Add(new ValidationResult($"{nameof(ShiftStart)} is required !", new string[] { nameof(ShiftStart), nameof(IsDayOff) }));
                }

                if (!this.ShiftEnd.HasValue)
                {
                    results.Add(new ValidationResult($"{nameof(ShiftEnd)} is required !", new string[] { nameof(ShiftEnd), nameof(IsDayOff) }));
                }

                if (ShiftDuration.HasValue && (ShiftDuration < 0.5d || ShiftDuration > 12d))
                {
                    results.Add(new ValidationResult($"{nameof(ShiftDuration)} must be between 30 minutes And 12 hours !", new string[] { nameof(ShiftDuration) }));
                }
            }
            else if (string.IsNullOrWhiteSpace(DayOffDescription))
            {
                results.Add(new ValidationResult($"{nameof(DayOffDescription)} is required for day off sifts !", new string[] { nameof(DayOffDescription), nameof(IsDayOff) }));
            }
            else
            {
                if (ShiftStart.HasValue || ShiftEnd.HasValue)
                {
                    results.Add(new ValidationResult($"{nameof(ShiftStart)} and {nameof(ShiftEnd)} must be cleared for day off !", new string[] { nameof(ShiftStart), nameof(ShiftEnd), nameof(IsDayOff) }));
                }
            }

            if (IsDayOffInRamadan.HasValue && IsDayOffInRamadan.Value && string.IsNullOrWhiteSpace(DayOffDescriptionInRamadan) && string.IsNullOrWhiteSpace(DayOffDescription))
            {
                results.Add(new ValidationResult($"{nameof(DayOffDescriptionInRamadan)} or {nameof(DayOffDescription)} is required for day off sifts !", new string[] { nameof(DayOffDescriptionInRamadan), nameof(IsDayOffInRamadan), nameof(DayOffDescription) }));
            }

            if (IsDayOffInRamadan.HasValue && !IsDayOffInRamadan.Value)
            {
                if (!this.ShiftStart.HasValue && !this.ShiftStartInRamadan.HasValue)
                {
                    results.Add(new ValidationResult($"{nameof(ShiftStart)} or {nameof(ShiftStartInRamadan)} is required for day off in ramadan !", new string[] { nameof(ShiftStartInRamadan), nameof(IsDayOffInRamadan) }));
                }

                if (!this.ShiftEnd.HasValue && !this.ShiftEndInRamadan.HasValue)
                {
                    results.Add(new ValidationResult($"{nameof(ShiftEnd)} or {nameof(ShiftEndInRamadan)} is required for day off in ramadan !", new string[] { nameof(ShiftEndInRamadan), nameof(IsDayOffInRamadan) }));
                }

                if (this.ShiftDurationInRamadan.HasValue && (this.ShiftDurationInRamadan < 0.5d || ShiftDurationInRamadan > 12d))
                {
                    results.Add(new ValidationResult($"{nameof(ShiftDurationInRamadan)} must be between 30 minutes And 12 hours !", new string[] { nameof(ShiftDurationInRamadan) }));
                }
            }
            else if (IsDayOffInRamadan.HasValue && IsDayOffInRamadan.Value)
            {
                if (ShiftStartInRamadan.HasValue || ShiftEndInRamadan.HasValue)
                {
                    results.Add(new ValidationResult($"{nameof(ShiftStartInRamadan)} and {nameof(ShiftEndInRamadan)} must be cleared for day off !", new string[] { nameof(ShiftStartInRamadan), nameof(ShiftEndInRamadan), nameof(IsDayOffInRamadan) }));
                }
            }

            return results;
        }

    }
}
