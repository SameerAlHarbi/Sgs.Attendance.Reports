using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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

        public async Task<List<CalendarDayReport>> GetCalendarsDaysReport(ContractWorkTime workTime,DateTime? fromDate=null,DateTime? toDate=null)
        {
            try
            {
                fromDate = fromDate ?? new DateTime(2018, 12, 1);
                toDate = toDate ?? DateTime.Today;

                var results = new List<CalendarDayReport>();

                DateTime startDate = fromDate.Value;
                while(startDate <= toDate)
                {
                    bool dayOff = startDate.DayOfWeek == DayOfWeek.Friday || startDate.DayOfWeek == DayOfWeek.Saturday;
                    results.Add(new CalendarDayReport
                    {
                        DayDate = startDate,
                        IsDayOff = dayOff,
                        DayOffDescription = dayOff ? "نهاية الاسبوع" : "",
                        CheckInDateTime = !dayOff ? startDate.Date.Add(new TimeSpan(7,30,0)):default(DateTime?),
                        CheckOutDateTime = !dayOff ? startDate.Date.Add(new TimeSpan(14,30,0)):default(DateTime?)
                    });
                    startDate = startDate.AddDays(1);
                }

                var workTimeCalenars = await GetAllAsNoTrackingListAsync(c => c.ContractWorkTime == workTime
                    && (!c.EndDate.HasValue && c.StartDate <= toDate.Value) || (c.EndDate.HasValue && c.EndDate >= fromDate.Value && c.StartDate <= toDate.Value),c => c.WorkShifts);

                foreach (var calendar in workTimeCalenars.Where(c => !c.EndDate.HasValue).OrderBy(c => c.StartDate))
                {
                    var calendarDaysReports = calendar.GetDaysReports(fromDate, toDate);
                    results.RemoveAll(d => calendarDaysReports.Any(cd => cd.DayDate.Date == d.DayDate.Date));
                    results.AddRange(calendarDaysReports);
                }

                foreach (var calendar in workTimeCalenars.Where(c => c.EndDate.HasValue).OrderBy(c => c.StartDate))
                {
                    var calendarDaysReports = calendar.GetDaysReports(fromDate, toDate);
                    results.RemoveAll(d => calendarDaysReports.Any(cd => cd.DayDate.Date == d.DayDate.Date));
                    results.AddRange(calendarDaysReports);
                }

                return results.OrderBy(d => d.DayDate).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
