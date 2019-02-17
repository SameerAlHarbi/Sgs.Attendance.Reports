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
        [Unique(ErrorMessage = "Calendar name is already exist !")]
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
    }
}
