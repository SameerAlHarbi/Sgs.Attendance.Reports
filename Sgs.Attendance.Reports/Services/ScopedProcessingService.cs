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
using System.Transactions;

namespace Sgs.Attendance.Reports.Services
{
    public class ScopedProcessingService : IScopedProcessingService
    {
        private readonly ProcessingsRequestsManager _processingsRequestsManager;
        private readonly EmployeesDaysReportsManager _employeesDaysReportsManager;
        private readonly AbsentsNotificationsManager _absentsNotificationsManager;
        private readonly IErpManager _erpManager;
        private readonly EmployeesCalendarsManager _employeesCalendarManager;
        private readonly EmployeesExcusesManager _employeesExcusesManager;
        private readonly WorkCalendarsManager _workCalendarsManager;
        private readonly ILogger _logger;

        public ScopedProcessingService(
              ProcessingsRequestsManager processingsRequestsManager
            , EmployeesDaysReportsManager employeesDaysReportsManager
            , AbsentsNotificationsManager absentsNotificationsManager
            , IErpManager erpManager
            , EmployeesCalendarsManager employeesCalendarManager
            , EmployeesExcusesManager employeesExcusesManager
            , WorkCalendarsManager workCalendarsManager
            , ILogger<ScopedProcessingService> logger)
        {
            _processingsRequestsManager = processingsRequestsManager;
            _employeesDaysReportsManager = employeesDaysReportsManager;
            _absentsNotificationsManager = absentsNotificationsManager;
            _erpManager = erpManager;
            _employeesCalendarManager = employeesCalendarManager;
            _employeesExcusesManager = employeesExcusesManager;
            _workCalendarsManager = workCalendarsManager;
            _logger = logger;
        }

