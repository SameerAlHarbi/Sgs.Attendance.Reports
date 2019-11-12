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

        protected override async Task<ICollection<ValidationResult>> ValidateUniqueItemAsync(EmployeeCalendar newItem)
        {
            var results = await base.ValidateUniqueItemAsync(newItem);

            var allEmployeeCalendars = await GetAllAsNoTrackingListAsync(c => c.EmployeeId == newItem.EmployeeId);

            if(allEmployeeCalendars.Count > 0)
            {
                if (allEmployeeCalendars.Any(c => !newItem.EndDate.HasValue && c.StartDate == newItem.StartDate && c.Id != newItem.Id))
                {
                    results.Add(new ValidationResult("Calendar start date duplicate",new string[] { "StartDate" }));
                }
                else if (allEmployeeCalendars.Any(c => newItem.EndDate.HasValue 
                    && (c.EndDate.HasValue && (c.StartDate >= newItem.StartDate && c.StartDate <= newItem.EndDate)
                    || ( newItem.StartDate >= c.StartDate && newItem.StartDate <= c.EndDate))))
                {

                }
            }

            return results;
        }
    }
}
