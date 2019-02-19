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

                var employeesIdsNumbers = employeesIds.TryParseToNumbers()?.Select(d => (int)d).ToList() ?? new List<int>();

                var resultsQuery =  _employeesDaysReportsManager
                    .GetAll(d => (employeesIdsNumbers.Count < 1 || employeesIdsNumbers.Contains(d.EmployeeId)) 
                            &&  d.DayDate >= startDate && d.DayDate <= endDate);

                if(pageNumber.HasValue)
                {
                    resultsQuery = resultsQuery.Skip((pageNumber.Value - 1) * pageSize).Take(pageSize);
                }

                var resultsData = await resultsQuery.AsNoTracking().ToListAsync();
                var resultViewModels = _mapper.Map<List<EmployeeDayReportViewModel>>(resultsData);

                if (resultViewModels.Count>0)
                {
                    employeesIdsNumbers = resultViewModels.Select(d => d.EmployeeId).Distinct().ToList();

                    var erpEmployees = await _erpManager.GetEmployeesInfo(employeesIdsNumbers);

                    foreach (var erpEmp in erpEmployees)
                    {
                        foreach (var dayReport in resultViewModels.Where(d => d.EmployeeId == erpEmp.EmployeeId))
                        {
                            dayReport.EmployeeName = erpEmp.Name;
                            dayReport.DepartmentName = erpEmp.DepartmentName;
                        }
                    } 
                }

                return PartialView(resultViewModels);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IActionResult EmployeeMonthReport()
        {
            return View(new List<EmployeeMonthReportViewModel>
                {
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 917 , EmployeeName ="سمير محمد الحربي", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="تقنية المعلومات والاتصالات" , TotalAbsentsDays=0,TotalWasteDays=2},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 917 , EmployeeName ="سمير محمد الحربي", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="تقنية المعلومات والاتصالات" , TotalAbsentsDays=0,TotalWasteDays=2},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0},
                    new EmployeeMonthReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", StartDate=new DateTime(2019,1,1),ToDate=new DateTime(2019,1,30),DepartmentName="الخدمات الالكترونية" ,TotalAbsentsDays=1,TotalWasteDays=0}

                });
        }

        public IActionResult AbsentsReport()
        {
            return View(new List<EmployeeDayReportViewModel>
                {
                    new EmployeeDayReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", DayDate=new DateTime(2019,1,1),DepartmentName="الخدمات الالكترونية" ,IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", DayDate=new DateTime(2019,1,1),DepartmentName="الخدمات الالكترونية" ,IsDelegationRequest=true},
                    new EmployeeDayReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", DayDate=new DateTime(2019,1,1),DepartmentName="الخدمات الالكترونية" ,IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", DayDate=new DateTime(2019,1,1),DepartmentName="الخدمات الالكترونية" ,IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", DayDate=new DateTime(2019,1,1),DepartmentName="الخدمات الالكترونية" ,IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", DayDate=new DateTime(2019,1,1),DepartmentName="الخدمات الالكترونية" ,IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", DayDate=new DateTime(2019,1,1),DepartmentName="الخدمات الالكترونية" ,IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", DayDate=new DateTime(2019,1,1),DepartmentName="الخدمات الالكترونية" ,IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", DayDate=new DateTime(2019,1,1),DepartmentName="الخدمات الالكترونية" ,IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", DayDate=new DateTime(2019,1,1),DepartmentName="الخدمات الالكترونية" ,IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", DayDate=new DateTime(2019,1,1),DepartmentName="الخدمات الالكترونية" ,IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", DayDate=new DateTime(2019,1,1),DepartmentName="الخدمات الالكترونية" ,IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", DayDate=new DateTime(2019,1,1),DepartmentName="الخدمات الالكترونية" ,IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", DayDate=new DateTime(2019,1,1),DepartmentName="الخدمات الالكترونية" ,IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", DayDate=new DateTime(2019,1,1),DepartmentName="الخدمات الالكترونية" ,IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", DayDate=new DateTime(2019,1,1),DepartmentName="الخدمات الالكترونية" ,IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", DayDate=new DateTime(2019,1,1),DepartmentName="الخدمات الالكترونية" ,IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", DayDate=new DateTime(2019,1,1),DepartmentName="الخدمات الالكترونية" ,IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", DayDate=new DateTime(2019,1,1),DepartmentName="الخدمات الالكترونية" ,IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", DayDate=new DateTime(2019,1,1),DepartmentName="الخدمات الالكترونية" ,IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", DayDate=new DateTime(2019,1,1),DepartmentName="الخدمات الالكترونية" ,IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", DayDate=new DateTime(2019,1,1),DepartmentName="الخدمات الالكترونية" ,IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", DayDate=new DateTime(2019,1,1),DepartmentName="الخدمات الالكترونية" ,IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", DayDate=new DateTime(2019,1,1),DepartmentName="الخدمات الالكترونية" ,IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", DayDate=new DateTime(2019,1,1),DepartmentName="الخدمات الالكترونية" ,IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", DayDate=new DateTime(2019,1,1),DepartmentName="الخدمات الالكترونية" ,IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", DayDate=new DateTime(2019,1,1),DepartmentName="الخدمات الالكترونية" ,IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", DayDate=new DateTime(2019,1,1),DepartmentName="الخدمات الالكترونية" ,IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", DayDate=new DateTime(2019,1,1),DepartmentName="الخدمات الالكترونية" ,IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", DayDate=new DateTime(2019,1,1),DepartmentName="الخدمات الالكترونية" ,IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", DayDate=new DateTime(2019,1,1),DepartmentName="الخدمات الالكترونية" ,IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ EmployeeId = 1143 , EmployeeName ="اسامه محمد نجار", DayDate=new DateTime(2019,1,1),DepartmentName="الخدمات الالكترونية" ,IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ EmployeeId = 917 , EmployeeName ="سمير محمد الحربي", DayDate=new DateTime(2019,2,1),DepartmentName="الخدمات الالكترونية" ,IsDelegationRequest=true},
                });
        }

        public IActionResult EmployeeDetailsMonthlyReport()
        {
            return View(new List<EmployeeDayReportViewModel>
                {
                    new EmployeeDayReportViewModel{ DayDate=new DateTime(2019,1,1),CheckInLateDurationTime=new TimeSpan(0,0,0),ActualCheckInDateTime=new DateTime(2019,1,1,7,30,10),ActualCheckOutDateTime=new DateTime(2019,1,1,14,33,10),VacationTypeName="",IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ DayDate=new DateTime(2019,1,2),ActualCheckInDateTime=new DateTime(2019,1,2,7,54,40),ActualCheckOutDateTime=new DateTime(2019,1,2,14,45,10),VacationTypeName="",IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ DayDate=new DateTime(2019,1,3),ActualCheckInDateTime=new DateTime(2019,1,3,7,43,40),ActualCheckOutDateTime=new DateTime(2019,1,3,14,55,10),VacationTypeName="",IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ DayDate=new DateTime(2019,1,4),VacationTypeName="اجازة نهاية الاسبوع",IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ DayDate=new DateTime(2019,1,5),VacationTypeName="اجازة نهاية الاسبوع",IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ DayDate=new DateTime(2019,1,6),ActualCheckInDateTime=new DateTime(2019,1,6,8,22,40),CheckInLateDuration=0.52,ActualCheckOutDateTime=new DateTime(2019,1,6,14,10,10),CheckOutEarlyDuration=0.20,VacationTypeName="",IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ DayDate=new DateTime(2019,1,7),ActualCheckInDateTime=new DateTime(2019,1,7,8,43,40),CheckInLateDuration=1.13,ActualCheckOutDateTime=new DateTime(2019,1,3,14,43,10),VacationTypeName="",IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ DayDate=new DateTime(2019,1,8),IsAbsentEmployee=true,IsDelegationRequest=true},
                    new EmployeeDayReportViewModel{ DayDate=new DateTime(2019,1,9),IsAbsentEmployee=true,IsDelegationRequest=true},
                    new EmployeeDayReportViewModel{ DayDate=new DateTime(2019,1,10),IsAbsentEmployee=true,IsDelegationRequest=true},
                    new EmployeeDayReportViewModel{ DayDate=new DateTime(2019,1,11),VacationTypeName="اجازة نهاية الاسبوع",IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ DayDate=new DateTime(2019,1,12),VacationTypeName="اجازة نهاية الاسبوع",IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ DayDate=new DateTime(2019,1,13),ActualCheckInDateTime=new DateTime(2019,1,13,8,55,40),CheckInLateDuration=1.25,ActualCheckOutDateTime=new DateTime(2019,1,13,13,58,10),CheckOutEarlyDuration=0.32,VacationTypeName="",IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ DayDate=new DateTime(2019,1,14),ActualCheckInDateTime=new DateTime(2019,1,14,7,22,40),ActualCheckOutDateTime=new DateTime(2019,1,14,14,22,10),CheckOutEarlyDuration=0.8,VacationTypeName="",IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ DayDate=new DateTime(2019,1,15),ActualCheckInDateTime=new DateTime(2019,1,15,6,59,40),ActualCheckOutDateTime=new DateTime(2019,1,15,14,43,10),VacationTypeName="",IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ DayDate=new DateTime(2019,1,16),ActualCheckInDateTime=new DateTime(2019,1,16,7,53,40),ActualCheckOutDateTime=new DateTime(2019,1,16,14,38,10),VacationTypeName="",IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ DayDate=new DateTime(2019,1,17),ActualCheckInDateTime=new DateTime(2019,1,17,8,45,40),ActualCheckOutDateTime=new DateTime(2019,1,17,15,10,10),VacationTypeName="",IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ DayDate=new DateTime(2019,1,18),ActualCheckInDateTime= null ,ActualCheckOutDateTime=null ,VacationTypeName="اجازة نهاية الاسبوع",IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ DayDate=new DateTime(2019,1,19),ActualCheckInDateTime= null ,ActualCheckOutDateTime=null ,VacationTypeName="اجازة نهاية الاسبوع",IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ DayDate=new DateTime(2019,1,20),CheckInLateDurationTime=new TimeSpan(1,21,0),ActualCheckInDateTime=new DateTime(2019,1,20,8,55,40),ActualCheckOutDateTime=new DateTime(2019,1,13,13,58,10),VacationTypeName="",IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ DayDate=new DateTime(2019,1,21),ActualCheckInDateTime=new DateTime(2019,1,21,7,22,40),ActualCheckOutDateTime=new DateTime(2019,1,14,14,22,10),VacationTypeName="",IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ DayDate=new DateTime(2019,1,22),CheckInLateDurationTime=new TimeSpan(0,0,0),ActualCheckInDateTime=null,ActualCheckOutDateTime=null,VacationTypeName="",IsDelegationRequest=false,IsAbsentEmployee=true},
                    new EmployeeDayReportViewModel{ DayDate=new DateTime(2019,1,23),ActualCheckInDateTime=new DateTime(2019,1,23,7,53,40),ActualCheckOutDateTime=new DateTime(2019,1,16,14,38,10),VacationTypeName="",IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ DayDate=new DateTime(2019,1,24),ActualCheckInDateTime=null,ActualCheckOutDateTime=null,VacationTypeName="اجازة اعتيادية",IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ DayDate=new DateTime(2019,1,25),ActualCheckInDateTime= null ,ActualCheckOutDateTime=null ,VacationTypeName="اجازة نهاية الاسبوع",IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ DayDate=new DateTime(2019,1,26),ActualCheckInDateTime= null ,ActualCheckOutDateTime=null ,VacationTypeName="اجازة نهاية الاسبوع",IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ DayDate=new DateTime(2019,1,27),ActualCheckInDateTime=null,ActualCheckOutDateTime=null,VacationTypeName="اجازة اعتيادية",IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ DayDate=new DateTime(2019,1,28),ActualCheckInDateTime=null,ActualCheckOutDateTime=null,VacationTypeName="اجازة اعتيادية",IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ DayDate=new DateTime(2019,1,29),ActualCheckInDateTime=null,ActualCheckOutDateTime=null,VacationTypeName="اجازة اعتيادية",IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ DayDate=new DateTime(2019,1,30),ActualCheckInDateTime=null,ActualCheckOutDateTime=null,VacationTypeName="اجازة اعتيادية",IsDelegationRequest=false},
                    new EmployeeDayReportViewModel{ DayDate=new DateTime(2019,1,31),ActualCheckInDateTime=null,ActualCheckOutDateTime=null,VacationTypeName="اجازة اعتيادية",IsDelegationRequest=false},


                });
        }

    }
}
