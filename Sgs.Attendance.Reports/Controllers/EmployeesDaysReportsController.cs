﻿using System;
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
using System.IO;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;

namespace Sgs.Attendance.Reports.Controllers
{
    public class EmployeesDaysReportsController : BaseController
    {
        private readonly EmployeesDaysReportsManager _employeesDaysReportsManager;
        private readonly IErpManager _erpManager;
        private readonly IHostingEnvironment _env;

        public EmployeesDaysReportsController(IHostingEnvironment environment,EmployeesDaysReportsManager employeesDaysReportsManager
            ,IErpManager erpManager,IMapper mapper, ILogger<EmployeesDaysReportsController> logger) : base(mapper, logger)
        {
            _employeesDaysReportsManager = employeesDaysReportsManager;
            _erpManager = erpManager;
            _env = environment;
        }

        public IActionResult Absents()
        {
            return View();
        }
        
        public IActionResult Wastes()
        {
            return View();
        }

        public IActionResult TransactionsReport()
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

                if (resultViewModels.Count > 0)
                {
                    employeesIds = resultViewModels.Select(d => d.EmployeeId).Distinct().ToArray();

                    //var erpEmployees = await _erpManager.GetEmployeesInfo(employeesIds);

                    //foreach (var erpEmp in erpEmployees)
                    //{
                    //    foreach (var dayReport in resultViewModels.Where(d => d.EmployeeId == erpEmp.EmployeeId))
                    //    {
                    //        dayReport.EmployeeName = erpEmp.Name;
                    //        dayReport.DepartmentName = erpEmp.DepartmentName;
                    //    }
                    //}
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
                            ToDate = endDate,
                            TotalAbsentsDays = employeeDays.Count()
                        };
                        summaryViewModels.Add(newSummary);
                    }

