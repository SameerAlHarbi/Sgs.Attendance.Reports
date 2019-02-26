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
                            &&  d.DayDate >= startDate && d.DayDate <= endDate && d.IsAbsentEmployee == true);

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
            catch (Exception ex)
            {
                throw;
            }
        }

        public IActionResult EmployeeMonthReport()
        {
            return View();
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
