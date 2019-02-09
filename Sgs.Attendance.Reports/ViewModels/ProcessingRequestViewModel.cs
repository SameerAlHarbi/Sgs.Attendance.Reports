using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sgs.Attendance.Reports.ViewModels
{
    public class ProcessingRequestViewModel
    {
        public int Id { get; set; }

        public DateTime RequestDate { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string Employees { get; set; }

        public bool Completed { get; set; }

        public string Status => Completed ? "مكتمل" : "مفتوح";

        public int UserId { get; set; }
    }
}
