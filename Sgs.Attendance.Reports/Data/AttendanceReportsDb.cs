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

        public DbSet<EmployeeCalendar> EmployeesCalendars { get; set; }

        public DbSet<EmployeeExcuse> EmployeesExcuses { get; set; }

        public DbSet<ProcessingRequest> ProcessingRequests { get; set; }

        public DbSet<EmployeeDayReport> EmployeeDaysReports { get; set; }

        public DbSet<WorkCalendar> WorkCalendars { get; set; }

        public DbSet<WorkShift> WorkShifts { get; set; }

        public DbSet<RequestReport> RequestsReports { get; set; }

        public DbSet<AbsentNotification> AbsentsNotifications { get; set; }

    }
}
