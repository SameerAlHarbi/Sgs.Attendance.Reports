using System;
using System.Collections.Generic;

namespace Sgs.Attendance.Reports.ViewModels
{
    public class EmployeeMonthReportViewModel
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; }

        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime ToDate { get; set; }

        public TimeSpan TotalWasteHoursTime { get; set; }

        public double TotalWasteHours { get; set; }

        public TimeSpan TotalAllowedHoursAvarageTime { get; set; }

        public double TotalAllowedHoursAvarage { get; set; }

        public TimeSpan WasteHoursTime { get; set; }

        public double WasteHours { get; set; }

        public int TotalWasteDays { get; set; }

        public int TotalAllowedDays { get; set; }

        public int WasteDays { get; set; }

        public int TotalAbsentsDays { get; set; }

        public TimeSpan TotalContractWorkDurationTime { get; set; }

        public double TotalContractWorkDuration { get; set; }

        public TimeSpan TotalActualWorkDurationTime { get; set; }

        public double TotalActualWorkDuration { get; set; }

        public TimeSpan ContractWorkDurationAvarageTime { get; set; }

        public double ContractWorkDurationAvarage { get; set; }

        public DateTime ProcessingDate { get; set; }

        public int UserId { get; set; }

        public string Note { get; set; }

        public List<EmployeeDayReportViewModel> DayReportsList { get; set; }

        public int  VacationsDays { get; set; }

        public int  SingleProofDays { get; set; }

        public bool AllAbsentsNotified { get; set; }
        public bool AllAbsentsWithNotes { get; set; }
    }
}
