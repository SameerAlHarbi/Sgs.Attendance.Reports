using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Sgs.Attendance.Reports.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sgs.Attendance.Reports.Data
{
    public class AttendanceReportsDb : DbContext
    {
        public AttendanceReportsDb(DbContextOptions options) 
            : base(options)
        {
        }

        public DbSet<EmployeeInfo> EmployeesInfo { get; set; }
    }
}
