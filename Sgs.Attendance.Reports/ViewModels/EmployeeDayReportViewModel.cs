using Sgs.Attendance.Reports.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sgs.Attendance.Reports.ViewModels
{
    public class EmployeeDayReportViewModel
    {
        public int Id { get; set; }

        public DateTime DayDate { get; set; }

        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; }

        public string DepartmentName { get; set; }

        public bool IsVacation { get; set; }

        public string VacationTypeName { get; set; }

        public DateTime? VacationRegisterDate { get; set; }

        public bool IsDelegationRequest { get; set; }

        public DateTime? DelegationRegisterDate { get; set; }

        public DateTime? ContractCheckInDateTime { get; set; }

        public DateTime? ContractCheckOutDateTime { get; set; }

        public TimeSpan ContractWorkDurationTime { get; set; }

        public double ContractWorkDuration { get; set; }

        public DateTime? ActualCheckInDateTime { get; set; }

        public DateTime? ActualCheckOutDateTime { get; set; }

        public TimeSpan ActualWorkDurationTime { get; set; }

        public double ActualWorkDuration { get; set; }

        public DateTime? CheckInDateTime { get; set; }

        public DateTime? CheckOutDateTime { get; set; }

        public TimeSpan WorkDurationTime { get; set; }

        public double WorkDuration { get; set; }

        public TimeSpan CheckInLateDurationTime { get; set; }

        public double CheckInLateDuration { get; set; }

        public TimeSpan CheckOutEarlyDurationTime { get; set; }

        public double CheckOutEarlyDuration { get; set; }

        public TimeSpan WasteDurationTime { get; set; }

        public double WasteDuration { get; set; }

        public AttendanceProof AttendanceProof { get; set; }

        public bool CheckInExcuse { get; set; }

        public double CheckInExcuseHours { get; set; }

        public bool CheckOutExcuse { get; set; }

        public double CheckOutExcuseHours { get; set; }

        public bool IsAbsentEmployee { get; set; }

        public DateTime ProcessingDate { get; set; }
    }
}
