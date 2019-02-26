using Sameer.Shared;
using System;

namespace Sgs.Attendance.Reports.Models
{
    public class EmployeeDayReport : ISameerObject
    {
        public int Id { get; set; }

        public DateTime DayDate { get; set; }

        public int EmployeeId { get; set; }

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

        public DateTime? ActualCheckInDateTime { get; set; }

        public DateTime? ActualCheckOutDateTime { get; set; }

        public TimeSpan? ActualWorkDurationTime { get; set; }

        public TimeSpan? ActualTotalDurationTime { get; set; }

        public DateTime? CheckInDateTime { get; set; }

        public DateTime? CheckOutDateTime { get; set; }

        public TimeSpan? WorkDurationTime { get; set; }

        public TimeSpan? WorkTotalDurationTime { get; set; }

        public TimeSpan? CheckInLateDurationTime { get; set; }

        public TimeSpan? CheckOutEarlyDurationTime { get; set; }

        public TimeSpan? WasteDurationTime { get; set; }

        public AttendanceProof AttendanceProof { get; set; }

        public bool CheckInExcuse { get; set; }

        public double CheckInExcuseHours { get; set; }

        public bool IsOpenCheckInExcuse => CheckInExcuse && CheckInExcuseHours == 0;

        public bool CheckOutExcuse { get; set; }

        public double CheckOutExcuseHours { get; set; }

        public bool IsOpenCheckOutExcuse => CheckOutExcuse && CheckOutExcuseHours == 0;

        public bool IsAbsentEmployee { get; set; }

        public DateTime ProcessingDate { get; set; }
    }
}
