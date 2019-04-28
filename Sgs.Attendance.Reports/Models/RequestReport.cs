using Sameer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sgs.Attendance.Reports.Models
{
    public class RequestReport : ISameerObject
    {
        public int Id { get; set; }

        public DateTime RequestDate { get; set; }

        public string ReportType { get; set; }

        public string ByUserId { get; set; }

        public string ByUserName { get; set; }

        public string FilterText { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

    }
}
