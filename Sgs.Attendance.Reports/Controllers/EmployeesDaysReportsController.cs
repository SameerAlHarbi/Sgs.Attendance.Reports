using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sgs.Attendance.Reports.Logic;
using Sgs.Attendance.Reports.ViewModels;
using Sameer.Shared;
using Microsoft.EntityFrameworkCore;
using Sgs.Attendance.Reports.Services;

namespace Sgs.Attendance.Reports.Controllers
{
    public class EmployeesDaysReportsController : BaseController
    {
        private readonly EmployeesDaysReportsManager _employeesDaysReportsManager;
        private readonly IErpManager _erpManager;

        public EmployeesDaysReportsController(EmployeesDaysReportsManager employeesDaysReportsManager
            ,IErpManager erpManager,IMapper mapper, ILogger<EmployeesDaysReportsController> logger) : base(mapper, logger)
        {
            _employeesDaysReportsManager = employeesDaysReportsManager;
            _erpManager = erpManager;
        }

        public IActionResult Absents()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AbsentsReport(DateTime startDate,DateTime endDate,string employeesIds = null,int? pageNumber = null,int pageSize=31)
        {
            try
            {
                startDate = startDate.Date;
                endDate = endDate.Date;

                var employeesIdsNumbers = employeesIds.TryParseToNumbers()?.Select(d => (int)d).ToList();

                var resultsQuery =  _employeesDaysReportsManager
                    .GetAll(d => (employeesIdsNumbers.Count < 1 || employeesIdsNumbers.Contains(d.EmployeeId)) 
                            &&  d.DayDate >= startDate && d.DayDate <= endDate);

                if(pageNumber.HasValue)
                {
                    resultsQuery = resultsQuery.Skip((pageNumber.Value - 1) * pageSize).Take(pageSize);
                }

                var resultsData = await resultsQuery.AsNoTracking().ToListAsync();
                var resultViewModels = _mapper.Map<List<EmployeeDayReportViewModel>>(resultsData);

                employeesIdsNumbers = resultViewModels.Select(d => d.EmployeeId).Distinct().ToList();

                var erpEmployees = await _erpManager.GetEmployeesInfo(employeesIdsNumbers);

                foreach (var erpEmp in erpEmployees)
                {
                    foreach (var dayReport in resultViewModels.Where(d => d.EmployeeId==erpEmp.EmployeeId))
                    {
                        dayReport.EmployeeName = erpEmp.Name;
                        dayReport.DepartmentName = erpEmp.DepartmentName;
                    }
                }

                return PartialView(resultViewModels);

                //return PartialView(new List<EmployeeDayReportViewModel>
                //{
                //    new EmployeeDayReportViewModel{ Id = 12}
                //});
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
