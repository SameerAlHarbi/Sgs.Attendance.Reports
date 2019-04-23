using AutoMapper;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sameer.Shared;
using Sgs.Attendance.Reports.Helpers;
using Sgs.Attendance.Reports.Logic;
using Sgs.Attendance.Reports.Models;
using Sgs.Attendance.Reports.Services;
using Sgs.Attendance.Reports.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sgs.Attendance.Reports.Controllers
{
    public class ProcessingRequestsController : BaseController
    {
        private readonly ProcessingsRequestsManager _processingsRequestsManager;
        private readonly EmployeesDaysReportsManager _employeesDaysReportsManager;
        private readonly IErpManager _erpManager;

        public ProcessingRequestsController(ProcessingsRequestsManager processingsRequestsManage
            ,EmployeesDaysReportsManager employeesDaysReportsManager ,IErpManager erpManager, IMapper mapper, ILogger<ProcessingRequestsController> logger) 
            : base(mapper, logger)
        {
            _processingsRequestsManager = processingsRequestsManage;
            _employeesDaysReportsManager = employeesDaysReportsManager;
            _erpManager = erpManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetAllProcessingRequestsForKendo([DataSourceRequest] DataSourceRequest request)
        {
            try
            {
                var allProcessingsData = await _processingsRequestsManager.GetAllAsNoTrackingListAsync();
                var allViewModels = _mapper.Map<List<ProcessingRequestViewModel>>(allProcessingsData);
                return Json(allViewModels.OrderByDescending(p => p.RequestDate).ToDataSourceResult(request));
            }
            catch (Exception)
            {
                return Json(new DataSourceResult() { Errors=new { message="خطأ اثناء قراءة البيانات" } });
            }
        }

        public async Task<IActionResult> AddNewProcessingRequest(DateTime fromDate, DateTime toDate, string employees)
        {
            try
            {

                var newData = new ProcessingRequest
                {
                    RequestDate = DateTime.Now,
                    FromDate = fromDate,
                    ToDate = toDate,
                    Employees = employees
                };

                if (User.Identity.IsAuthenticated)
                {
                    newData.UserId = User.Identity.Name.ConvertToInteger();
                }

                var saveResult = await _processingsRequestsManager.InsertNewDataItem(newData);

                if (saveResult.Status == RepositoryActionStatus.Created)
                {
                    return Json("Ok");
                }
                else
                {
                    return Json(new { errors = "Error" });
                }

            }
            catch (Exception)
            {
                return Json(new { errors = "Error" });
            }
        }

        public async Task<IActionResult> GetProcessingPercentageJson()
        {
            try
            {
                decimal result = 100;

                var unCompletedRequest = await _processingsRequestsManager.GetSingleItemAsync(p => p.Completed == false);
                if(unCompletedRequest != null)
                {
                    int daysCount = (int)unCompletedRequest.ToDate.Subtract(unCompletedRequest.FromDate).TotalDays + 1;
                    int employeesCount = unCompletedRequest.Employees?.TryParseToNumbers()?.Count() ?? 0;
                    if(employeesCount == 0)
                    {
                        var allEmployees = await _erpManager.GetShortEmployeesInfo();
                        employeesCount = allEmployees.Count;
                    }

                    int totalRecords = daysCount * employeesCount;

                    var completedDaysReports = await _employeesDaysReportsManager
                        .GetAll(d => d.ProcessingDate >= unCompletedRequest.RequestDate).AsNoTracking().CountAsync();

                    if (completedDaysReports > 0 && totalRecords > 0)
                    {
                        result = ((decimal)completedDaysReports / (decimal)totalRecords) * 100m;
                    }
                }

                return Json(new { Percentage = Math.Round(result,2) });
            }
            catch (Exception)
            {
                return Json(new { Percentage = 0 });
            }
        }

    }
}