                    ViewBag.ShowAbsents = true;
                    ViewBag.ShowWaste = false;
                    return PartialView("WastesReport", summaryViewModels.OrderBy(s => s.EmployeeId).ToList());
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        public async Task<IActionResult> WasteReport(int years, int months,int day = 0
            , string employeesIds = null, string departments = null, string reportType = null, int? pageNumber = null, int pageSize = 31)
        {
            try
            {
                DateTime startDate = new DateTime(years, months, 1);
                DateTime endDate = new DateTime(years, months, 1).AddMonths(1).AddDays(-1);

                if (day > 0)
                {
                    startDate = new DateTime(years, months, day);
                    endDate = new DateTime(years, months, day);
                }

                if (reportType == "Transactions")
                {
                    var transactionViewModels = await _erpManager.GetAllTransaction(startDate, endDate, await getEmployeesIds(employeesIds, departments));

                    var allemployees = await _erpManager.GetShortEmployeesInfo();

                    Parallel.ForEach(transactionViewModels.GroupBy(t => t.EmployeeId), (source) => {
                        var emp = allemployees.FirstOrDefault(e => e.EmployeeId == source.Key);


                    });

                    return PartialView("TransactionsReport", transactionViewModels.OrderBy(t => t.EmployeeId));
                }

                var resultViewModels = await getDetailsData(startDate, endDate
                        , employeesIds, departments, reportType, pageNumber, pageSize);

                var summaryViewModels = getSummaryData(startDate, endDate, resultViewModels);

                var employeesIdsList = resultViewModels.Select(d => d.EmployeeId).Distinct().ToList();

               
                if (string.IsNullOrWhiteSpace(reportType) || reportType    == "summary" || (employeesIdsList.Count() > 1 && endDate.Subtract(startDate).TotalDays > 1))
                {
                    ViewBag.ShowAbsents = false;
                    ViewBag.ShowWaste = true;

                    return PartialView("WastesReport", summaryViewModels.Where(s => s.WasteDays > 0).OrderBy(s => s.EmployeeId).ToList());
                }
                else
                {
                    return PartialView("EmployeeDetailsMonthlyReport", summaryViewModels);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async Task<List<int>> getEmployeesIds(string employeesIds, string departments)
        {
            IEnumerable<decimal> listOfEmployeesIdsNumbers = employeesIds.TryParseToNumbers();

            var employeesIdsList = listOfEmployeesIdsNumbers?.Select(d => (int)d).ToList() ?? new List<int>();

            var departmentsList = departments?.Split(',').ToList() ?? new List<string>();

            if (departmentsList.Count() > 0)
            {
                var departmentsEmployees = new List<EmployeeInfoViewModel>();
                foreach (var department in departmentsList)
                {
                    departmentsEmployees.AddRange(await _erpManager.GetDepartmentEmployeesInfo(department));
                }

                if (departmentsEmployees.Any())
                {
                    employeesIdsList.AddRange(departmentsEmployees.Select(d => d.EmployeeId));
                    employeesIdsList = employeesIdsList.Distinct().OrderBy(e => e).ToList();
                }
            }

            return employeesIdsList;
        }

        public async Task<IEnumerable<EmployeeDayReportViewModel>> getDetailsData(DateTime startDate, DateTime endDate
            , string employeesIds = null, string departments = null, string reportType = null, int? pageNumber = null, int pageSize = 31)
        {
            try
            {
                List<int> employeesIdsList = await getEmployeesIds(employeesIds, departments);

                var resultsQuery = _employeesDaysReportsManager
                    .GetAll(d => (employeesIdsList.Count < 1 || employeesIdsList.Contains(d.EmployeeId))
                            && d.DayDate >= startDate && d.DayDate <= endDate
                            && (reportType == null || reportType == "fullDetails"
                            || (d.WasteDurationTime != null && d.WasteDurationTime.Value > new TimeSpan(0, 0, 0))));

                if (pageNumber.HasValue)
                {
                    resultsQuery = resultsQuery.Skip((pageNumber.Value - 1) * pageSize).Take(pageSize);
                }

                var resultsData = await resultsQuery.AsNoTracking().ToListAsync();
                var resultViewModels = _mapper.Map<List<EmployeeDayReportViewModel>>(resultsData);

                return resultViewModels.OrderBy(d => d.EmployeeId).ThenBy(d => d.DayDate).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IEnumerable<EmployeeMonthReportViewModel> getSummaryData(DateTime startDate,DateTime endDate,IEnumerable<EmployeeDayReportViewModel> detailsData)
        {
            try
            {
                var summaryViewModels = new List<EmployeeMonthReportViewModel>();
                foreach (var employeeDays in detailsData.GroupBy(e => e.EmployeeId))
                {
                    var newSummary = new EmployeeMonthReportViewModel
                    {
                        EmployeeId = employeeDays.Key,
                        EmployeeName = employeeDays.First().EmployeeName,
                        DepartmentName = employeeDays.First().DepartmentName,
                        ProcessingDate = employeeDays.Max(d => d.ProcessingDate),
                        StartDate = startDate,
                        ToDate = endDate,
                        ContractWorkDurationAvarage = employeeDays.Where(d => d.ContractWorkDuration.HasValue).Count() > 0 ? employeeDays.Where(d => d.ContractWorkDuration.HasValue)
                            .Average(d => d.ContractWorkDuration.Value) : 0,
                        TotalActualWorkDuration = employeeDays
                            .Sum(d => d.ActualWorkDuration.HasValue ? d.ActualWorkDuration.Value : 0),
                        TotalContractWorkDuration = employeeDays
                            .Sum(d => d.ContractWorkDuration.HasValue ? d.ContractWorkDuration.Value : 0),
                        TotalWasteHours = employeeDays
                            .Sum(d => d.WasteDuration.HasValue ? d.WasteDuration.Value : 0),
                        DayReportsList = employeeDays.ToList(),
                        TotalWasteDays = employeeDays.Count(d => d.IsAbsentEmployee),
                        VacationsDays = employeeDays.Count(d => d.IsVacation && d.VacationRegisterDate.HasValue),
                        
                    };

                    newSummary.ContractWorkDurationAvarageTime = newSummary.ContractWorkDurationAvarage.ConvertToTime();
                    newSummary.TotalActualWorkDurationTime = newSummary.TotalActualWorkDuration.ConvertToTime();
                    newSummary.TotalContractWorkDurationTime = newSummary.TotalContractWorkDuration.ConvertToTime();
                    newSummary.TotalWasteHoursTime = newSummary.TotalWasteHours.ConvertToTime();

                    newSummary.SingleProofDays = employeeDays.Where(d => d.WasteDuration >= d.ContractWorkDuration).Count();

                    newSummary.WasteHours = newSummary.TotalWasteHours - newSummary.ContractWorkDurationAvarage;

                    if (newSummary.SingleProofDays > 0)
                    {
                        newSummary.WasteHours -= newSummary.ContractWorkDurationAvarage;
                    }

                    newSummary.WasteHours = newSummary.WasteHours > 0 ? newSummary.WasteHours : 0;

                    newSummary.WasteHoursTime = newSummary.WasteHours.ConvertToTime();

                    if (newSummary.WasteHours > 0 && newSummary.ContractWorkDurationAvarage > 0)
                    {
                        newSummary.WasteDays = (int)(newSummary.WasteHours / newSummary.ContractWorkDurationAvarage);
                    }

                    //newSummary.VacationsDays = employeeDays.Where(d => d.IsVacation && d.VacationRegisterDate.HasValue).Count();


                    summaryViewModels.Add(newSummary);
                }

                return summaryViewModels;
            }
            catch (Exception)
            {
                throw;
            }
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

        [HttpGet]
        public async Task<IActionResult> GetDetailsReport(int years, int months, int day = 0
            , string employeesIds = null, string departments = null, string reportType = null, int? pageNumber = null, int pageSize = 31)
        {
            DateTime startDate = new DateTime(years, months, 1);
            DateTime endDate = new DateTime(years, months, 1).AddMonths(1).AddDays(-1);

            if (day > 0)
            {
                startDate = new DateTime(years, months, day);
                endDate = new DateTime(years, months, day);
            }

            var resultViewModels = await getDetailsData(startDate, endDate
                    , employeesIds, departments, reportType, pageNumber, pageSize);

            var summaryViewModels = getSummaryData(startDate, endDate, resultViewModels);

            var employeesIdsList = resultViewModels.Select(d => d.EmployeeId).Distinct().ToList();

            if (string.IsNullOrWhiteSpace(reportType) || reportType == "summary" || (employeesIdsList.Count() > 1 && endDate.Subtract(startDate).TotalDays > 1))
            {
                summaryViewModels = summaryViewModels.Where(s => s.WasteDays > 0).OrderBy(s => s.EmployeeId).ToList();

                var path = Path.Combine(_env.WebRootPath, "Excels/Summary.xlsx");
                var fileInfo = new FileInfo(path);

                using (ExcelPackage package = new ExcelPackage(fileInfo.OpenRead()))
                {
                    ExcelWorksheet excelWorksheet = package.Workbook.Worksheets[0];
                    int i = 2;

                    foreach (var data in summaryViewModels)
                    {
                        try
                        {
                            int k = 1;
                            excelWorksheet.Cells[i, k++].Value = data.EmployeeId;
                            excelWorksheet.Cells[i, k++].Value = data.EmployeeName;
                            excelWorksheet.Cells[i, k++].Value = data.DepartmentName;
                            excelWorksheet.Cells[i, k++].Value = data.StartDate;
                            excelWorksheet.Cells[i, k++].Value = data.StartDate.ConvertToString(true, true, true);
                            excelWorksheet.Cells[i, k++].Value = data.ToDate;
                            excelWorksheet.Cells[i, k++].Value = data.ToDate.ConvertToString(true, true, true);
                            excelWorksheet.Cells[i, k++].Value = data.WasteDays;
                            //excelWorksheet.Cells[i, k++].Value = data.VacationsDays;
                            //excelWorksheet.Cells[i, k++].Value = data.TotalAbsentsDays;
                            i++;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }

                    var fileStream = new MemoryStream();
                    package.SaveAs(fileStream);
                    fileStream.Position = 0;

                    var fsr = new FileStreamResult(fileStream, contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                    fsr.FileDownloadName = $"Details{startDate.Year}-{startDate.Month}.xlsx";
                    return fsr;
                }
            }
            else
            {
                var path = Path.Combine(_env.WebRootPath, "Excels/Details.xlsx");
                var fileInfo = new FileInfo(path);

                using (ExcelPackage package = new ExcelPackage(fileInfo.OpenRead()))
                {
                    ExcelWorksheet excelWorksheet = package.Workbook.Worksheets[0];
                    int i = 2;

                    foreach (var data in resultViewModels)
                    {
                        try
                        {
                            int k = 1;
                            excelWorksheet.Cells[i, k++].Value = data.EmployeeId;
                            excelWorksheet.Cells[i, k++].Value = data.EmployeeName;
                            excelWorksheet.Cells[i, k++].Value = data.DepartmentName;
                            excelWorksheet.Cells[i, k++].Value = data.DayDate;
                            excelWorksheet.Cells[i, k++].Value = data.DayDate.ConvertToString(true, true, true);
                            excelWorksheet.Cells[i, k++].Value = data.ContractWorkDurationTime;
                            excelWorksheet.Cells[i, k++].Value = data.CheckInDateTimeForReport;
                            excelWorksheet.Cells[i, k++].Value = data.CheckOutDateTime;
                            excelWorksheet.Cells[i, k++].Value = data.WorkDurationTime;
                            excelWorksheet.Cells[i, k++].Value = data.CheckInLateDurationTime;
                            excelWorksheet.Cells[i, k++].Value = data.CheckOutEarlyDurationTime;
                            excelWorksheet.Cells[i, k++].Value = data.WasteDurationTime;
                            excelWorksheet.Cells[i, k++].Value = data.VacationName;
                            excelWorksheet.Cells[i, k++].Value = data.CheckInExcuse ? data.CheckInExcuseHours.ToString() : string.Empty;
                            excelWorksheet.Cells[i, k++].Value = data.CheckOutExcuse ? data.CheckOutExcuseHours.ToString() : string.Empty;
                            excelWorksheet.Cells[i, k++].Value = data.IsDelegationRequest ? "طلب انتداب" : data.IsVacationRequest ? "طلب إجازة" : string.Empty;

                            i++;
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }

                    var fileStream = new MemoryStream();
                    package.SaveAs(fileStream);
                    fileStream.Position = 0;

                    var fsr = new FileStreamResult(fileStream, contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                    fsr.FileDownloadName = $"Details{startDate.Year}-{startDate.Month}.xlsx";
                    return fsr;
                }
            }
        }
    }
}
