using Microsoft.EntityFrameworkCore.Migrations;

namespace Sgs.Attendance.Reports.Migrations
{
    public partial class addEmployeeNameToDayReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DepartmentId",
                table: "EmployeeDaysReports",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentName",
                table: "EmployeeDaysReports",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmployeeName",
                table: "EmployeeDaysReports",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "EmployeeDaysReports");

            migrationBuilder.DropColumn(
                name: "DepartmentName",
                table: "EmployeeDaysReports");

            migrationBuilder.DropColumn(
                name: "EmployeeName",
                table: "EmployeeDaysReports");
        }
    }
}
