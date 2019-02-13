using Sameer.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sgs.Attendance.Reports.Models
{
    public class WorkCalendar : ISameerObject,IValidatableObject
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public ContractWorkTime ContractWorkTime { get; set; }

        public List<WorkShift> WorkShifts { get; set; }

        public string Note { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var result = new List<ValidationResult>();

            if(EndDate.HasValue && StartDate.Date > EndDate.Value)
            {
                result.Add(new ValidationResult("Start date can't be after end date !", new string[] { nameof(StartDate), nameof(EndDate) }));
            }

            return result;
        }
    }
}
