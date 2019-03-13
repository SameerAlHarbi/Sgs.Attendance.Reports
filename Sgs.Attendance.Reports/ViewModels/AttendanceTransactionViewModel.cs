using System;

namespace Sgs.Attendance.Reports.ViewModels
{
    public class AttendanceTransactionViewModel
    {
        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; }

        public string DepartmentName { get; set; }

        public DateTime TransactionDate { get; set; }
    }
}
