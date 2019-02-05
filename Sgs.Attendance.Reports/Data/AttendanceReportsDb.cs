using Microsoft.EntityFrameworkCore;
using Sgs.Attendance.Reports.Models;

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
