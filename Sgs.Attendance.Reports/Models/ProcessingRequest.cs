using Sameer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sgs.Attendance.Reports.Models
{
    public class ProcessingRequest : ISameerObject
    {
        public int Id { get; set; }

        public DateTime RequestDate { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string Employees { get; set; }

        [Unique(UniqueValue =false,ErrorMessage ="يوجد طلب معالجة مفتوح مسبقاً")]
        public bool Completed { get; set; }

        public int UserId { get; set; }
    }
}
