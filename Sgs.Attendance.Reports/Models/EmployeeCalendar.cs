using Sameer.Shared;
using System;
using System.ComponentModel.DataAnnotations;

namespace Sgs.Attendance.Reports.Models
{
    public class EmployeeCalendar : ISameerObject
    {
        public int Id { get; set; }

        [Range(1, 10000, ErrorMessage = "Employee id not valid !")]
        public int EmployeeId { get; set; }

        public ContractWorkTime ContractWorkTime { get; set; }

        public AttendanceProof AttendanceProof { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Note { get; set; }

        public int UserId { get; set; }
    }
}
