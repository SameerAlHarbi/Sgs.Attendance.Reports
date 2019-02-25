using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sgs.Attendance.Reports.Migrations
{
    public partial class fixingEmployeeDayReportDuration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "ActualTotalDurationTime",
                table: "EmployeeDaysReports",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "WorkTotalDurationTime",
                table: "EmployeeDaysReports",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualTotalDurationTime",
                table: "EmployeeDaysReports");

            migrationBuilder.DropColumn(
                name: "WorkTotalDurationTime",
                table: "EmployeeDaysReports");
        }
    }
}
