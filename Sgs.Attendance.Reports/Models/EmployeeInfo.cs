using Sameer.Shared;
using System.ComponentModel.DataAnnotations;

namespace Sgs.Attendance.Reports.Models
{
    public class EmployeeInfo : ISameerObject
    {
        public int Id { get; set; }

        [Unique(ErrorMessage = "Employee Exist !")]
        [Range(1, 10000, ErrorMessage = "Employee id not valid !")]
        public int EmployeeId { get; set; }

        public ContractWorkTime ContractWorkTime { get; set; }

        public AttendanceProof AttendanceProof { get; set; }

        public string Note { get; set; }
    }
}
