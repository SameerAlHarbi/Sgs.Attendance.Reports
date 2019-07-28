using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sgs.Attendance.Reports.Migrations
{
    public partial class absentnotification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AbsentNotifiedByEmployeeId",
                table: "EmployeeDaysReports",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AbsentNotifiedByEmployeeName",
                table: "EmployeeDaysReports",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AbsentNotifiedDate",
                table: "EmployeeDaysReports",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AbsentsNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EmployeeId = table.Column<int>(nullable: false),
                    AbsentDate = table.Column<DateTime>(nullable: false),
                    SendDate = table.Column<DateTime>(nullable: false),
                    ByEmployeeId = table.Column<string>(nullable: true),
                    ByEmployeeName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AbsentsNotifications", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AbsentsNotifications");

            migrationBuilder.DropColumn(
                name: "AbsentNotifiedByEmployeeId",
                table: "EmployeeDaysReports");

            migrationBuilder.DropColumn(
                name: "AbsentNotifiedByEmployeeName",
                table: "EmployeeDaysReports");

            migrationBuilder.DropColumn(
                name: "AbsentNotifiedDate",
                table: "EmployeeDaysReports");
        }
    }
}
