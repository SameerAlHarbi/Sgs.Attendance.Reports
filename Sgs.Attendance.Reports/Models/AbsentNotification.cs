using Sameer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sgs.Attendance.Reports.Models
{
    public class AbsentNotification:ISameerObject
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public DateTime AbsentDate { get; set; }

        public DateTime SendDate { get; set; }

        public string ByEmployeeId { get; set; }

        public string ByEmployeeName { get; set; }
    }
}
