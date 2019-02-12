using AutoMapper;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sameer.Shared;
using Sgs.Attendance.Reports.Helpers;
using Sgs.Attendance.Reports.Logic;
using Sgs.Attendance.Reports.Models;
using Sgs.Attendance.Reports.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sgs.Attendance.Reports.Controllers
{
    public class ProcessingRequestsController : BaseController
    {
        private readonly ProcessingsRequestsManager _processingsRequestsManager;
        public ProcessingRequestsController(ProcessingsRequestsManager processingsRequestsManage, IMapper mapper, ILogger<ProcessingRequestsController> logger) 
            : base(mapper, logger)
        {
            _processingsRequestsManager = processingsRequestsManage;
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
                return Json(allViewModels.ToDataSourceResult(request));
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
    }
}
