using Sameer.Shared;
using System;

namespace Sgs.Attendance.Reports.Models
{
    public class EmployeeExcuse : ISameerObject
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public DateTime RegisterDate { get; set; }

        [Unique(nameof(EmployeeId), nameof(ExcuseType))]
        public DateTime ExcueseDate { get; set; }

        public ExcuseType ExcuseType { get; set; }

        public double? ExcuseHours { get; set; }

        public string Note { get; set; }
    }
}
