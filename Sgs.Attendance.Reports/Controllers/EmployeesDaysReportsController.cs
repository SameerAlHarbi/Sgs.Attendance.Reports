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
        
        public IActionResult Wastes()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AbsentsReport(DateTime startDate, DateTime endDate
            , int[] employeesIds = null,string[] departments=null, string absentNotes = "all"
            , bool summaryReport = true,int? pageNumber = null,int pageSize=31)
        {
            try
            {
                startDate = startDate.Date;
                endDate = endDate.Date;

                employeesIds = employeesIds ?? new int[] { } ;
                departments = departments ?? new string[] { };

                var employeesIdsList = employeesIds.ToList();

                if(departments.Count() > 0)
                {
                    var departmentsEmployees = new List<EmployeeInfoViewModel>();
                    foreach (var department in departments)
                    {
                        departmentsEmployees.AddRange(await _erpManager.GetDepartmentEmployeesInfo(department));
                    }

                    if (departmentsEmployees.Any())
                    {
                        employeesIdsList.AddRange(departmentsEmployees.Select(d => d.EmployeeId));
                        employeesIds = employeesIdsList.Distinct().OrderBy(e => e).ToArray();
                    }
                }

                var resultsQuery =  _employeesDaysReportsManager
                    .GetAll(d => (employeesIds.Count() < 1 || employeesIds.Contains(d.EmployeeId)) 
                            && (absentNotes.Trim().ToLower() == "show"
                                || (absentNotes.Trim().ToLower() == "hide" && !d.IsDelegationRequest && !d.IsVacationRequest)
                                || (absentNotes.Trim().ToLower() == "notes" && (d.IsDelegationRequest || d.IsVacationRequest))
                                || (absentNotes.Trim().ToLower() == "vacations" && d.IsVacationRequest)
                                || (absentNotes.Trim().ToLower() == "delegations" && d.IsDelegationRequest))
                            &&  d.DayDate >= startDate && d.DayDate <= endDate && d.IsAbsentEmployee == true);

                if(pageNumber.HasValue)
                {
                    resultsQuery = resultsQuery.Skip((pageNumber.Value - 1) * pageSize).Take(pageSize);
                }

                var resultsData = await resultsQuery.AsNoTracking().ToListAsync();
                var resultViewModels = _mapper.Map<List<EmployeeDayReportViewModel>>(resultsData);

                if (resultViewModels.Count>0)
                {
                    employeesIds = resultViewModels.Select(d => d.EmployeeId).Distinct().ToArray();

                    var erpEmployees = await _erpManager.GetEmployeesInfo(employeesIds);

                    foreach (var erpEmp in erpEmployees)
                    {
                        foreach (var dayReport in resultViewModels.Where(d => d.EmployeeId == erpEmp.EmployeeId))
                        {
                            dayReport.EmployeeName = erpEmp.Name;
                            dayReport.DepartmentName = erpEmp.DepartmentName;
                        }
                    } 
                }

                resultViewModels = resultViewModels.OrderBy(d => d.EmployeeId).ThenBy(d => d.DayDate).ToList();

                if (!summaryReport)
                {
                    return PartialView(resultViewModels);
                }
                else
                {
                    var summaryViewModels = new List<EmployeeMonthReportViewModel>();
                    foreach (var employeeDays in resultViewModels.GroupBy(e => e.EmployeeId))
                    {
                        var newSummary = new EmployeeMonthReportViewModel
                        {
                            EmployeeId = employeeDays.Key,
                            EmployeeName = employeeDays.First().EmployeeName,
                            DepartmentName = employeeDays.First().DepartmentName,
                            ProcessingDate = employeeDays.Max(d => d.ProcessingDate),
                            StartDate = startDate,
                            ToDate = employeeDays.Max(d => d.DayDate),
                            TotalAbsentsDays = employeeDays.Count()
                        };
                        summaryViewModels.Add(newSummary);
                    }
                    return PartialView("WastesReport", summaryViewModels.OrderBy(s => s.EmployeeId).ToList());
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> WastesReport(DateTime startDate, DateTime endDate, string employeesIds = null, int? pageNumber = null, int pageSize = 31)
        {
             
            return PartialView(new List<EmployeeMonthReportViewModel>
            {
                new EmployeeMonthReportViewModel{EmployeeId = 1143,EmployeeName="Osama Mohammed Najjar", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),TotalAbsentsDays=0,TotalWasteDays=1}
            });
        }

        public IActionResult EmployeeDetailsMonthlyReport()
        {
            return View();
        }

        public async Task<IActionResult> PartialEmployeeDetailsReport(int employeeId,DateTime fromDate,DateTime toDate)
        {
            var results = await _employeesDaysReportsManager.GetAllAsNoTrackingListAsync(d => d.EmployeeId == employeeId);

            return PartialView("EmployeeDetailsMonthlyReport", results);
        }

        public async Task<IActionResult> EmployeeDetailsReport(int employeeId, DateTime fromDate, DateTime toDate)
        {
            var results = await _employeesDaysReportsManager.GetAllAsNoTrackingListAsync(d => d.EmployeeId == employeeId);
            return View(results);
        }

    }
}
