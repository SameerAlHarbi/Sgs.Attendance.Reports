using Microsoft.Extensions.Logging;
using Sgs.Attendance.Reports.Logic;
using System;
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

        public async Task<bool> DoWork()
        {
            try
            {
                var openProcessingRequests = await _processingsRequestsManager.GetSingleItemAsync(pr => !pr.Completed);
                if(openProcessingRequests != null)
                {
                    openProcessingRequests.Completed = true;
                    await _processingsRequestsManager.UpdateItemAsync(openProcessingRequests);
                }
            }
            catch (Exception)
            {
            }

            return true;
        }
    }
}
