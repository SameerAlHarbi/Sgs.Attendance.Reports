using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sgs.Attendance.Reports.ViewModels
{
    public class ShortEmployeeInfoViewModel
    {
        public int EmployeeId { get; set; }

        public string Name { get; set; }

        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; }
    }
}
