using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Sameer.Shared.Data;
using Sgs.Attendance.Reports.Models;

namespace Sgs.Attendance.Reports.Logic
{
    public class EmployeesCalendarsManager : GeneralManager<EmployeeCalendar>
    {
        public EmployeesCalendarsManager(IRepository repo) : base(repo)
        {
        }

        protected override ICollection<ValidationResult> ValidateItemInformation(EmployeeCalendar newItem)
        {
            var results = base.ValidateItemInformation(newItem);
            
            if(newItem.EndDate.HasValue && newItem.EndDate < newItem.StartDate)
            {
                results.Add(new ValidationResult("End date can't be before start date !",new string[] { "StartDate","EndDate"}));
            }

            return results;
        }

        protected override async Task<ICollection<ValidationResult>> ValidateUniqueItemAsync(EmployeeCalendar newItem)
        {
            var results = await base.ValidateUniqueItemAsync(newItem);

            var allEmployeeCalendars = await GetAllAsNoTrackingListAsync(c => c.EmployeeId == newItem.EmployeeId);

            if(allEmployeeCalendars.Count > 0)
            {
                if (allEmployeeCalendars.Any(c => c.StartDate == newItem.StartDate 
                    && c.Id != newItem.Id && c.EndDate.HasValue == newItem.EndDate.HasValue ))
                {
                    results.Add(new ValidationResult("Calendar start date duplicate",new string[] { "StartDate" }));
                }
                else if (allEmployeeCalendars.Any(c => newItem.EndDate.HasValue 
                            && c.EndDate.HasValue
                            && ((c.StartDate >= newItem.StartDate && c.StartDate <= newItem.EndDate)
                                || ( newItem.StartDate >= c.StartDate && newItem.StartDate <= c.EndDate))))
                {
                    results.Add(new ValidationResult("Calendar conflict", new string[] { "StartDate", "EndDate" }));
                }
            }

            return results;
        }
    }
}