        private async Task<bool> processDaysReports(IEnumerable<EmployeeInfoViewModel> employees ,DateTime fromDate,DateTime toDate)
        {
            try
            {
                var employeesIds = employees.Select(e => e.EmployeeId).OrderBy(e => e).ToList();

                //Get employees Calendars
                var allEmployeesCalendars = await _employeesCalendarManager.GetAllAsNoTrackingListAsync(ec => employeesIds.Contains(ec.EmployeeId)
                    && ((ec.StartDate <= fromDate && ec.EndDate == null) || (ec.StartDate >= fromDate && ec.StartDate <= toDate) || (ec.StartDate <= fromDate && ec.EndDate >= fromDate)));

                //Get workCalendars
                var calendarsDaysReports = new Dictionary<ContractWorkTime, List<CalendarDayReport>>();
                foreach (var workTime in (ContractWorkTime[])Enum.GetValues(typeof(ContractWorkTime)))
                {
                    calendarsDaysReports.Add(workTime, await _workCalendarsManager.GetCalendarsDaysReport(workTime, fromDate, toDate));
                }

                //Get employees vacations
                var allEmployeesVacations = await _erpManager.GetAllVacations(fromDate, toDate, employeesIds);

                //Get employees Open vacations requests
                var allEmployeesOpenVacationsRequests = await _erpManager.GetAllOpenVacations(fromDate, toDate, employeesIds);

                //Get employees open delegations requests
                var allEmployeesOpenDelegationsRequests = await _erpManager.GetAllOpenDelegations(fromDate, toDate, employeesIds);

                //Get employees transactions
                var allEmployeesTransactions = await _erpManager.GetAllTransaction(fromDate, toDate, employeesIds);

                //Get employees excuses
                var allEmployeesExcuses = await _employeesExcusesManager.GetAllAsNoTrackingListAsync(ex => employeesIds.Contains(ex.EmployeeId)
                    && ex.ExcueseDate >= fromDate && ex.ExcueseDate <= toDate);

                var resultsDaysReports = new List<EmployeeDayReport>();

                DateTime startDate = fromDate;

                while (startDate <= toDate)
                {
                    var employeesCalendars = allEmployeesCalendars.Where(c => (c.StartDate <= startDate && c.EndDate == null)
                    || (startDate >= c.StartDate && startDate <= c.EndDate)).ToList();
                    var excuses = allEmployeesExcuses.Where(x => x.ExcueseDate.Date == startDate);
                    var vacations = allEmployeesVacations.Where(v => startDate >= v.StartDate && startDate <= v.EndDate).ToList();
                    var openDelegationsRequests = allEmployeesOpenDelegationsRequests.Where(d => startDate >= d.StartDate && startDate <= d.EndDate).ToList();
                    var openVacationsRequests = allEmployeesOpenVacationsRequests.Where(d => startDate >= d.StartDate && startDate <= d.EndDate).ToList();
                    var transactions = allEmployeesTransactions.Where(t => t.TransactionDate.Date >= startDate && t.TransactionDate.Date <= startDate.AddDays(1).Date).ToList();

                    foreach (var employee in employees)
                    {
                        var newDayReport = new EmployeeDayReport
                        {
                            EmployeeId = employee.EmployeeId,
                            EmployeeName = employee.Name,
                            DepartmentId = employee.DepartmentId,
                            DepartmentName = employee.DepartmentName,
                            DayDate = startDate.Date,
                            ProcessingDate = DateTime.Now
                        };

                        var defaultEmployeeCalendar = employeesCalendars.Where(c => c.EndDate == null 
                            && c.EmployeeId == employee.EmployeeId)
                            .OrderByDescending(c => c.StartDate).FirstOrDefault() ?? new EmployeeCalendar
                            {
                                StartDate = startDate,
                                AttendanceProof = AttendanceProof.RequiredInOut,
                                EmployeeId = employee.EmployeeId,
                                ContractWorkTime = ContractWorkTime.Default
                            };

                        var limitedCalendar = employeesCalendars.Where(c => c.EndDate.HasValue && c.EmployeeId == employee.EmployeeId).FirstOrDefault();

                        var appliedCalendar = limitedCalendar ?? defaultEmployeeCalendar;

                        newDayReport.AttendanceProof = appliedCalendar.AttendanceProof;

                        var contractDayReport = calendarsDaysReports[appliedCalendar.ContractWorkTime]?
                            .FirstOrDefault(d => d.DayDate.Date == startDate.Date);

                        if (!contractDayReport.IsDayOff)
                        {
                            newDayReport.ContractCheckInDateTime = contractDayReport.CheckInDateTime;
                            newDayReport.ContractCheckOutDateTime = contractDayReport.CheckOutDateTime;
                            newDayReport.ContractWorkDurationTime = contractDayReport.WorkDuration;
                        }
                        else
                        {
                            newDayReport.IsVacation = true;
                            newDayReport.VacationName = contractDayReport.DayOffDescription;
                        }

                        var employeeVacation = vacations.FirstOrDefault(v => v.EmployeeId == employee.EmployeeId);
                        if (employeeVacation != null)
                        {
                            newDayReport.IsVacation = true;
                            newDayReport.VacationName = employeeVacation.VacationTypeName;
                            newDayReport.VacationRegisterDate = employeeVacation.RegisterDate;
                        }

                        var employeeOpenVacationRequest = openVacationsRequests
                            .FirstOrDefault(vr => vr.EmployeeId == employee.EmployeeId);
                        if (employeeOpenVacationRequest != null)
                        {
                            newDayReport.IsVacationRequest = true;
                            newDayReport.VacationRequestDate = employeeOpenVacationRequest.RequestDate;
                        }

                        var employeeOpenDelegationRequest = openDelegationsRequests
                            .FirstOrDefault(vr => vr.EmployeeId == employee.EmployeeId);
                        if (employeeOpenDelegationRequest != null)
                        {
                            newDayReport.IsDelegationRequest = true;
                            newDayReport.DelegationRequestDate = employeeOpenDelegationRequest.RequestDate;
                        }

                        DateTime? checkOutStartRange = null;

                        if (newDayReport.ContractCheckInDateTime.HasValue
                                && newDayReport.ContractCheckOutDateTime.HasValue)
                        {
                            var checkInStartRange = newDayReport.ContractCheckInDateTime.Value.Add(new TimeSpan(-2, 0, 0));

                             checkOutStartRange = newDayReport.ContractCheckInDateTime.Value.Add(new TimeSpan(0, 31, 0));
                            var checkOutEndRange = newDayReport.ContractCheckOutDateTime.Value.Add(new TimeSpan(5, 0, 0));

                            var employeeTransactions = transactions.Where(t => t.EmployeeId == employee.EmployeeId).OrderBy(o => o.TransactionDate).ToList();
                            if (employeeTransactions.Count > 0)
                            {
                                newDayReport.ActualCheckInDateTime = employeeTransactions
                                    .FirstOrDefault(t => t.TransactionDate >= checkInStartRange && t.TransactionDate <= newDayReport.ContractCheckOutDateTime.Value)?.TransactionDate;

                                newDayReport.ActualCheckOutDateTime = employeeTransactions.LastOrDefault(t => t.TransactionDate >= checkOutStartRange.Value && t.TransactionDate <= checkOutEndRange)?.TransactionDate;

                                if (newDayReport.ActualCheckInDateTime.HasValue
                                    && newDayReport.ActualCheckOutDateTime.HasValue)
                                {
                                    newDayReport.ActualCheckOutDateTime = newDayReport.ActualCheckOutDateTime >= newDayReport.ActualCheckInDateTime.Value.AddMinutes(1) ?
                                           newDayReport.ActualCheckOutDateTime : default(DateTime?);

                                    var checkIn = newDayReport.ActualCheckInDateTime >= newDayReport.ContractCheckInDateTime ?
                                        newDayReport.ActualCheckInDateTime : newDayReport.ContractCheckInDateTime;

                                    var checkOut = newDayReport.ActualCheckOutDateTime.HasValue 
                                        && newDayReport.ActualCheckOutDateTime > newDayReport.ContractCheckOutDateTime ?
                                        newDayReport.ContractCheckOutDateTime : newDayReport.ActualCheckOutDateTime;

                                    newDayReport.ActualWorkDurationTime = checkOut.HasValue ?
                                        checkOut.Value.Subtract(checkIn.Value) : default(TimeSpan?);

                                    newDayReport.ActualTotalDurationTime = newDayReport.ActualCheckOutDateTime.HasValue ?
                                        newDayReport.ActualCheckOutDateTime.Value
                                        .Subtract(newDayReport.ActualCheckInDateTime.Value) : default(TimeSpan?);
                                }
                            }
                        }

                        var employeeExcuses = excuses.Where(x => x.EmployeeId == employee.EmployeeId).ToList();
                        newDayReport.CheckInExcuse = employeeExcuses.Any(x => x.ExcuseType == ExcuseType.CheckIn);
                        newDayReport.CheckOutExcuse = employeeExcuses.Any(x => x.ExcuseType == ExcuseType.CheckOut);

                        newDayReport.CheckInExcuseHours = employeeExcuses.FirstOrDefault(x =>
                            x.ExcuseType == ExcuseType.CheckIn)?.ExcuseHours ?? 0;

                        newDayReport.CheckOutExcuseHours = employeeExcuses.FirstOrDefault(x =>
                            x.ExcuseType == ExcuseType.CheckOut)?.ExcuseHours ?? 0;

                        if (!newDayReport.IsVacation)
                        {
                            newDayReport.CheckInDateTime = newDayReport.ActualCheckInDateTime;
                            newDayReport.CheckOutDateTime = newDayReport.ActualCheckOutDateTime;

                            if (newDayReport.CheckInDateTime.HasValue
                                && (newDayReport.CheckInDateTime.HasValue && checkOutStartRange.HasValue && newDayReport.CheckInDateTime.Value > checkOutStartRange.Value )
                                && !newDayReport.CheckOutDateTime.HasValue
                                && newDayReport.CheckInExcuse)
                            {
                                newDayReport.CheckOutDateTime = newDayReport.CheckInDateTime;
                            }

                            if (newDayReport.CheckInDateTime.HasValue 
                                && newDayReport.CheckInDateTime > newDayReport.ContractCheckInDateTime)
                            {
                                if (newDayReport.CheckInDateTime <= 
                                    newDayReport.ContractCheckInDateTime.Value.Add(new TimeSpan(0, 31, 0))
                                    || newDayReport.IsOpenCheckInExcuse)
                                {
                                    newDayReport.CheckInDateTime = newDayReport.ContractCheckInDateTime;
                                }
                                else if (newDayReport.CheckInExcuse)
                                {
                                    newDayReport.CheckInDateTime = newDayReport.CheckInDateTime.Value
                                        .Subtract(newDayReport.CheckInExcuseHours.ConvertToTime());

                                    newDayReport.CheckInDateTime = newDayReport.CheckInDateTime < newDayReport.ContractCheckInDateTime ?
                                        newDayReport.ContractCheckInDateTime : newDayReport.CheckInDateTime;
                                    
                                }
                            }
                            else if(!newDayReport.CheckInDateTime.HasValue 
                                && newDayReport.IsOpenCheckInExcuse)
                            {
                                newDayReport.CheckInDateTime = newDayReport.ContractCheckInDateTime;
                            }

                            if (newDayReport.CheckOutDateTime.HasValue
                                && newDayReport.CheckOutDateTime.Value < newDayReport.ContractCheckOutDateTime)
                            {
                                newDayReport.CheckOutDateTime = newDayReport.IsOpenCheckOutExcuse ?
                                    newDayReport.ContractCheckOutDateTime : newDayReport.CheckOutDateTime;

                                if (!newDayReport.IsOpenCheckOutExcuse)
                                {
                                    newDayReport.CheckOutDateTime = newDayReport.CheckOutDateTime.Value
                                        .Add(newDayReport.CheckOutExcuseHours.ConvertToTime());

                                    newDayReport.CheckOutDateTime = newDayReport.CheckOutDateTime > newDayReport.ContractCheckOutDateTime ?
                                        newDayReport.ContractCheckOutDateTime : newDayReport.CheckOutDateTime;
                                }
                            }
                            else if (!newDayReport.CheckOutDateTime.HasValue
                                && newDayReport.IsOpenCheckOutExcuse)
                            {
                                newDayReport.CheckOutDateTime = newDayReport.ContractCheckOutDateTime;
                            }

                            //Calculations

                            newDayReport.IsAbsentEmployee = !newDayReport.IsVacation
                                && !newDayReport.CheckInDateTime.HasValue
                                && !newDayReport.CheckOutDateTime.HasValue
                                && newDayReport.AttendanceProof != AttendanceProof.Exempted;

                            if (newDayReport.CheckInDateTime.HasValue 
                                && newDayReport.CheckOutDateTime.HasValue && newDayReport.AttendanceProof == AttendanceProof.RequiredInOut)
                            {
                                var checkIn = newDayReport.CheckInDateTime >= newDayReport.ContractCheckInDateTime ?
                                        newDayReport.CheckInDateTime : newDayReport.ContractCheckInDateTime;

                                var checkOut = newDayReport.CheckOutDateTime > newDayReport.ContractCheckOutDateTime ?
                                    newDayReport.ContractCheckOutDateTime : newDayReport.CheckOutDateTime;

                                newDayReport.WorkDurationTime = checkOut.Value.Subtract(checkIn.Value) ;

                                newDayReport.WorkTotalDurationTime = newDayReport.CheckOutDateTime.Value
                                    .Subtract(newDayReport.CheckInDateTime.Value);
                            }

                            if (newDayReport.AttendanceProof == AttendanceProof.RequiredInOut)
                            {
                                newDayReport.CheckInLateDurationTime = newDayReport.CheckInDateTime.HasValue 
                                    && newDayReport.CheckInDateTime.Value > newDayReport.ContractCheckInDateTime ? 
                                     newDayReport.CheckInDateTime.Value.Subtract(newDayReport.ContractCheckInDateTime.Value) : default(TimeSpan?);

                                newDayReport.CheckOutEarlyDurationTime = newDayReport.CheckOutDateTime.HasValue 
                                    && newDayReport.CheckOutDateTime.Value < newDayReport.ContractCheckOutDateTime ? 
                                     newDayReport.ContractCheckOutDateTime.Value.Subtract(newDayReport.CheckOutDateTime.Value) : default(TimeSpan?);

                                if (!newDayReport.IsAbsentEmployee)
                                {
                                    newDayReport.WasteDurationTime = !newDayReport.WorkDurationTime.HasValue ?
                                        newDayReport.ContractWorkDurationTime : newDayReport.ContractWorkDurationTime.Value
                                        .Subtract(newDayReport.WorkDurationTime.Value);
                                }
                            }
                        }

                        resultsDaysReports.Add(newDayReport);
                    }

                    startDate = startDate.AddDays(1);

                }

                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        await _employeesDaysReportsManager
                                        .DirectDeleteItems(d => (employeesIds.Count < 1 || employeesIds.Contains(d.EmployeeId))
                                            && d.DayDate >= fromDate && d.DayDate <= toDate);

                        try
                        {
                            var notificationsLogs = await _absentsNotificationsManager.GetAllAsNoTrackingListAsync(d => (employeesIds.Count < 1 || employeesIds.Contains(d.EmployeeId))
                                            && d.AbsentDate >= fromDate && d.AbsentDate <= toDate);

                            if(notificationsLogs.Count > 0)
                            {
                                foreach (var rdr in resultsDaysReports)
                                {
                                    var notificationLog = notificationsLogs.FirstOrDefault(n => n.EmployeeId == rdr.EmployeeId 
                                        && n.AbsentDate == rdr.DayDate);

                                    rdr.AbsentNotified = notificationLog != null;
                                    rdr.AbsentNotifiedByEmployeeId = notificationLog?.ByEmployeeId;
                                    rdr.AbsentNotifiedByEmployeeName = notificationLog?.ByEmployeeName;
                                    rdr.AbsentNotifiedDate = notificationLog?.SendDate;
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }

                        //foreach (var item in resultsDaysReports)
                        //{
                        //    try
                        //    {
                        //        await _employeesDaysReportsManager.InsertNewDataItem(item);
                        //    }
                        //    catch (Exception ex)
                        //    {

                        //        throw;
                        //    }
                        //}

                        await _employeesDaysReportsManager.InsertNewDataItems(resultsDaysReports);

                        scope.Complete();
                    }
                    catch (Exception ex)
                    {
                        scope.Dispose();
                        throw;
                    }
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
                //Check if there is any open proceesing request
                var openProcessingRequest = await _processingsRequestsManager.GetSingleItemAsync(pr => !pr.Completed);
                if(openProcessingRequest != null)
                {
                    //Get requset employees ids
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

                    var employees = await _erpManager.GetEmployeesInfo(employeesIds);
                    var lastRequestEmployeeId = employees.Max(e => e.EmployeeId);

                    if (fromDate > openProcessingRequest.ToDate)
                    {
                        if (lastEmployeeId >= lastRequestEmployeeId)
                        {
                            openProcessingRequest.Completed = true;
                            openProcessingRequest.CompletedDate = DateTime.Now;
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
                        .Where(e => e.EmployeeId <= lastEmployeeId).OrderByDescending(e => e.EmployeeId)
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
            catch (Exception ex)
            {
            }

            return true;
        }

    }
}
