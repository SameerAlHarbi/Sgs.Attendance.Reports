using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sameer.Shared;
using Sgs.Attendance.Reports.Logic;
using Sgs.Attendance.Reports.Models;
using Sgs.Attendance.Reports.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sgs.Attendance.Reports.Services
{
    public class ScopedProcessingService : IScopedProcessingService
    {
        private readonly ProcessingsRequestsManager _processingsRequestsManager;
        private readonly EmployeesDaysReportsManager _employeesDaysReportsManager;
        private readonly IErpManager _erpManager;
        private readonly EmployeesCalendarsManager _employeesCalendarManager;
        private readonly EmployeesExcusesManager _employeesExcusesManager;
        private readonly ILogger _logger;

        public ScopedProcessingService(
              ProcessingsRequestsManager processingsRequestsManager
            , EmployeesDaysReportsManager employeesDaysReportsManager
            , IErpManager erpManager
            , EmployeesCalendarsManager employeesCalendarManager
            , EmployeesExcusesManager employeesExcusesManager
            , ILogger<ScopedProcessingService> logger)
        {
            _processingsRequestsManager = processingsRequestsManager;
            _employeesDaysReportsManager = employeesDaysReportsManager;
            _erpManager = erpManager;
            _employeesCalendarManager = employeesCalendarManager;
            _employeesExcusesManager = employeesExcusesManager;
            _logger = logger;
        }

        private async Task<bool> processDaysReports(IEnumerable<ShortEmployeeInfoViewModel> employees ,DateTime fromDate,DateTime toDate)
        {
            try
            {
                var employeesIds = employees.Select(e => e.EmployeeId).OrderBy(e => e).ToList();

                //Get employees Calendars
                var allEmployeesCalendars = await _employeesCalendarManager.GetAllAsNoTrackingListAsync(ec => employeesIds.Contains(ec.EmployeeId)
                    && (ec.StartDate <= fromDate && ec.EndDate == null) || (ec.StartDate >= fromDate && ec.StartDate <= toDate) || (ec.StartDate <= fromDate && ec.EndDate >= fromDate));

                //Get employees excuses
                var allEmployeesExcuses = await _employeesExcusesManager.GetAllAsNoTrackingListAsync(ex => employeesIds.Contains(ex.EmployeeId)
                    && ex.ExcueseDate >= fromDate && ex.ExcueseDate <= toDate);

                //Get employees vacations
                var allEmployeesVacations = await _erpManager.GetAllVacations(fromDate, toDate,employeesIds);

                //Get employees open delegations requests
                var allEmployeesOpenDelegationsRequests = await _erpManager.GetAllOpenDelegations(fromDate, toDate, employeesIds);

                //Get employees Open vacations requests
                var allEmployeesOpenVacationsRequests = await _erpManager.GetAllOpenVacations(fromDate, toDate, employeesIds);

                //Get employees transactions
                var allEmployeesTransactions = await _erpManager.GetAllTransaction(fromDate, toDate, employeesIds);

                var resultsDaysReports = new List<EmployeeDayReport>();

                while (fromDate <= toDate)
                {
                    var calendars = allEmployeesCalendars.Where(c => (c.StartDate <= fromDate && c.EndDate == null) 
                    && (fromDate >= c.StartDate && fromDate <= c.EndDate)).ToList();
                    var excuses = allEmployeesExcuses.Where(x => x.ExcueseDate.Date == fromDate);
                    var vacations = allEmployeesVacations.Where(v => fromDate >= v.StartDate && fromDate <= v.EndDate).ToList();
                    var openDelegationsRequest = allEmployeesOpenDelegationsRequests.Where(d => fromDate >= d.StartDate && fromDate <= d.EndDate).ToList();
                    var openVacationsRequest = allEmployeesOpenVacationsRequests.Where(d => fromDate >= d.StartDate && fromDate <= d.EndDate).ToList();
                    var transactions = allEmployeesTransactions.Where(t => t.TransactionDate.Date == fromDate).ToList();

                    foreach (var employee in employees)
                    {
                        var newDayReport = new EmployeeDayReport
                        {
                            EmployeeId = employee.EmployeeId,
                            DayDate = fromDate,
                            ProcessingDate = DateTime.Now
                        };

                        var defaultCalendar = calendars.Where(c => c.EndDate == null)
                            .OrderByDescending(c => c.StartDate).FirstOrDefault() ?? new EmployeeCalendar
                            {
                                StartDate = fromDate,
                                AttendanceProof = AttendanceProof.RequiredInOut,
                                EmployeeId = employee.EmployeeId,
                                ContractWorkTime = ContractWorkTime.ShiftA
                            };

                        var limitedCalendar = calendars.Where(c => c.EndDate.HasValue).FirstOrDefault();

                        var appliedCalendar = limitedCalendar ?? defaultCalendar;

                        newDayReport.AttendanceProof = limitedCalendar.AttendanceProof;
                        
                        
                        

                    }

                    fromDate = fromDate.AddDays(1);
                }


                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return false;
            }
        }

        public async Task<bool> DoWork()
        {
            try
            {
                var openProcessingRequest = await _processingsRequestsManager.GetSingleItemAsync(pr => !pr.Completed);
                if(openProcessingRequest != null)
                {
                    IEnumerable<decimal> listOfEmployeesIdsNumbers = openProcessingRequest.Employees.TryParseToNumbers();
                    List<int> employeesIds = listOfEmployeesIdsNumbers?.Select(d => (int)d).ToList() ?? new List<int>();

                    //Get last processed day report for required employees
                    var lastDayReport = await  _employeesDaysReportsManager
                        .GetAll(d => (employeesIds.Count < 1 || employeesIds.Contains(d.EmployeeId))
                            && d.ProcessingDate > openProcessingRequest.RequestDate)
                            .OrderByDescending(d => d.EmployeeId).ThenByDescending(d => d.DayDate).FirstOrDefaultAsync();

                    int lastEmployeeId = lastDayReport?.EmployeeId ?? 0;
                    DateTime fromDate = lastDayReport?.DayDate.AddDays(1) ?? openProcessingRequest.FromDate;
                    DateTime toDate = lastDayReport?.DayDate.AddDays(10) ?? openProcessingRequest.FromDate.AddDays(10);
                    toDate = toDate <= openProcessingRequest.ToDate ? toDate : openProcessingRequest.ToDate;

                    var employees = await _erpManager.GetShortEmployeesInfo(employeesIds);
                    var lastRequestEmployeeId = employees.Max(e => e.EmployeeId);

                    if (fromDate > openProcessingRequest.ToDate)
                    {
                        if (lastEmployeeId >= lastRequestEmployeeId)
                        {
                            openProcessingRequest.Completed = true;
                            await _processingsRequestsManager.UpdateItemAsync(openProcessingRequest);
                            return true; 
                        }

                        fromDate = openProcessingRequest.FromDate;
                        toDate = openProcessingRequest.FromDate.AddDays(10);
                        toDate = toDate <= openProcessingRequest.ToDate ? toDate : openProcessingRequest.ToDate;

                        employees = employees
                        .Where(e => e.EmployeeId > lastEmployeeId).OrderBy(e => e.EmployeeId)
                        .Take(100).ToList();
                    }
                    else if(lastEmployeeId > 0)
                    {
                        employees = employees
                        .Where(e => e.EmployeeId <= lastEmployeeId).OrderBy(e => e.EmployeeId)
                        .Take(100).ToList();
                    }
                    else
                    {
                        employees = employees
                        .Where(e => e.EmployeeId > lastEmployeeId).OrderBy(e => e.EmployeeId)
                        .Take(100).ToList();
                    }
               
                    return await processDaysReports(employees, fromDate, toDate);
                }
            }
            catch (Exception)
            {
            }

            return true;
        }

    }
}
