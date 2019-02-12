using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sameer.Shared;
using Sgs.Attendance.Reports.Logic;
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
        private readonly ILogger _logger;

        public ScopedProcessingService(
              ProcessingsRequestsManager processingsRequestsManager
            , EmployeesDaysReportsManager employeesDaysReportsManager
            , IErpManager erpManager
            , ILogger<ScopedProcessingService> logger)
        {
            _processingsRequestsManager = processingsRequestsManager;
            _employeesDaysReportsManager = employeesDaysReportsManager;
            _erpManager = erpManager;
            _logger = logger;
        }

        private async Task<bool> processDaysReports(IEnumerable<int> employeesIds ,DateTime fromDate,DateTime toDate)
        {
            try
            {
                
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
                    DateTime toDate = lastDayReport?.DayDate.AddDays(5) ?? openProcessingRequest.FromDate.AddDays(5);
                    toDate = toDate <= openProcessingRequest.ToDate ? toDate : openProcessingRequest.ToDate;

                    var employees = await _erpManager.GetShortEmployeesInfo(employeesIds);
                    var lastRequestEmployeeId = employees.Max(e => e.EmployeeId);

                    if (fromDate > openProcessingRequest.ToDate && lastEmployeeId)
                    {
                        openProcessingRequest.Completed = true;
                        await _processingsRequestsManager.UpdateItemAsync(openProcessingRequest);
                        return true;
                    }


                    if (lastDayReport == null)
                    {
                        employeesIds = employees.OrderBy(e => e.EmployeeId).Select(e => e.EmployeeId).Distinct().Take(10).ToList();
                        return await processDaysReports(employeesIds, openProcessingRequest.FromDate, openProcessingRequest openProcessingRequest.ToDate);
                    }
                    else
                    {
                        employeesIds = employees.Where().OrderBy(e => e.EmployeeId).Select(e => e.EmployeeId).Distinct().Take(10).ToList();
                    }  
                }
            }
            catch (Exception)
            {
            }

            return true;
        }
    }
}
