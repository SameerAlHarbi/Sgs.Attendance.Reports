using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using Sameer.Shared;
using Sgs.Attendance.Reports.Logic;
using Sgs.Attendance.Reports.Models;
using Sgs.Attendance.Reports.Services;
using Sgs.Attendance.Reports.ViewModels;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sgs.Attendance.Reports.Controllers
{
    [Authorize(Roles = "Admin,AttendanceDepartment")]
    public class EmployeesDaysReportsController : BaseController
    {
        private readonly EmployeesDaysReportsManager _employeesDaysReportsManager;
        private readonly AbsentsNotificationsManager _absentsNotificationsManager;
        private readonly RequestsReportsManager _requestsReportsManager;
        private readonly IErpManager _erpManager;
        private readonly IHostingEnvironment _env;

        public EmployeesDaysReportsController(IHostingEnvironment environment, EmployeesDaysReportsManager employeesDaysReportsManager,
            AbsentsNotificationsManager absentsNotificationsManager, RequestsReportsManager requestsReportsManager, IErpManager erpManager, IMapper mapper, ILogger<EmployeesDaysReportsController> logger) : base(mapper, logger)
        {
            _employeesDaysReportsManager = employeesDaysReportsManager;
            _absentsNotificationsManager = absentsNotificationsManager;
            _requestsReportsManager = requestsReportsManager;
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
            , int[] employeesIds = null, string[] departments = null, string absentNotes = "all"
            , bool summaryReport = true, int? pageNumber = null, int pageSize = 31)
        {
            try
            {
                startDate = startDate.Date;
                endDate = endDate.Date;

                employeesIds = employeesIds ?? new int[] { };
                departments = departments ?? new string[] { };

                var employeesIdsList = employeesIds.ToList();

                if (departments.Count() > 0)
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


                try
                {
                    string reportType = "Absents " + (pageNumber.HasValue ? "Page : 1" : "All Pages") + ", ";
                    reportType += $"Notes : {absentNotes} ,";
                    reportType += summaryReport ? "Summary" : "Details";

                    saveReportRequest(employeesIds, departments, startDate, endDate, reportType);
                }
                catch (Exception)
                {
                }

                var resultsQuery = _employeesDaysReportsManager
                    .GetAll(d => (employeesIds.Count() < 1 || employeesIds.Contains(d.EmployeeId))
                            && (absentNotes.Trim().ToLower() == "show"
                                || (absentNotes.Trim().ToLower() == "hide" && !d.IsDelegationRequest && !d.IsVacationRequest)
                                || (absentNotes.Trim().ToLower() == "notes" && (d.IsDelegationRequest || d.IsVacationRequest))
                                || (absentNotes.Trim().ToLower() == "vacations" && d.IsVacationRequest)
                                || (absentNotes.Trim().ToLower() == "delegations" && d.IsDelegationRequest))
                            && d.DayDate >= startDate && d.DayDate <= endDate && d.IsAbsentEmployee == true);

                if (pageNumber.HasValue)
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
                            TotalAbsentsDays = employeeDays.Count(),
                            AllAbsentsNotified = !employeeDays.Where(d => d.IsAbsentEmployee
                                              && !d.IsDelegationRequest
                                              && !d.IsVacationRequest).Any(d => !d.AbsentNotified),
                            AllAbsentsWithNotes = !employeeDays.Where(d => d.IsAbsentEmployee
                                             && !d.IsDelegationRequest
                                             && !d.IsVacationRequest).Any()
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
        public async Task<IActionResult> WasteReport(int years, int months, int day = 0
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

                var empsIds = await getEmployeesIds(employeesIds, departments);
                try
                {
                    saveReportRequest(empsIds.ToArray(), departments?.Split(',').ToArray() ?? new string[0], startDate, endDate, reportType);
                }
                catch (Exception)
                {
                }

                if (reportType == "Transactions")
                {
                    var daysCount = endDate.Date.Subtract(startDate.Date).TotalDays + 1;


                    if (daysCount > 1 && (empsIds.Count < 1 || empsIds.Count > 10))
                    {
                        throw new Exception("Transactions to much");
                    }
                    var transactionViewModels = await _erpManager.GetAllTransaction(startDate, endDate, empsIds);

                    //Temp Delete
                    //transactionViewModels = transactionViewModels.Except(transactionViewModels.Where(t => t.EmployeeId == 917)).ToList();

                    return PartialView("TransactionsReport", transactionViewModels.OrderBy(t => t.TransactionDate));
                }

                var resultViewModels = await getDetailsData(startDate, endDate
                        , employeesIds, departments, reportType, pageNumber, pageSize);

                var summaryViewModels = getSummaryData(startDate, endDate, resultViewModels);

                var employeesIdsList = resultViewModels.Select(d => d.EmployeeId).Distinct().ToList();


                if (string.IsNullOrWhiteSpace(reportType) || reportType == "summary" || (string.IsNullOrWhiteSpace(departments) && employeesIdsList.Count() > 1 && endDate.Subtract(startDate).TotalDays > 1))
                {
                    ViewBag.ShowAbsents = false;
                    ViewBag.ShowWaste = true;

                    return PartialView("WastesReport", summaryViewModels.Where(s => s.WasteDays > 0).OrderBy(s => s.EmployeeId).ToList());
                }
                else
                {
                    var departmentsNamesList = new List<string>();
                    if (!string.IsNullOrWhiteSpace(departments))
                    {
                        var departmentsList = departments?.Split(',').ToList() ?? new List<string>();
                        var allDepartments = await _erpManager.GetFlatDepartmentsInfo();
                        foreach (var dept in departmentsList)
                        {
                            var currentDept = allDepartments.FirstOrDefault(d => d.Code == dept);
                            if(currentDept != null)
                            {
                                departmentsNamesList.Add(currentDept.Name);
                            }
                        }
                    }
                    ViewBag.DepartmentName = string.Join(" - ", departmentsNamesList);
                    ViewBag.MultiDates = endDate.Subtract(startDate).TotalDays > 1;
                    ViewBag.MonthName = startDate.GetMonthName();
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

        public IEnumerable<EmployeeMonthReportViewModel> getSummaryData(DateTime startDate, DateTime endDate, IEnumerable<EmployeeDayReportViewModel> detailsData)
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

        public async Task<IActionResult> PartialEmployeeDetailsReport(int employeeId, DateTime fromDate, DateTime toDate)
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

            if (string.IsNullOrWhiteSpace(reportType) || reportType == "summary" || (string.IsNullOrWhiteSpace(departments) && employeesIdsList.Count() > 1 && endDate.Subtract(startDate).TotalDays > 1))
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
            else if (!string.IsNullOrWhiteSpace(departments))
            {
                var path = Path.Combine(_env.WebRootPath, "Excels/DepartmentReport.xlsx");
                var fileInfo = new FileInfo(path);

                using (ExcelPackage package = new ExcelPackage(fileInfo.OpenRead()))
                {
                    ExcelWorksheet excelWorksheet = package.Workbook.Worksheets[0];
                    int i = 2;

                    foreach (var data in resultViewModels
                        .Where(d => d.DayDate.DayOfWeek != DayOfWeek.Friday
                        && d.DayDate.DayOfWeek != DayOfWeek.Saturday).OrderBy(d => d.DayDate).ThenBy(d => d.EmployeeId))
                    {
                        try
                        {
                            int k = 1;
                            excelWorksheet.Cells[i, k++].Value = data.EmployeeId;
                            excelWorksheet.Cells[i, k++].Value = data.EmployeeName;
                            excelWorksheet.Cells[i, k++].Value = data.DepartmentName;
                            excelWorksheet.Cells[i, k++].Value = data.DayDate;
                            excelWorksheet.Cells[i, k++].Value = data.DayDate.ConvertToString(true, true, true);
                            excelWorksheet.Cells[i, k++].Value = data.CheckInDateTimeForReport;
                            excelWorksheet.Cells[i, k++].Value = data.CheckOutDateTime;
                            excelWorksheet.Cells[i, k++].Value = data.VacationName;
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

        private void saveReportRequest(int[] employeesIds, string[] departmentsList
            , DateTime fromDate, DateTime toDate, string reportType)
        {
            try
            {
                string filterText = string.Join(',', employeesIds);
                if (departmentsList.Count() > 0)
                {
                    filterText += filterText.Length > 0 ? "," : "";
                    filterText += string.Join(",", departmentsList);
                }

                if (User.Identity.IsAuthenticated)
                {
                    using (var context = new PrincipalContext(ContextType.Domain))
                    {
                        UserPrincipal principal = UserPrincipal.FindByIdentity(context, User.Identity.Name);
                        string userName = principal.Name;
                        var requestReport = new RequestReport()
                        {
                            ByUserId = User.Identity.Name,
                            ByUserName = userName,
                            FilterText = filterText.Length > 0 ? filterText : "All",
                            FromDate = fromDate,
                            ToDate = toDate,
                            ReportType = reportType,
                            RequestDate = DateTime.Now
                        };

                        _requestsReportsManager.InsertNew(requestReport);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        public async Task<IActionResult> NotifiAbsent(int absentId)
        {
            try
            {
                var absentInfo = await _employeesDaysReportsManager.GetByIdAsync(absentId);

                if (absentInfo == null)
                {
                    throw new Exception("No Data");
                }

                var result =  await _erpManager.NotifiAbsent(absentInfo.EmployeeId, absentInfo.DayDate);

                if (result)
                {
                    try
                    {
                        using (var context = new PrincipalContext(ContextType.Domain))
                        {
                            UserPrincipal principal = UserPrincipal.FindByIdentity(context, User.Identity.Name);
                            string userName = principal.Name;

                            absentInfo.AbsentNotified = true;
                            absentInfo.AbsentNotifiedDate = DateTime.Now;
                            absentInfo.AbsentNotifiedByEmployeeId = User.Identity.Name;
                            absentInfo.AbsentNotifiedByEmployeeName = principal.Name;
                        }

                        await _employeesDaysReportsManager.UpdateDataItem(absentInfo);

                        var currentNotificationLog = await _absentsNotificationsManager
                            .GetSingleItemAsync(n => n.EmployeeId == absentInfo.EmployeeId && n.AbsentDate == absentInfo.DayDate);

                        currentNotificationLog = currentNotificationLog ?? new AbsentNotification
                        {
                            AbsentDate = absentInfo.DayDate,
                            EmployeeId = absentInfo.EmployeeId
                        };

                        currentNotificationLog.ByEmployeeId = absentInfo.AbsentNotifiedByEmployeeId;
                        currentNotificationLog.ByEmployeeName = absentInfo.AbsentNotifiedByEmployeeName;
                        currentNotificationLog.SendDate = absentInfo.AbsentNotifiedDate.Value;

                        if (currentNotificationLog.Id < 1)
                        {
                            await _absentsNotificationsManager.InsertNewAsync(currentNotificationLog);
                        }
                        else
                        {
                            await _absentsNotificationsManager.UpdateItemAsync(currentNotificationLog);
                        }
                    }
                    catch (Exception)
                    {
                    }
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

        public async Task<IActionResult> NotifiAbsents(int employeeId, string fromDate, string toDate)
        {
            try
            {
                DateTime? fromDateObject = fromDate.TryParseToDate();
                DateTime? toDateObject = toDate.TryParseToDate();

                if (!fromDateObject.HasValue || !toDateObject.HasValue)
                {
                    throw new Exception("Date error");
                }

                var absentsInfo = await _employeesDaysReportsManager
                    .GetAllAsNoTrackingListAsync(d => d.EmployeeId == employeeId && d.IsAbsentEmployee && !d.IsDelegationRequest && !d.IsVacationRequest
                        && d.DayDate >= fromDateObject.Value && d.DayDate <= toDateObject.Value);

                if (absentsInfo == null || !absentsInfo.Any())
                {
                    throw new Exception("No Data");
                }

                string userName = "";
                using (var context = new PrincipalContext(ContextType.Domain))
                {
                    UserPrincipal principal = UserPrincipal.FindByIdentity(context, User.Identity.Name);
                    userName = principal.Name;
                }

                var currentNotificationsLog = new List<AbsentNotification>();

                try
                {
                    currentNotificationsLog = await _absentsNotificationsManager
                                .GetAllAsNoTrackingListAsync(n => n.EmployeeId == employeeId
                                    && n.AbsentDate >= fromDateObject.Value
                                    && n.AbsentDate <= toDateObject.Value);
                }
                catch (Exception)
                {
                }

                bool result = true;

                foreach (var absentInfo in absentsInfo)
                {

                    result = await _erpManager.NotifiAbsent(absentInfo.EmployeeId, absentInfo.DayDate);

                    try
                    {

                        absentInfo.AbsentNotified = true;
                        absentInfo.AbsentNotifiedDate = DateTime.Now;
                        absentInfo.AbsentNotifiedByEmployeeId = User.Identity.Name;
                        absentInfo.AbsentNotifiedByEmployeeName = userName;


                        await _employeesDaysReportsManager.UpdateDataItem(absentInfo);

                        var currentNotificationLog = currentNotificationsLog
                            .FirstOrDefault(n => n.EmployeeId == absentInfo.EmployeeId && n.AbsentDate == absentInfo.DayDate);

                        currentNotificationLog = currentNotificationLog ?? new AbsentNotification
                        {
                            AbsentDate = absentInfo.DayDate,
                            EmployeeId = absentInfo.EmployeeId
                        };

                        currentNotificationLog.ByEmployeeId = absentInfo.AbsentNotifiedByEmployeeId;
                        currentNotificationLog.ByEmployeeName = absentInfo.AbsentNotifiedByEmployeeName;
                        currentNotificationLog.SendDate = absentInfo.AbsentNotifiedDate.Value;

                        if (currentNotificationLog.Id < 1)
                        {
                            await _absentsNotificationsManager.InsertNewAsync(currentNotificationLog);
                        }
                        else
                        {
                            await _absentsNotificationsManager.UpdateItemAsync(currentNotificationLog);
                        }
                    }
                    catch (Exception)
                    {
                    }

                }

                if (result)
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
