using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sgs.Attendance.Reports.Migrations
{
    public partial class DaysReports : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmployeeDaysReports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DayDate = table.Column<DateTime>(nullable: false),
                    EmployeeId = table.Column<int>(nullable: false),
                    IsVacation = table.Column<bool>(nullable: false),
                    VacationTypeName = table.Column<string>(nullable: true),
                    VacationRegisterDate = table.Column<DateTime>(nullable: true),
                    IsDelegationRequest = table.Column<bool>(nullable: false),
                    DelegationRegisterDate = table.Column<DateTime>(nullable: true),
                    ContractCheckInDateTime = table.Column<DateTime>(nullable: true),
                    ContractCheckOutDateTime = table.Column<DateTime>(nullable: true),
                    ContractWorkDurationTime = table.Column<TimeSpan>(nullable: false),
                    ContractWorkDuration = table.Column<double>(nullable: false),
                    ActualCheckInDateTime = table.Column<DateTime>(nullable: true),
                    ActualCheckOutDateTime = table.Column<DateTime>(nullable: true),
                    ActualWorkDurationTime = table.Column<TimeSpan>(nullable: false),
                    ActualWorkDuration = table.Column<double>(nullable: false),
                    CheckInDateTime = table.Column<DateTime>(nullable: true),
                    CheckOutDateTime = table.Column<DateTime>(nullable: true),
                    WorkDurationTime = table.Column<TimeSpan>(nullable: false),
                    WorkDuration = table.Column<double>(nullable: false),
                    CheckInLateDurationTime = table.Column<TimeSpan>(nullable: false),
                    CheckInLateDuration = table.Column<double>(nullable: false),
                    CheckOutEarlyDurationTime = table.Column<TimeSpan>(nullable: false),
                    CheckOutEarlyDuration = table.Column<double>(nullable: false),
                    WasteDurationTime = table.Column<TimeSpan>(nullable: false),
                    WasteDuration = table.Column<double>(nullable: false),
                    AttendanceProof = table.Column<int>(nullable: false),
                    CheckInExcuse = table.Column<bool>(nullable: false),
                    CheckInExcuseHours = table.Column<double>(nullable: false),
                    CheckOutExcuse = table.Column<bool>(nullable: false),
                    CheckOutExcuseHours = table.Column<double>(nullable: false),
                    IsAbsentEmployee = table.Column<bool>(nullable: false),
                    ProcessingDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeDaysReports", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeDaysReports");
        }
    }
}
