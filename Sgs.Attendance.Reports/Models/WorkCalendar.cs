using Sameer.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sgs.Attendance.Reports.Models
{
    public class WorkCalendar : ISameerObject,IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Calendar name is required !")]
        [Unique(nameof(ContractWorkTime),ErrorMessage = "Calendar name is already exist !")]
        [StringLength(100, ErrorMessage = "Calendar name can't be more than {1} characters !")]
        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public ContractWorkTime ContractWorkTime { get; set; }

        public List<WorkShift> WorkShifts { get; set; }

        public bool IsVacationCalendar { get; set; }

        public string Note { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if(EndDate.HasValue && StartDate.Date > EndDate.Value)
            {
                results.Add(new ValidationResult("Start date can't be after end date !", new string[] { nameof(StartDate), nameof(EndDate) }));
            }

            if (IsVacationCalendar && !EndDate.HasValue)
            {
                results.Add(new ValidationResult($"{nameof(EndDate)} is required for vacation caledar !", new string[] { nameof(EndDate), nameof(IsVacationCalendar) }));
            }

            return results;
        }

        public List<CalendarDayReport> DaysReports(DateTime? fromDate = null,DateTime? toDate = null)
        {
            var results = new List<CalendarDayReport>();

            fromDate = (fromDate ?? this.StartDate).Date;
            toDate = (toDate ?? StartDate.AddDays(31)).Date;

            fromDate = fromDate < this.StartDate.Date ? this.StartDate.Date : fromDate;
            toDate = this.EndDate.HasValue
                && toDate > this.EndDate.Value ? this.EndDate.Value : toDate;

            if(fromDate > toDate)
            {
                return results;
            }

            int shiftsCount = this.WorkShifts?.Count ?? 0;
            int shiftOrder = (int)fromDate.Value.Date.Subtract(StartDate.Date).TotalDays;

            while (fromDate <= toDate)
            {
                while (shiftOrder > shiftsCount - 1)
                {
                    shiftOrder = shiftOrder - shiftsCount;
                }

                var newDayReport = new CalendarDayReport
                {
                    DayDate = fromDate.Value,
                    IsDayOff = this.IsVacationCalendar,
                    DayOffDescription = this.IsVacationCalendar ? this.Name : string.Empty
                };

                if(!this.IsVacationCalendar && this.WorkShifts.Count > 1)
                {
                       
                }

                results.Add(newDayReport);
                fromDate = fromDate.Value.AddDays(1);
            }

            return results;
        }
    }
}
