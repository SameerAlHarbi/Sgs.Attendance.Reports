using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sgs.Attendance.Reports.Migrations
{
    public partial class FixEmployeeDayReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualWorkDuration",
                table: "EmployeeDaysReports");

            migrationBuilder.DropColumn(
                name: "CheckInLateDuration",
                table: "EmployeeDaysReports");

            migrationBuilder.DropColumn(
                name: "CheckOutEarlyDuration",
                table: "EmployeeDaysReports");

            migrationBuilder.DropColumn(
                name: "ContractWorkDuration",
                table: "EmployeeDaysReports");

            migrationBuilder.DropColumn(
                name: "WasteDuration",
                table: "EmployeeDaysReports");

            migrationBuilder.DropColumn(
                name: "WorkDuration",
                table: "EmployeeDaysReports");

            migrationBuilder.RenameColumn(
                name: "VacationTypeName",
                table: "EmployeeDaysReports",
                newName: "VacationName");

            migrationBuilder.RenameColumn(
                name: "DelegationRegisterDate",
                table: "EmployeeDaysReports",
                newName: "VacationRequestDate");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "WorkDurationTime",
                table: "EmployeeDaysReports",
                nullable: true,
                oldClrType: typeof(TimeSpan));

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "WasteDurationTime",
                table: "EmployeeDaysReports",
                nullable: true,
                oldClrType: typeof(TimeSpan));

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "ContractWorkDurationTime",
                table: "EmployeeDaysReports",
                nullable: true,
                oldClrType: typeof(TimeSpan));

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "CheckOutEarlyDurationTime",
                table: "EmployeeDaysReports",
                nullable: true,
                oldClrType: typeof(TimeSpan));

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "CheckInLateDurationTime",
                table: "EmployeeDaysReports",
                nullable: true,
                oldClrType: typeof(TimeSpan));

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "ActualWorkDurationTime",
                table: "EmployeeDaysReports",
                nullable: true,
                oldClrType: typeof(TimeSpan));

            migrationBuilder.AddColumn<DateTime>(
                name: "DelegationRequestDate",
                table: "EmployeeDaysReports",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVacationRequest",
                table: "EmployeeDaysReports",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DelegationRequestDate",
                table: "EmployeeDaysReports");

            migrationBuilder.DropColumn(
                name: "IsVacationRequest",
                table: "EmployeeDaysReports");

            migrationBuilder.RenameColumn(
                name: "VacationRequestDate",
                table: "EmployeeDaysReports",
                newName: "DelegationRegisterDate");

            migrationBuilder.RenameColumn(
                name: "VacationName",
                table: "EmployeeDaysReports",
                newName: "VacationTypeName");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "WorkDurationTime",
                table: "EmployeeDaysReports",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "WasteDurationTime",
                table: "EmployeeDaysReports",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "ContractWorkDurationTime",
                table: "EmployeeDaysReports",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "CheckOutEarlyDurationTime",
                table: "EmployeeDaysReports",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "CheckInLateDurationTime",
                table: "EmployeeDaysReports",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "ActualWorkDurationTime",
                table: "EmployeeDaysReports",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ActualWorkDuration",
                table: "EmployeeDaysReports",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CheckInLateDuration",
                table: "EmployeeDaysReports",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "CheckOutEarlyDuration",
                table: "EmployeeDaysReports",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "ContractWorkDuration",
                table: "EmployeeDaysReports",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "WasteDuration",
                table: "EmployeeDaysReports",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "WorkDuration",
                table: "EmployeeDaysReports",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
