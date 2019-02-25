using Sameer.Shared;
using Sgs.Attendance.Reports.Models;
using System;

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

        public string VacationName { get; set; }

        public DateTime? VacationRegisterDate { get; set; }

        public bool IsDelegationRequest { get; set; }

        public DateTime? DelegationRequestDate { get; set; }

        public bool IsVacationRequest { get; set; }

        public DateTime? VacationRequestDate { get; set; }

        public DateTime? ContractCheckInDateTime { get; set; }

        public DateTime? ContractCheckOutDateTime { get; set; }

        public TimeSpan? ContractWorkDurationTime { get; set; }

        public double? ContractWorkDuration => ContractWorkDurationTime.HasValue ?
            ContractWorkDurationTime.Value.ConvertToDouble() : default(double?);

        public DateTime? ActualCheckInDateTime { get; set; }

        public DateTime? ActualCheckOutDateTime { get; set; }

        public TimeSpan? ActualWorkDurationTime { get; set; }

        public TimeSpan? ActualTotalDurationTime { get; set; }

        public double? ActualWorkDuration => ActualWorkDurationTime.HasValue ?
            ActualWorkDurationTime.Value.ConvertToDouble() : default(double?);

        public double? ActualTotalDuration => ActualTotalDurationTime.HasValue ?
            ActualTotalDurationTime.Value.ConvertToDouble() : default(double?);

        public DateTime? CheckInDateTime { get; set; }

        public DateTime? CheckOutDateTime { get; set; }

        public TimeSpan? WorkDurationTime { get; set; }

        public TimeSpan? WorkTotalDurationTime { get; set; }

        public double? WorkDuration => WorkDurationTime.HasValue ?
            WorkDurationTime.Value.ConvertToDouble() : default(double?);

        public double? WorkTotalDuration => WorkTotalDurationTime.HasValue ?
            WorkTotalDurationTime.Value.ConvertToDouble() : default(double?);

        public TimeSpan? CheckInLateDurationTime { get; set; }

        public double? CheckInLateDuration => CheckInLateDurationTime.HasValue ?
            CheckInLateDurationTime.Value.ConvertToDouble() : default(double?);

        public bool IsLate => CheckInLateDuration.HasValue 
            && CheckInLateDuration.Value > 0d;

        public TimeSpan? CheckOutEarlyDurationTime { get; set; }

        public double? CheckOutEarlyDuration => CheckOutEarlyDurationTime.HasValue ?
            CheckOutEarlyDurationTime.Value.ConvertToDouble() : default(double?);

        public bool IsEarly => CheckOutEarlyDuration > 0;

        public TimeSpan? WasteDurationTime { get; set; }

        public double? WasteDuration => WasteDurationTime.HasValue ?
            WasteDurationTime.Value.ConvertToDouble() : default(double?);

        public AttendanceProof AttendanceProof { get; set; }

        public bool CheckInExcuse { get; set; }

        public double CheckInExcuseHours { get; set; }

        public bool CheckOutExcuse { get; set; }

        public double CheckOutExcuseHours { get; set; }

        public bool IsAbsentEmployee { get; set; }

        public DateTime ProcessingDate { get; set; }
    }
}
