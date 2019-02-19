using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Sameer.Shared.Data;
using Sgs.Attendance.Reports.Models;

namespace Sgs.Attendance.Reports.Logic
{
    public class WorkCalendarsManager : GeneralManager<WorkCalendar>
    {
        public WorkCalendarsManager(IRepository repo)
            : base(repo)
        {
        }

        protected override async Task<ICollection<ValidationResult>> CustomNewRulesValidateAsync(WorkCalendar newItem)
        {
            var results = await base.CustomNewRulesValidateAsync(newItem);

            if(newItem.EndDate.HasValue)
            {
                var allConflicts = await this.GetAllAsNoTrackingListAsync(c => c.StartDate <= newItem.EndDate && c.EndDate >= newItem.StartDate);
                if(allConflicts.Count > 0)
                {
                    results.Add(new ValidationResult("عفواً التقويم متعارض مع تقويم آخر",new string[] { nameof(WorkCalendar.StartDate),nameof(WorkCalendar.EndDate)}));
                }
            }

            return results;
        }
    }
}
